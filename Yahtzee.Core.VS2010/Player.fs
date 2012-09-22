namespace Yahtzee.Core

type PlayerType = | Human | CPU

type Player = {
    Type : PlayerType;
    Scorecard : Scorecard
} with
    member this.Score(turn) =
        this.Scorecard.Score(turn)