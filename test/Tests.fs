module Tests

open System
open Xunit
open Battleship
open Battleship.Base
open Swensen.Unquote

[<Fact>]
let ``Ship creation`` () =
    let shipResult = Ship.create {X=0u; Y=0u} 10u Ship.Horizontal
    let expected = Ok ({Ship.Coords = [for i = 0u to 9u do yield {X=0u; Y=i},Ship.Operational]
                        Ship.Length = 10u
                        Ship.Orientation = Ship.Horizontal;
                        Ship.State = Ship.Operational})
    shipResult =! expected
    let shipResult = Ship.create {X=0u; Y=0u} 11u Ship.Horizontal
    let expected = Error "Length must be between 1 and 10 inclusive"
    shipResult =! expected
    let shipResult = Ship.create {X=0u; Y=0u} 11u Ship.Vertical
    let expected = Error "Length must be between 1 and 10 inclusive"
    shipResult =! expected
    let shipResult = Ship.create {X=0u; Y=0u} 0u Ship.Vertical
    let expected = Error "Length must be between 1 and 10 inclusive"
    shipResult =! expected
    let shipResult = Ship.create {X=0u; Y=0u} 0u Ship.Horizontal
    let expected = Error "Length must be between 1 and 10 inclusive"
    shipResult =! expected
    let shipResult = Ship.create {X=11u; Y=0u} 1u Ship.Horizontal
    let expected = Error "Invalid position"
    shipResult =! expected

[<Fact>]
let ``Adding ship to board`` () =
    // create 3 ships in different positions and add them to the board.
    // all should succeed
    let ship1 = Ship.create {X=0u; Y=0u} 2u Ship.Horizontal
    let ship2 = Ship.create {X=1u; Y=0u} 2u Ship.Vertical
    let ship3 = Ship.create {X=7u; Y=5u} 3u Ship.Horizontal
    match ship1, ship2, ship3 with
    | Ok ship1, Ok ship2, Ok ship3 ->
        let game = Game.newGame ()
        let game' = Game.addShip Game.Player1 ship1 game
        // first ship should return Ok
        match game' with
        | Ok game' ->
            let game' = Game.addShip Game.Player1 ship2 game'
            match game' with
            | Ok game' ->
                let game' = Game.addShip Game.Player1 ship3 game'
                match game' with
                | Ok _ -> ()
                | Error msg -> msg =! ""
            | Error msg -> msg =! ""
        | Error msg -> msg =! ""

    | _ -> "Creation of ships failed. All ships should be Ok" =! ""

[<Fact>]
let ``Adding ship to board same position`` () =
    // create ships in the same position
    let ship1 = Ship.create {X=0u; Y=0u} 2u Ship.Horizontal
    let ship2 = Ship.create {X=0u; Y=0u} 2u Ship.Horizontal
    match ship1, ship2 with
    | Ok ship1, Ok ship2 ->
        let game = Game.newGame ()
        let game' = Game.addShip Game.Player1 ship1 game
        // first ship should return Ok
        match game' with
        | Ok game' ->
            let game' = Game.addShip Game.Player1 ship2 game'
            match game' with
            | Ok _ -> "Addition of second ship should return Error" =! ""
            // second ship should return because in same position as first
            | Error msg -> ()
        | Error msg -> msg =! ""

    | _ -> "All created ships should be Ok" =! ""


[<Fact>]
let ``Adding ship with overlap`` () =
    let ship1 = Ship.create {X=3u; Y=5u} 4u Ship.Horizontal
    let ship2 = Ship.create {X=2u; Y=5u} 2u Ship.Vertical
    match ship1, ship2 with
    | Ok ship1, Ok ship2 ->
        let game = Game.newGame ()
        let game' = Game.addShip Game.Player1 ship1 game
        // first ship should return Ok
        match game' with
        | Ok game' ->
            let game' = Game.addShip Game.Player1 ship2 game'
            match game' with
            | Ok _ -> "Addition of second ship should return Error" =! ""
            // 2nd ship should return Error
            | Error msg -> ()
        | Error msg -> msg =! ""

    | _ -> "All created ships should be Ok" =! ""


[<Fact>]
let ``Attack`` () =
    let game = Game.newGame ()
    let ship1 = Ship.create {X=0u; Y=0u} 2u Ship.Horizontal
    let ship2 = Ship.create {X=1u; Y=0u} 2u Ship.Vertical
    match ship1, ship2 with
    | Ok ship1, Ok ship2 ->
        let game' =
            Ok game
            |> Result.bind (Game.addShip Game.Player1 ship1)
            |> Result.bind (Game.addShip Game.Player2 ship2)
        match game' with
        | Ok game' ->
            //player 1 attacks and hits
            let game', wasHit = Game.attack Game.Player1ToPlayer2 {X=2u;Y=0u} game'
            wasHit =! true

            //player 2 attacks and misses
            let game', wasHit = Game.attack Game.Player2ToPlayer1 {X=3u;Y=2u} game'
            wasHit =! false

            //player 1 attacks and hits
            let game', wasHit = Game.attack Game.Player1ToPlayer2 {X=1u;Y=0u} game'
            wasHit =! true

            let state = Game.getPlayerState game'
            state =! (Player.Playing, Player.Lost)
        | Error msg ->
            msg =! ""
    | _ -> "All created ships should be Ok" =! ""