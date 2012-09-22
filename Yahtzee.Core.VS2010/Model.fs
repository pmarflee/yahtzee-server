namespace Yahtzee.Core

type ScorecardSectionType = | Upper | Lower

type Category =
    | Ones
    | Twos
    | Threes
    | Fours
    | Fives
    | Sixes
    | ThreeOfAKind
    | FourOfAKind
    | FullHouse
    | SmallStraight
    | LargeStraight
    | Yahtzee
    | Chance
with
    member this.Section = 
        match this with
        | Ones | Twos | Threes | Fours | Fives | Sixes -> Upper
        | ThreeOfAKind | FourOfAKind | FullHouse | SmallStraight | LargeStraight | Yahtzee | Chance -> Lower

type Die =
    | One = 1
    | Two = 2
    | Three = 3
    | Four = 4
    | Five = 5
    | Six = 6

type Turn = { Dice : Die list; Category : Category }