namespace Yahtzee.Core

module Scoring =

    type LowerSectionScore =
    | FullHouse = 25
    | SmallStraight = 30
    | LargeStraight = 40
    | Yahtzee = 50

    let YahtzeeBonus = 100

    module private ScoringUtils =

        let ofAKindScorer dice category scorer =
            match
                match dice
                  |> Seq.groupBy id
                  |> Seq.map (fun (die, seq) -> die, Seq.length seq)
                  |> Seq.sortBy (fun (die, count) -> -count)
                  |> Seq.toList with
                | (_, 3) :: tl when category = ThreeOfAKind -> true
                | (_, 4) :: tl when category = ThreeOfAKind || category = FourOfAKind -> true
                | (_, 5) :: tl when category = Yahtzee -> true
                | (_, 3) :: (_, 2) :: tl when category = FullHouse -> true
                | _ -> false 
                with
            | true -> scorer dice
            | false -> 0

        let sumOfDice dice = List.sumBy int dice
        let scoreYahtzee = (fun _ -> int LowerSectionScore.Yahtzee)
        let threeOfAKindScorer dice = ofAKindScorer dice ThreeOfAKind sumOfDice
        let fourOfAKindScorer dice = ofAKindScorer dice FourOfAKind sumOfDice
        let fullHouseScorer dice = ofAKindScorer dice FullHouse (fun _ -> int LowerSectionScore.FullHouse)
        let yahtzeeScorer dice = ofAKindScorer dice Yahtzee scoreYahtzee

        let straightScorer dice options score =
            let set = dice |> Set.ofList
            if options |> List.exists (fun straight -> Set.isSubset straight set)
            then score else 0

        let dieValues = System.Enum.GetValues(typeof<Die>) :?> Die[]
        let getStraightScorerOptions size = dieValues |> Seq.windowed size |> Seq.map Set.ofArray |> Seq.toList
        let smallStraightScorerOptions = getStraightScorerOptions 4
        let largeStraightScorerOptions = getStraightScorerOptions 5

        let smallStraightScorer dice = straightScorer dice smallStraightScorerOptions (int LowerSectionScore.SmallStraight)
        let largeStraightScorer dice = straightScorer dice largeStraightScorerOptions (int LowerSectionScore.LargeStraight)
        let chanceScorer dice = List.sumBy int dice
        let upperCategoryScorer value dice = List.sumBy (fun die -> if int die = value then int die else 0) dice

        let scorers = 
            [ Ones, upperCategoryScorer 1;
            Twos, upperCategoryScorer 2;
            Threes, upperCategoryScorer 3;
            Fours, upperCategoryScorer 4;
            Fives, upperCategoryScorer 5;
            Sixes, upperCategoryScorer 6;
            ThreeOfAKind, threeOfAKindScorer;
            FourOfAKind, fourOfAKindScorer;
            FullHouse, fullHouseScorer;
            SmallStraight, smallStraightScorer;
            LargeStraight, largeStraightScorer;
            Yahtzee, yahtzeeScorer;
            Chance, chanceScorer ]
            |> Map.ofList

    let Score turn = ScoringUtils.scorers |> Map.find turn.Category <| turn.Dice

    let GetScorer category = Map.find category ScoringUtils.scorers

    let IsYahtzee (dice : Die list) = ScoringUtils.yahtzeeScorer dice = int LowerSectionScore.Yahtzee

    let NOfAKind dice = ScoringUtils.ofAKindScorer dice Yahtzee ScoringUtils.sumOfDice
    
    let ScoreJoker turn =
        match turn.Category with
        | Ones | Twos | Threes | Fours | Fives | Sixes | ThreeOfAKind | FourOfAKind | Chance -> NOfAKind turn.Dice
        | FullHouse -> int LowerSectionScore.FullHouse
        | SmallStraight -> int LowerSectionScore.SmallStraight
        | LargeStraight -> int LowerSectionScore.LargeStraight
        | _ -> 0