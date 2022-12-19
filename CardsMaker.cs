public class CardsMaker
{
    Random r = new Random();

    public void MakeCreatures(string fileName)
    {
        var text = File.ReadAllLines("dd_names.txt");

        List<string> names = new List<string>();
        List<string> races = new List<string>();
        List<string> classes = new List<string>();

        foreach (var line in text)
        {
            var name = line.Split(',')[0];

            if (!names.Contains(name))
            {
                names.Add(name.Trim());
            }

            var raceAndClass = line.Split(',')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!races.Contains(raceAndClass[0]))
            {
                races.Add(raceAndClass[0].Trim());
            }
            if (!classes.Contains(raceAndClass[1]))
            {
                classes.Add(raceAndClass[1].Trim());
            }
        }


        foreach (var race in races)
        {
            Console.Write(race + ", ");
        }
        foreach (var c in classes)
        {
            Console.Write(c + ", ");
        }

        // var text = File.ReadAllText("cards.json");

        // var cards = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Card>>(text);


        var newCards = new List<Card>();

        int cardId = 1000;

        for (int i = 0; i < names.Count; i++)
        {
            var cardClass = classes[r.Next(classes.Count)];
            var cardRace = races[r.Next(races.Count)];

            Card cc = new Card();

            cc.Id = cardId++;
            cc.Type = "Creature";
            cc.Name = $"{names[r.Next(names.Count)]}";
            cc.CreatureTypes = new string[] { cardRace, cardClass };

            cc.Dexterity = r.Next(1, 51);
            cc.Intelligence = r.Next(1, 51);
            cc.Strength = r.Next(1, 51);

            newCards.Add(cc);
        }

        var newCardsJson = System.Text.Json.JsonSerializer.Serialize(newCards);

        File.WriteAllText(fileName, newCardsJson);
    }
}