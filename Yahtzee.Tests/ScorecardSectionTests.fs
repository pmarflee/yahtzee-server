namespace Yahtzee.Tests

open System
open Xunit
open Yahtzee.Core

module ScorecardSectionTests =

    module IsFullTests = 

        [<Fact>]
        let ``Should Be False When Upper Section Is Not Full``() =
            let section = new ScorecardSection(Upper)

            Assert.False(section.IsFull)

        [<Fact>]
        let ``Should Be True When Upper Section Is Full``() =
            let section = new ScorecardSection(Upper)
            section.Categories |> Set.iter (fun cat -> section.RecordScore(cat, 0))

            Assert.True(section.IsFull)

        [<Fact>]
        let ``Should Be False When Lower Section Is Not Full``() =
            let section = new ScorecardSection(Lower)

            Assert.False(section.IsFull)

        [<Fact>]
        let ``Should Be True When Lower Section Is Not Full``() =
            let section = new ScorecardSection(Lower)
            section.Categories |> Set.iter (fun cat -> section.RecordScore(cat, 0))

            Assert.True(section.IsFull)

    module ScoreTests = 

        [<Fact>]
        let ``Should Return The Sum Of All Scores On The Scorecard``() =
            let section = new ScorecardSection(Upper)
            section.RecordScore(Ones, 5)
            section.RecordScore(Twos, 10)
            section.RecordScore(Threes, 9)
            section.RecordScore(Fours, 8)
            section.RecordScore(Fives, 15)
            section.RecordScore(Sixes, 18)

            Assert.Equal(65, section.Score)

        [<Fact>]
        let ``Should Return 0 When The Scorecard Is Empty``() =
            let section = new ScorecardSection(Upper)

            Assert.Equal(0, section.Score)

    module RecordScoreTests =

        [<Fact>]
        let ``Should Fail When Category Is Not Valid For Upper Section``() =
            let section = new ScorecardSection(Upper)
            
            Assert.Throws<Exception>(fun () -> section.RecordScore(Yahtzee, 0))

        [<Fact>]
        let ``Should Fail When Score Is Invalid``() =
            let section = new ScorecardSection(Upper)
            
            Assert.Throws<Exception>(fun () -> section.RecordScore(Ones, -1))

        [<Fact>]
        let ``Should Fail When Category Has Already Been Scored``() =
            let section = new ScorecardSection(Upper)
            section.RecordScore(Ones, 0)

            Assert.Throws<Exception>(fun () -> section.RecordScore(Ones, 0))
            

        [<Fact>]
        let ``Should Succeed When Category Has Not Been Scored And Score Is Valid``() =
            let section = new ScorecardSection(Upper)

            Assert.DoesNotThrow(fun () -> section.RecordScore(Ones, 0))
            Assert.Equal((true, 0), section.TryGetScore(Ones))