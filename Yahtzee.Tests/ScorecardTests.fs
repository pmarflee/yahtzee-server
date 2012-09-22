namespace Yahtzee.Tests

open System
open Xunit
open Yahtzee.Core

module ScorecardTests =
    
    module UpperCategoryTests =

        [<Fact>]
        let ``Score Ones Should Record Score In Ones Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = Ones; Dice = [Die.One; Die.One; Die.One; Die.One; Die.One] })

            Assert.Equal(ScoreResult.Success(5), result)
            Assert.Equal((true, 5), scorecard.UpperSection.TryGetScore(Ones))

        [<Fact>]
        let ``Score Twos Should Record Score In Twos Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = Twos; Dice = [Die.One; Die.Two; Die.One; Die.Two; Die.One] })

            Assert.Equal(ScoreResult.Success(4), result)
            Assert.Equal((true, 4), scorecard.UpperSection.TryGetScore(Twos))

        [<Fact>]
        let ``Score Threes Should Record Score In Threes Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = Threes; Dice = [Die.Three; Die.Two; Die.Three; Die.Two; Die.Three] })

            Assert.Equal(ScoreResult.Success(9), result)
            Assert.Equal((true, 9), scorecard.UpperSection.TryGetScore(Threes))

        [<Fact>]
        let ``Score Fours Should Record Score In Fours Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = Fours; Dice = [Die.One; Die.Four; Die.Four; Die.Two; Die.One] })

            Assert.Equal(ScoreResult.Success(8), result)
            Assert.Equal((true, 8), scorecard.UpperSection.TryGetScore(Fours))
        
        [<Fact>]
        let ``Score Fives Should Record Score In Fives Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = Fives; Dice = [Die.One; Die.Two; Die.Five; Die.Two; Die.Five] })

            Assert.Equal(ScoreResult.Success(10), result)
            Assert.Equal((true, 10), scorecard.UpperSection.TryGetScore(Fives))

        [<Fact>]
        let ``Score Sixes Should Record Score In Sixes Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = Sixes; Dice = [Die.Six; Die.Two; Die.Six; Die.Six; Die.One] })

            Assert.Equal(ScoreResult.Success(18), result)
            Assert.Equal((true, 18), scorecard.UpperSection.TryGetScore(Sixes))

        [<Fact>]
        let ``Score Ones Should Fail When Category Is Already Scored``() =
            let scorecard = new Scorecard()
            scorecard.Score({ Category = Ones; Dice = [Die.Six; Die.Two; Die.Six; Die.Six; Die.One] }) |> ignore

            let result = scorecard.Score({ Category = Ones; Dice = [Die.Six; Die.Two; Die.Six; Die.Six; Die.One] })
            
            Assert.False(match result with
                        | ScoreResult.Success(_) -> true
                        | ScoreResult.Failure(_) -> false)

    module LowerCategoryTests =
        
        [<Fact>]
        let ``Score ThreeOfAKind Should Record Score In ThreeOfAKind Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = ThreeOfAKind; Dice = [Die.One; Die.Two; Die.One; Die.Two; Die.One] })

            Assert.Equal(ScoreResult.Success(7), result)
            Assert.Equal((true, 7), scorecard.LowerSection.TryGetScore(ThreeOfAKind))

        [<Fact>]
        let ``Score FourOfAKind Should Record Score In FourOfAKind Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = FourOfAKind; Dice = [Die.One; Die.One; Die.One; Die.Two; Die.One] })

            Assert.Equal(ScoreResult.Success(6), result)
            Assert.Equal((true, 6), scorecard.LowerSection.TryGetScore(FourOfAKind))

        [<Fact>]
        let ``Score SmallStraight Should Record Score In SmallStraight Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = SmallStraight; Dice = [Die.One; Die.Two; Die.Three; Die.Four; Die.Five] })
            let score = int Scoring.LowerSectionScore.SmallStraight

            Assert.Equal(ScoreResult.Success(score), result)
            Assert.Equal((true, score), scorecard.LowerSection.TryGetScore(SmallStraight))

        [<Fact>]
        let ``Score LargeStraight Should Record Score In LargeStraight Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = LargeStraight; Dice = [Die.One; Die.Two; Die.Three; Die.Four; Die.Five] })
            let score = int Scoring.LowerSectionScore.LargeStraight

            Assert.Equal(ScoreResult.Success(score), result)
            Assert.Equal((true, score), scorecard.LowerSection.TryGetScore(LargeStraight))

        [<Fact>]
        let ``Score FullHouse Should Record Score In FullHouse Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = FullHouse; Dice = [Die.One; Die.One; Die.One; Die.Two; Die.Two] })
            let score = int Scoring.LowerSectionScore.FullHouse

            Assert.Equal(ScoreResult.Success(score), result)
            Assert.Equal((true, score), scorecard.LowerSection.TryGetScore(FullHouse))

        [<Fact>]
        let ``Score Chance Should Record Score In Chance Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = Chance; Dice = [Die.One; Die.Two; Die.Four; Die.Six; Die.One] })

            Assert.Equal(ScoreResult.Success(14), result)
            Assert.Equal((true, 14), scorecard.LowerSection.TryGetScore(Chance))

        [<Fact>]
        let ``Score Yahtzee Should Fail When Category Is Already Scored``() =
            let scorecard = new Scorecard()
            scorecard.Score({ Category = Yahtzee; Dice = [Die.One; Die.One; Die.One; Die.One; Die.One] }) |> ignore

            let result = scorecard.Score({ Category = Yahtzee; Dice = [Die.One; Die.One; Die.One; Die.One; Die.One] })
            
            Assert.False(match result with
                        | ScoreResult.Success(_) -> true
                        | ScoreResult.Failure(_) -> false)

    module YahtzeeScoringTests =

        [<Fact>]
        let ``Score Yahtzee Should Record Score In Yahtzee Category``() =
            let scorecard = new Scorecard()
            let result = scorecard.Score({ Category = Yahtzee; Dice = [Die.One; Die.One; Die.One; Die.One; Die.One] })
            let score = int Scoring.LowerSectionScore.Yahtzee

            Assert.Equal(ScoreResult.Success(score), result)
            Assert.Equal((true, score), scorecard.LowerSection.TryGetScore(Yahtzee))

        [<Fact>]
        let ``Score Upper Section With Yahtzee Succeeds With Bonus If Yahtzee Is Already Scored And Category Matches Dice``() =
            let scorecard = new Scorecard()
            let dice = [Die.One; Die.One; Die.One; Die.One; Die.One] 
            scorecard.Score({ Category = Yahtzee; Dice = dice }) |> ignore

            let result = scorecard.Score({ Category = Ones; Dice = dice })

            Assert.Equal(ScoreResult.Success(105), result)
            Assert.Equal((true, 5), scorecard.UpperSection.TryGetScore(Ones))

        [<Fact>]
        let ``Score Upper Section With Yahtzee Fails If Yahtzee Is Already Scored And Category Matches Dice And Category Is Already Scored``() =
            let scorecard = new Scorecard()
            let dice = [Die.One; Die.One; Die.One; Die.One; Die.One] 
            scorecard.Score({ Category = Yahtzee; Dice = dice }) |> ignore
            scorecard.Score({ Category = Ones; Dice = dice }) |> ignore

            let result = scorecard.Score({ Category = Ones; Dice = dice })

            Assert.False(match result with
                        | ScoreResult.Success(_) -> true
                        | ScoreResult.Failure(_) -> false)
            
        [<Fact>]
        let ``Score Lower Section With Yahtzee Succeeds If Yahtzee Is Already Scored And Category Matches Dice And Dice Category Is Already Scored``() =
            let scorecard = new Scorecard()
            let dice = [Die.One; Die.One; Die.One; Die.One; Die.One] 
            scorecard.Score({ Category = Yahtzee; Dice = dice }) |> ignore
            scorecard.Score({ Category = Ones; Dice = dice }) |> ignore

            let result = scorecard.Score({ Category = ThreeOfAKind; Dice = dice })

            Assert.True(match result with
                        | ScoreResult.Success(_) -> true
                        | ScoreResult.Failure(_) -> false)
            Assert.Equal((true, 5), scorecard.LowerSection.TryGetScore(ThreeOfAKind))
            
        [<Fact>]
        let ``Score Lower Section With Yahtzee Fails If Yahtzee Is Already Scored And Category Matches Dice And Dice Category Is Already Scored And Lower Section Category Is Already Scored``() =
            let scorecard = new Scorecard()
            let dice = [Die.One; Die.One; Die.One; Die.One; Die.One] 
            scorecard.Score({ Category = Yahtzee; Dice = dice }) |> ignore
            scorecard.Score({ Category = Ones; Dice = dice }) |> ignore
            scorecard.Score({ Category = ThreeOfAKind; Dice = dice }) |> ignore

            let result = scorecard.Score({ Category = ThreeOfAKind; Dice = dice })

            Assert.False(match result with
                        | ScoreResult.Success(_) -> true
                        | ScoreResult.Failure(_) -> false)

        [<Fact>]
        let ``Any Lower Section Category Can Be Scored By A Yahtzee Joker If The Yahtzee Category And The Upper Section Category Have Been Scored``() =
            let scorecard = new Scorecard()
            let dice = [Die.One; Die.One; Die.One; Die.One; Die.One] 
            scorecard.Score({ Category = Yahtzee; Dice = dice }) |> ignore
            scorecard.Score({ Category = Ones; Dice = dice }) |> ignore
            scorecard.Score({ Category = ThreeOfAKind; Dice = dice }) |> ignore

            let result = scorecard.Score({ Category = FullHouse; Dice = dice })

            Assert.True(match result with
                        | ScoreResult.Success(_) -> true
                        | ScoreResult.Failure(_) -> false)
            Assert.Equal((true, int Scoring.LowerSectionScore.FullHouse), scorecard.LowerSection.TryGetScore(FullHouse))

        [<Fact>]
        let ``Second Yahtzee Wins A Bonus Chip If The First Yahtzee Was Scored In The Yahtzee Box``() =
            let scorecard = new Scorecard()
            scorecard.Score({ Category = Yahtzee; Dice = [Die.One; Die.One; Die.One; Die.One; Die.One] }) |> ignore
            scorecard.Score({ Category = Ones; Dice = [Die.One; Die.One; Die.One; Die.One; Die.One] }) |> ignore

            Assert.Equal(1, scorecard.BonusChips)

        [<Fact>]
        let ``Second Yahtzee Does Not Win A Bonus Chip If The First Yahtzee Was Not Scored In The Yahtzee Box``() =
            let scorecard = new Scorecard()
            scorecard.Score({ Category = Ones; Dice = [Die.One; Die.One; Die.One; Die.One; Die.One] }) |> ignore
            scorecard.Score({ Category = Yahtzee; Dice = [Die.One; Die.One; Die.One; Die.One; Die.One] }) |> ignore

            Assert.Equal(0, scorecard.BonusChips)