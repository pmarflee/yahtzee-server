namespace Yahtzee.Core

open System.Collections.Generic

[<RequireQualifiedAccess>] 
module Enum = 
  let unexpected<'a, 'b, 'c when 'a : enum<'b>> (value:'a) : 'c = //' 
    failwithf "Unexpected enum member: %A: %A" typeof<'a> value //' 

type ScorecardSection(sectionType) =
    let scores = new Dictionary<Category, int>()
    let categories =
        match sectionType with
        | Upper -> [Ones; Twos; Threes; Fours; Fives; Sixes]
        | Lower -> [ThreeOfAKind; FourOfAKind; FullHouse; SmallStraight; LargeStraight; Yahtzee; Chance]
        |> Set.ofList
    let (|ScoreSuccess|ScoreFailure|) (category, score) =
        if not <| Set.contains category categories then
            ScoreFailure("Category cannot be scored in this section")
        elif score < 0 then
            ScoreFailure("Score must be greater than or equal to zero")
        elif fst <| scores.TryGetValue category then
            ScoreFailure("Category has already been scored")
        else
            ScoreSuccess

    member this.Categories = categories
    member this.Scores = scores :> IReadOnlyDictionary<Category, int>
    member this.Type = sectionType
    member this.Score = Seq.sum scores.Values
    member this.TryGetScore(category) = scores.TryGetValue category
    member this.IsFull = categories |> Set.forall (fun cat -> scores.ContainsKey(cat))        
    member this.RecordScore(category, score) = 
        match category, score with
        | ScoreFailure message -> failwith message
        | ScoreSuccess -> scores.Add(category, score)

type ScoreJokerResult =
    | Success of int * bool
    | Failure of string

type ScoreResult =
    | Success of int
    | Failure of string

type Scorecard() =
    let upperSection = new ScorecardSection(Upper)
    let lowerSection = new ScorecardSection(Lower)
    let mutable bonusChips = 0
    let mutable yahtzees = 0
    let dieCategoryMap =
        [ Die.One, Category.Ones;
        Die.Two, Category.Twos;
        Die.Three, Category.Threes;
        Die.Four, Category.Fours;
        Die.Five, Category.Fives;
        Die.Six, Category.Sixes ]
        |> Map.ofList
    let hasScoredYahtzee = fun () -> lowerSection.Scores.ContainsKey Category.Yahtzee
    let scoreJoker turn =
        if turn.Category = Category.Yahtzee then
            ScoreJokerResult.Failure("Yahtzee combination has already been scored")
        else
            let (_, pointsScoredForYahtzee) = lowerSection.TryGetScore Category.Yahtzee
            let yahtzeePoints = int Scoring.LowerSectionScore.Yahtzee 
            let eligibleForBonus = pointsScoredForYahtzee = yahtzeePoints
            let upperCategoryType = Map.find (List.head turn.Dice) dieCategoryMap
            let upperCategoryHasScore = upperSection.Scores.ContainsKey upperCategoryType
            if not upperCategoryHasScore then
                if turn.Category = upperCategoryType then
                    ScoreJokerResult.Success(Scoring.ScoreJoker turn, eligibleForBonus)
                else
                    ScoreJokerResult.Failure(sprintf "Yahtzee must be scored in category %A" upperCategoryType)
            else 
                if lowerSection.Scores.ContainsKey turn.Category then
                    ScoreJokerResult.Failure(sprintf "Category %A is already scored" turn.Category)
                elif turn.Category.Section = ScorecardSectionType.Upper && not lowerSection.IsFull then
                    ScoreJokerResult.Failure("Combination must be scored in the lower section if it is not full")
                else
                    ScoreJokerResult.Success(Scoring.ScoreJoker turn, eligibleForBonus)

    member this.UpperSection = upperSection
    member this.LowerSection = lowerSection
    member this.BonusChips = bonusChips
    member this.Yahtzees = yahtzees
    member this.Total = upperSection.Score + lowerSection.Score
    member this.UpperBonus = if upperSection.Score >= 63 then 35 else 0
    member this.IsFull = this.UpperSection.IsFull && this.LowerSection.IsFull
    member this.Score(turn) =
        let section = 
            match turn.Category.Section with
            | Upper -> this.UpperSection
            | Lower -> this.LowerSection
        let success category score eligibleForBonus =
            section.RecordScore(category, score)
            ScoreResult.Success(score + if eligibleForBonus then Scoring.YahtzeeBonus else 0)
        if Scoring.IsYahtzee turn.Dice && hasScoredYahtzee() then
            match scoreJoker turn with
            | ScoreJokerResult.Failure(message) -> ScoreResult.Failure(message)
            | ScoreJokerResult.Success(score, eligibleForBonus) ->
                if eligibleForBonus then
                    bonusChips <- bonusChips + 1
                yahtzees <- yahtzees + 1
                success turn.Category score eligibleForBonus
        else 
            if section.Scores.ContainsKey turn.Category then
                ScoreResult.Failure(sprintf "Category %A is already scored" turn.Category)
            else
                success turn.Category (Scoring.Score turn) false
    

