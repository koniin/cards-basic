
// var cm = new CardsMaker();
// cm.MakeCreatures(".\\creatureCards.json");

Random rng = new Random();

var cardDb = new CardDatabase();
cardDb.Load(".\\creatureCards.json");


List<Card> computerDeck = new List<Card>();
List<Card> humanDeck = new List<Card>();

int deckCount = 15;

for (int i = 0; i < deckCount * 2; i++)
{
    if (i < deckCount)
    {
        computerDeck.Add(cardDb.GetCard(1000 + i));
    }
    else
    {
        humanDeck.Add(cardDb.GetCard(1000 + i));
    }
}

ShuffleFisherYates(computerDeck, rng);
ShuffleFisherYates(humanDeck, rng);

var computerHand = new List<Card>();
var humanHand = new List<Card>();

DrawCard(computerDeck, 4, computerHand);
DrawCard(humanDeck, 4, humanHand);

bool gameOver = false;

string[] stats = new string[] {
    "str", "int", "dex"
};

int computerHealth = 4;
int humanHealth = 4;

Card actor1PlayedCard = null;
Card actor2PlayedCard = null;

var actor1TakeTurn = ComputerActorTurn;
var actor2TakeTurn = HumanActorTurn;

string state = "round_start";
string stat = "";
while (!gameOver)
{

    // ANIMATIONS CAN BE HANDLED BY HAVING ANOTHER STATE 
    // AND AN ONCOMPLETE => set next state

    switch (state)
    {
        case "round_start":
            {
                // START TURN
                stat = stats[rng.Next(stats.Length)];
                state = "turn_computer";
                break;
            }
        case "turn_computer":
            {// Computer takes turn depending on stat
                actor1PlayedCard = actor1TakeTurn(stat);
                state = "turn_player_start";
                break;
            }
        case "turn_player_start":
            {
                Console.WriteLine($"This turns stat: {stat}");
                Console.WriteLine($"Human HP: {humanHealth}");
                Console.WriteLine($"Computer HP: {computerHealth}");
                // display hand
                Console.WriteLine("---------------- Your hand ----------------");
                foreach (var c in humanHand)
                {
                    Console.WriteLine(GetCardDisplay(c));
                }

                state = "turn_player_input";
                break;
            }
        case "turn_player_input":
            {
                actor2PlayedCard = actor2TakeTurn(stat);
                if (actor2PlayedCard != null)
                {
                    state = "resolve";
                }
                break;
            }
        case "resolve":
            {
                var (winner, reason) = GetResult(stat, actor1PlayedCard, actor2PlayedCard);

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(GetCardDisplay(actor1PlayedCard));
                Console.WriteLine(GetCardDisplay(actor2PlayedCard));
                if (winner == 0)
                {
                    Console.WriteLine("DRAW!");
                }
                else if (winner == 1)
                {
                    Console.WriteLine("Computer wins turn");
                    humanHealth--;
                }
                else if (winner == 2)
                {
                    Console.WriteLine("Player won turn");
                    computerHealth--;
                }
                else
                {
                    Console.WriteLine($"ERROR: {reason}");
                }

                state = "round_start";

                Console.WriteLine(Environment.NewLine);

                if (humanHealth <= 0)
                {
                    Console.WriteLine("Computer WINS!");
                    gameOver = true;
                }
                else if (computerHealth <= 0)
                {
                    Console.WriteLine("You WIN!");
                    gameOver = true;
                }
                break;
            }
    }
}

Card ComputerActorTurn(string stat)
{
    var id = ComputerTurn(stat, computerHand, rng);
    var (couldPlayCard, computerPlayedCard) = PlayCard(id, computerHand);
    return computerPlayedCard;
}

Card HumanActorTurn(string stat)
{
    Card c = null;
    if (Console.KeyAvailable)
    {
        ConsoleKeyInfo key = Console.ReadKey(true);
        switch (key.Key)
        {
            case ConsoleKey.F1:
                Console.WriteLine("You pressed F1!");
                break;
            case ConsoleKey.Q:
                gameOver = true;
                break;
            case ConsoleKey.Spacebar:
                if (actor2PlayedCard != null)
                {
                    Console.WriteLine("End Turn");
                    state = "resolve";
                }
                else
                {
                    Console.WriteLine("Can't end turn");
                }
                break;
            case ConsoleKey.P:
                {
                    Console.WriteLine("Blocking until we have gotten an id of card to play");
                    var idString = Console.ReadLine();
                    var id = int.Parse(idString);
                    var (foundCard, ccc) = PlayCard(id, humanHand);
                    if (foundCard)
                    {
                        c = ccc;
                    }
                    break;
                }
            default:
                break;
        }
    }

    return c;
}

int ComputerTurn(string stat, List<Card> cards, Random rng)
{
    // 40% chance to make a random move
    if (rng.Next(0, 100) < 40)
    {
        return cards[rng.Next(cards.Count)].Id;
    }

    int idHighest = 0;
    int valHighest = 0;
    for (int i = 0; i < cards.Count; i++)
    {
        if (GetStatValue(stat, cards[i]) > valHighest)
        {
            idHighest = cards[i].Id;
        }
    }
    return idHighest;
}

int GetStatValue(string stat, Card card)
{
    switch (stat)
    {
        case "str":
            return card.Strength;
        case "int":
            return card.Intelligence;
        case "dex":
            return card.Dexterity;
        default:
            return -1;
    }
}

(int, string) GetResult(string statToCompare, Card first, Card second)
{
    int result = 0;
    string resultText = string.Empty;
    var firstValue = GetStatValue(statToCompare, first);
    var secondValue = GetStatValue(statToCompare, second);
    if (firstValue != secondValue)
    {
        result = firstValue > secondValue ? 1 : 2;
    }
    return (result, string.Empty);
}

string GetCardDisplay(Card c)
{
    string type = "";
    foreach (var cType in c.CreatureTypes)
    {
        type += cType + ", ";
    }
    type = type.Remove(type.LastIndexOf(','));

    return $"{c.Id}, {c.Name.PadRight(20)} {type.PadRight(20)}\t str: {c.Strength} int: {c.Intelligence} dex: {c.Dexterity}";
}

(bool, Card) PlayCard(int id, List<Card> cards)
{
    for (int i = 0; i < cards.Count; i++)
    {
        if (cards[i].Id == id)
        {
            var card = cards[i];
            cards.RemoveAt(i);
            return (true, card);
        }
    }

    return (false, new Card());
}

void DrawCard(List<Card> deck, int count, List<Card> result)
{
    if (count > deck.Count)
    {
        return;
    }

    for (int i = deck.Count - 1, c = 0; c < count; i--, c++)
    {
        result.Add(deck[i]);
        deck.RemoveAt(i);
    }
}


void ShuffleFisherYates<T>(IList<T> list, Random rng)
{
    int n = list.Count;
    while (n > 1)
    {
        n--;
        int k = rng.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
    }
}