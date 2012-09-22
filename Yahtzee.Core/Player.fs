namespace Yahtzee.Core

type PlayerType = | Human | CPU

type Player(playerType) =
    let scorecard = new Scorecard()

    member this.Type = playerType
    member this.Scorecard = scorecard

    member this.Score(turn) = this.Scorecard.Score(turn)