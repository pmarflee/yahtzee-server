namespace Yahtzee.Tests

open Xunit
open Yahtzee.Core
open Yahtzee.Core.Scoring

module ScoringTests =

    [<Fact>]
    let ``Score Fives With 2 Fives Should Equal 10``() =
        let dice = [ Die.One; Die.Two; Die.Five; Die.Four; Die.Five ];    

        let expected = 10
        let actual = Scoring.Score { Dice = dice; Category = Fives }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score Fives With 0 Fives Should Equal 0``() =
        let dice = [ Die.One; Die.Two; Die.Two; Die.Four; Die.Six ];

        let expected = 0
        let actual = Scoring.Score { Dice = dice; Category = Fives }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score ThreeOfAKind With Three Ones Should Equal The Sum Of The Dice``() =
        let dice = [ Die.One; Die.Two; Die.One; Die.Four; Die.One ];

        let expected = 9
        let actual = Scoring.Score { Dice = dice; Category = ThreeOfAKind }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score ThreeOfAKind With Two Ones Should Equal 0``() =
        let dice = [ Die.Five; Die.Two; Die.One; Die.Four; Die.One ];

        let expected = 0
        let actual = Scoring.Score { Dice = dice; Category = ThreeOfAKind }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score FourOfAKind With Four Ones Should Equal The Sum Of The Dice``() =
        let dice = [ Die.One; Die.One; Die.One; Die.Four; Die.One ];

        let expected = 8
        let actual = Scoring.Score { Dice = dice; Category = FourOfAKind }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score FourOfAKind With Three Ones Should Equal 0``() =
        let dice = [ Die.One; Die.One; Die.One; Die.Four; Die.Five ];

        let expected = 0
        let actual = Scoring.Score { Dice = dice; Category = FourOfAKind }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score FullHouse With Three Ones And Two Fours Should Equal 25``() =
        let dice = [ Die.One; Die.One; Die.One; Die.Four; Die.Four ];

        let expected = 25
        let actual = Scoring.Score { Dice = dice; Category = FullHouse }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score FullHouse With Five Ones Should Equal 0``() =
        let dice = [ Die.One; Die.One; Die.One; Die.One; Die.One ];

        let expected = 0
        let actual = Scoring.Score { Dice = dice; Category = FullHouse }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score FullHouse With Two Ones And Two Fours Should Equal 0``() =
        let dice = [ Die.One; Die.One; Die.Four; Die.Four; Die.Five ];

        let expected = 0
        let actual = Scoring.Score { Dice = dice; Category = FullHouse }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score SmallStraight With 1234 Should Equal 30``() =
        let dice = [ Die.One; Die.One; Die.Two; Die.Four; Die.Three ];    

        let expected = 30
        let actual = Scoring.Score { Dice = dice; Category = SmallStraight }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score SmallStraight With 2345 Should Equal 30``() =
        let dice = [ Die.Three; Die.Five; Die.Two; Die.Four; Die.Three ];        

        let expected = 30
        let actual = Scoring.Score { Dice = dice; Category = SmallStraight }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score SmallStraight With 3456 Should Equal 30``() =
        let dice = [ Die.Three; Die.Five; Die.Six; Die.Four; Die.Three ];

        let expected = 30
        let actual = Scoring.Score { Dice = dice; Category = SmallStraight }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score SmallStraight With 12345 Should Equal 30``() =
        let dice = [ Die.One; Die.Two; Die.Three; Die.Four; Die.Five ];    

        let expected = 30
        let actual = Scoring.Score { Dice = dice; Category = SmallStraight }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score LargeStraight With 12345 Should Equal 40``() =
        let dice = [ Die.One; Die.Two; Die.Four; Die.Three; Die.Five ];

        let expected = 40
        let actual = Scoring.Score { Dice = dice; Category = LargeStraight }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score LargeStraight With 23456 Should Equal 40``() =
        let dice = [ Die.Two; Die.Six; Die.Four; Die.Three; Die.Five ];

        let expected = 40
        let actual = Scoring.Score { Dice = dice; Category = LargeStraight }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score Yahtzee With Five Ones Should Equal 50``() =
        let dice = [ Die.One; Die.One; Die.One; Die.One; Die.One ];

        let expected = 50
        let actual = Scoring.Score { Dice = dice; Category = Yahtzee }

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Score Yahtzee With Four Ones Should Equal 0``() =
        let dice = [ Die.One; Die.One; Die.One; Die.One; Die.Two ];

        let expected = 0
        let actual = Scoring.Score { Dice = dice; Category = Yahtzee }

        Assert.Equal(expected, actual)