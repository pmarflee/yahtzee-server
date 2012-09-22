namespace Yahtzee.Core

open System.Collections.Generic

[<RequireQualifiedAccess>] 
module Enum = 
  let unexpected<'a, 'b, 'c when 'a : enum<'b>> (value:'a) : 'c = //' 
    failwithf "Unexpected enum member: %A: %A" typeof<'a> value //' 

type ScorecardSection = {
    Type : ScorecardSectionType;
    Scores : Dictionary<Category, int>
} with
    member this.Score = this.Scores.Values |> Seq.sum
    member this.TryGetScore(category) = this.Scores.TryGetValue category
    member this.IsFull =
        let categories =
            match this.Type with
            | Upper -> [Ones;Twos;Threes;Fours;Fives;Sixes]
            | Lower -> [ThreeOfAKind;FourOfAKind;FullHouse;SmallStraight;LargeStraight;Yahtzee;Chance]
            |> Set.ofList
        this.Scores.Keys |> Seq.forall (fun key -> Set.contains key categories)

type ScoreYahtzeeResult =
    | Success of int * bool
    | Failure of string

type ScoreResult =
    | Success of int
    | Failure of string

type Scorecard = {
    UpperSection : ScorecardSection;
    LowerSection : ScorecardSection;
    mutable BonusChips : int;
    mutable Yahtzees : int;
} with
    member this.Total = this.UpperSection.Score + this.LowerSection.Score
    member this.UpperBonus = if this.UpperSection.Score >= 63 then 35 else 0

    member private this.ScoreYahtzee(turn) =
        let (hasScoredYahtzee, yahtzeePoints) = this.LowerSection.TryGetScore Category.Yahtzee
        if turn.Category = Category.Yahtzee then
            if hasScoredYahtzee then
                ScoreYahtzeeResult.Failure("Yahtzee combination has already been scored")
            else
                ScoreYahtzeeResult.Success(Scoring.yahtzeeScore, true)
        else
            let upperCategoryType = 
                let die = List.head turn.Dice
                match die with
                    | Die.One -> Category.Ones
                    | Die.Two -> Category.Twos
                    | Die.Three -> Category.Threes
                    | Die.Four -> Category.Fours
                    | Die.Five -> Category.Fives
                    | Die.Six -> Category.Sixes
                    | _ -> Enum.unexpected die
            
            let upperCategoryHasScore = this.UpperSection.Scores.ContainsKey upperCategoryType
            if not upperCategoryHasScore && turn.Category <> upperCategoryType then
                ScoreYahtzeeResult.Failure(sprintf "Yahtzee must be scored in category %A" upperCategoryType)
            else 
                if this.LowerSection.Scores.ContainsKey turn.Category then
                    ScoreYahtzeeResult.Failure(sprintf "Category %A is already scored" turn.Category)
                elif turn.Category.Section = ScorecardSectionType.Upper && not this.LowerSection.IsFull then
                    ScoreYahtzeeResult.Failure("Combination must be scored in the lower section if it is not full")
                else
                    let firstYahtzeeScoredInYahtzeeBox = this.LowerSection.TryGetScore Category.Yahtzee = (true, Scoring.yahtzeeScore)
                    ScoreYahtzeeResult.Success(Scoring.ScoreJoker turn, firstYahtzeeScoredInYahtzeeBox)

    member this.Score(turn) =
        let section = 
            match turn.Category.Section with
            | Upper -> this.UpperSection
            | Lower -> this.LowerSection
        let success category score eligibleForBonus =
            section.Scores.Add(category, score)
            ScoreResult.Success(score + if eligibleForBonus then Scoring.yahtzeeBonus else 0)
        if Scoring.IsYahtzee turn.Dice then
            match this.ScoreYahtzee turn with
            | ScoreYahtzeeResult.Failure(message) -> ScoreResult.Failure(message)
            | ScoreYahtzeeResult.Success(score, eligibleForBonus) ->
                if eligibleForBonus then
                    this.BonusChips <- this.BonusChips + 1
                this.Yahtzees <- this.Yahtzees + 1
                success turn.Category score eligibleForBonus
        else 
            if section.Scores.ContainsKey turn.Category then
                ScoreResult.Failure(sprintf "Category %A is already scored" turn.Category)
            else
                success turn.Category (Scoring.Score turn) false
    

