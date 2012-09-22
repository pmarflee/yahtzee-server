namespace Yahtzee.Core

open System.Collections.Generic

type GameStatus = InProgress | Over

type GameState = {
    CurrentPlayer : int;
    Round : int;
    Points : int
}

type TurnResult =
    | Success of GameState
    | Failure of string

type Game(players : Player list) =
    let mutable round = 0
    let mutable status = GameStatus.InProgress
    let mutable currentPlayer = 0

    member this.Players = players
    member this.Round = round
    member this.Status = status
    member this.CurrentPlayer = currentPlayer
    member this.NumberOfPlayers = players.Length
    member this.Score(player, turn) =
        if this.Status = GameStatus.Over then
            TurnResult.Failure("Game is over")
        elif player <> this.CurrentPlayer then
            TurnResult.Failure(sprintf "Player %d is not the current player" player)
        else
            let player = List.nth this.Players this.CurrentPlayer
            let scoreResult = player.Score turn
            let gameStatus =
                match scoreResult with
                | ScoreResult.Success(_) when this.Players |> List.forall (fun p -> p.Scorecard.IsFull) -> GameStatus.Over
                | _ -> GameStatus.InProgress
            if gameStatus = GameStatus.InProgress then            
                currentPlayer <- (this.CurrentPlayer + 1) % this.NumberOfPlayers
                if this.CurrentPlayer = 0 then
                    round <- this.Round + 1
            match scoreResult with
            | ScoreResult.Success(points) -> 
                let gameState = { CurrentPlayer = this.CurrentPlayer; Round = this.Round; Points = points }
                TurnResult.Success(gameState)
            | ScoreResult.Failure(message) -> TurnResult.Failure(message)
    static member Create(humans, cpus) =        
        let players = [
            for i in 1 .. humans -> new Player(Human)
            for i in 1 .. cpus -> new Player(CPU)
        ]
        new Game(players)        


