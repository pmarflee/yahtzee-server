namespace Yahtzee.Core

module Scoring =

    type LowerSectionScore =
    | FullHouse = 25
    | SmallStraight = 30
    | LargeStraight = 40
    | Yahtzee = 50

    let YahtzeeBonus = 100

    module private ScoringUtils =

        let nOfAKind dice n =
            dice 
            |> Seq.groupBy id
            |> Seq.filter (fun (_, dice) -> dice |> Seq.length >= n)
            |> Seq.tryPick (fun (value, dice) -> Some(value, Seq.length dice))

        let nOfAKindScorer dice n scorer =
            match nOfAKind dice n with
            | Some(_) -> scorer dice
            | None -> 0

        let sumOfDice dice = List.sumBy int dice
        let threeOfAKindScorer dice = nOfAKindScorer dice 3 sumOfDice
        let fourOfAKindScorer dice = nOfAKindScorer dice 4 sumOfDice
        let yahtzeeScorer dice = nOfAKindScorer dice 5 (fun _ -> int LowerSectionScore.Yahtzee)

        let fullHouseScorer dice =
            match dice
              |> Seq.groupBy id
              |> Seq.map (fun (_, seq) -> Seq.length seq)
              |> Seq.sort
              |> Seq.toList with
            | [2; 3] -> int LowerSectionScore.FullHouse
            | _ -> 0

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

        let upperCategoryScorer value dice =
            List.sumBy (fun die -> if int die = value then int die else 0) dice

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

    let IsYahtzee (dice : Die list) = (ScoringUtils.nOfAKind dice 5).IsSome

    let NOfAKind dice = ScoringUtils.nOfAKindScorer dice 5 ScoringUtils.sumOfDice
    
    let ScoreJoker turn =
        match turn.Category with
        | Ones | Twos | Threes | Fours | Fives | Sixes | ThreeOfAKind | FourOfAKind | Chance -> NOfAKind turn.Dice
        | FullHouse -> int LowerSectionScore.FullHouse
        | SmallStraight -> int LowerSectionScore.SmallStraight
        | LargeStraight -> int LowerSectionScore.LargeStraight
        | _ -> 0