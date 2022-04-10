namespace Battleship

module Base =
    [<Literal>]
    let BoardDimension = 10u

    type Coordinate = {
        X : uint32
        Y : uint32
    }

    type Cell = {
        hasShip : bool
    }

module Ship =
    open Base

    type ShipState =
        | Operational
        | Destroyed

    type Orientation =
        | Horizontal
        | Vertical

    type Ship = {
        Length : uint32
        Coords : (Base.Coordinate * ShipState) list
        Orientation : Orientation
        State : ShipState
    }

    /// Create a ship and validate it.
    let create (pos : Base.Coordinate) (length : uint32) (orientation : Orientation) =
        if length = 0u || length > 10u then
            Error "Length must be between 1 and 10 inclusive"
        else if pos.X >= Base.BoardDimension || pos.Y >= Base.BoardDimension then
            Error "Invalid position"
        else if pos.X + length > Base.BoardDimension && orientation = Horizontal || pos.Y + length > Base.BoardDimension && orientation = Vertical then
            Error "Ship length exceeds board dimensions."
        else
            let coords = [
                if orientation = Horizontal then
                    for i = 0u to length - 1u do
                        yield ({Coordinate.X = pos.X; Y = pos.Y + i}, Operational)
                else
                    for i = 0u to length - 1u do
                        yield ({Coordinate.X = pos.X + i; Y = pos.Y}, Operational)
            ]
            Ok { Length = length; Coords = coords; Orientation = orientation; State = Operational }

    let getState (ship: Ship) =
        let result = List.tryFind (fun (_, state) -> state = Operational) ship.Coords
        match result with
        | Some _ -> Operational
        | None -> Destroyed

    let attack attackCoord ship =
        let mutable hit = false
        let coords' =
            List.map
                (fun (coordinate, oldState) -> if coordinate = attackCoord then hit <- true; (coordinate, Destroyed) else (coordinate, oldState))
                ship.Coords
        {ship with Coords = coords'}, hit

module Board =
    open Base
    let create () =
        Array2D.create (int BoardDimension) (int BoardDimension) {hasShip = false}



module Player =
    open Base
    open Ship

    type PlayerState =
        | Playing // when the player has ships that are not destroyed
        | Lost // when all ships destroyed
        | Won // when other players ships are destroyed.

    type Player = {
        Board : Cell [,]
        State : PlayerState
        Ships: Ship list
    }

    let newPlayer () =
        {Board = Board.create () ; State = Playing; Ships = []}


    /// Add a ship. Returns None if the ship could not be added to the board, i.e. the coordinates were already occupied.
    /// Returns the player unchanged in this case. If the ship is added it returns an updated player state.
    let addShip (newShip : Ship) (player: Player) =
        //check that the coordinates are not already occupied

        let occupied =
            newShip.Coords
            |> List.tryFind (fun ({Coordinate.X = x; Y = y},_) -> player.Board[int x, int y].hasShip)

        match occupied with
        | Some _ -> Error "Position occupied"
        | None ->
            let board' =
                newShip.Coords
                |> List.iter (fun ({Coordinate.X = x; Y = y},_) -> player.Board[int x, int y] <- {hasShip = true})
                player.Board
            let newListOfShips = newShip::player.Ships
            Ok {player with Board = board'; State = Playing; Ships = newListOfShips}

    /// Attack this player at the given coordinate
    let recvAttack (attackCoord: Coordinate) player =
        let ships', hitList = List.map (fun ship -> Ship.attack attackCoord ship) player.Ships |> List.unzip
        let state' =
            match List.tryFind (fun x -> Ship.getState x = Ship.Operational) ships' with
            | Some _ -> Playing
            | None -> Lost
        let anyHit = hitList |> List.fold (fun state wasHit -> if not state then wasHit else true) false
        {player with Ships = ships'; State = state'}, anyHit

module Game =
    open Base

    type Game = private {
        Player1 : Player.Player
        Player2 : Player.Player
    }

    type AttackVector =
        | Player1ToPlayer2
        | Player2ToPlayer1

    type PlayerEnum =
        | Player1
        | Player2

    let newGame () =
        {Player1 = Player.newPlayer (); Player2 = Player.newPlayer ()}

    let addShip player ship game =
        match player with
        | Player1 ->
            let p = Player.addShip ship game.Player1
            match p with
            | Error msg -> Error msg
            | Ok p ->
                Ok {game with Player1 = p}
        | Player2 ->
            let p = Player.addShip ship game.Player2
            match p with
            | Error msg -> Error msg
            | Ok p ->
                Ok {game with Player2 = p}

    let attack (target: AttackVector) (pos: Coordinate) game =
        match target with
        | Player1ToPlayer2 ->
            let player', wasHit = Player.recvAttack pos game.Player2
            {game with Player2 = player'}, wasHit
        | Player2ToPlayer1 ->
            let player', wasHit = Player.recvAttack pos game.Player1
            {game with Player1 = player'}, wasHit

    let getPlayerState game =
        game.Player1.State, game.Player2.State