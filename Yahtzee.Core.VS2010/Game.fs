namespace Yahtzee.Core

type GameStatus = InProgress | Over

type GameState = {
    CurrentPlayer : int;
    Round : int;
    Points : int
}

type TurnResult =
    | Success of GameState
    | Failure of string

type Game = {
    Players : Player list;
    mutable Round : int;
    mutable Status : GameStatus;
    mutable CurrentPlayer : int;
} with
    member this.NumberOfPlayers = this.Players.Length
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
                | ScoreResult.Success(_) when this.Round = 13 && this.CurrentPlayer = this.NumberOfPlayers - 1 -> GameStatus.Over
                | _ -> GameStatus.InProgress
            if gameStatus = GameStatus.InProgress then            
                this.CurrentPlayer <- (this.CurrentPlayer + 1) % this.NumberOfPlayers
                if this.CurrentPlayer = 0 then
                    this.Round <- this.Round + 1
            match scoreResult with
            | ScoreResult.Success(points) -> 
                let gameState = { CurrentPlayer = this.CurrentPlayer; Round = this.Round; Points = points }
                TurnResult.Success(gameState)
            | ScoreResult.Failure(message) -> TurnResult.Failure(message)


