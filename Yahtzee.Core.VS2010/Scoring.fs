namespace Yahtzee.Core

module Scoring =

    let yahtzeeScore = 50
    let fullHouseScore = 25
    let smallStraightScore = 30
    let largeStraightScore = 40
    let yahtzeeBonus = 100

    module private ScoringUtils =

        let nOfAKind (dice : Die list) n =
            dice 
            |> Seq.groupBy (fun die -> die)
            |> Seq.filter (fun (_, dice) -> dice |> Seq.length >= n)
            |> Seq.tryPick (fun (value, dice) -> Some(value, Seq.length dice))

        let nOfAKindScorer dice n =
            match nOfAKind dice n with
            | Some((value, count)) -> (int value) * count
            | None -> 0

        let threeOfAKindScorer dice = nOfAKindScorer dice 3

        let fourOfAKindScorer dice = nOfAKindScorer dice 4

        let yahtzeeScorer dice = if (nOfAKind dice 5).IsSome then yahtzeeScore else 0

        let fullHouseScorer dice =
            match nOfAKind dice 3 with
            | None -> 0
            | Some(threeOfAKind) ->
                match nOfAKind (List.filter (fun die -> die <> fst threeOfAKind) dice) 2 with
                | None -> 0
                | Some(twoOfAKind) ->
                    if fst twoOfAKind = fst threeOfAKind then
                        0
                    else
                        fullHouseScore

        let straightScorer (dice : Die list) options score =
            let distinctAndSorted = dice |> Seq.distinct |> Seq.sort |> Seq.toList
            if options |> List.exists (fun option -> option = distinctAndSorted)
            then score else 0

        let smallStraightScorerOptions = [
            [ Die.One; Die.Two; Die.Three; Die.Four ];
            [ Die.Two; Die.Three; Die.Four; Die.Five ];
            [ Die.Three; Die.Four; Die.Five; Die.Six ]
        ]
        let smallStraightScorer dice = straightScorer dice smallStraightScorerOptions smallStraightScore

        let largeStraightScorerOptions = [
            [ Die.One; Die.Two; Die.Three; Die.Four; Die.Five ];
            [ Die.Two; Die.Three; Die.Four; Die.Five; Die.Six ];
        ]
        let largeStraightScorer dice = straightScorer dice largeStraightScorerOptions largeStraightScore

        let chanceScorer dice = List.sumBy (fun die -> int die) dice

        let upperCategoryScorer value (dice : Die list) =
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

    let NOfAKind dice = ScoringUtils.nOfAKindScorer dice 5
    
    let ScoreJoker turn =
        match turn.Category with
        | Ones | Twos | Threes | Fours | Fives | Sixes | ThreeOfAKind | FourOfAKind | Chance -> NOfAKind turn.Dice
        | FullHouse -> fullHouseScore
        | SmallStraight -> smallStraightScore
        | LargeStraight -> largeStraightScore
        | _ -> 0