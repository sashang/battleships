open Battleship
open Battleship.Base
open System

[<EntryPoint>]
let main argv =
    let game = Game.newGame ()

    let ship1 = Ship.create {X = 1u; Y = 1u} 2u Ship.Vertical
    let ship2 = Ship.create {X = 3u; Y = 1u} 1u Ship.Horizontal
    match ship1,ship2 with
    | Ok ship1, Ok ship2 ->
        let r =
            Result.bind (fun game -> Game.addShip Game.Player1 ship1 game) (Ok game)
            |> Result.bind (fun game -> Game.addShip Game.Player1 ship2 game)
        match r with
        | Ok game ->
            let (game', hit) = Game.attack Game.Player2ToPlayer1 {X = 1u; Y = 1u}  game
            let (game', hit) = Game.attack Game.Player2ToPlayer1 {X = 2u; Y = 1u}  game'
            let (game', hit) = Game.attack Game.Player2ToPlayer1 {X = 3u; Y = 1u}  game'
            printfn "state: [%A]" (Game.getPlayerState game')
        | Error msg -> failwith msg
    | Error msg, _ ->
        failwith msg
    | _,Error msg ->
        failwith msg

    0