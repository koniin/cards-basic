public class CardDatabase {

    private List<Card> _cards = new List<Card>();

    public void Load(string fileName) {
        var text = File.ReadAllText(fileName);
        _cards = System.Text.Json.JsonSerializer.Deserialize<List<Card>>(text);
    }

    public Card GetCard(int id)
    {
        foreach(var c in _cards) {
            if(c.Id == id) {
                return c;
            }
        }

        throw new Exception("card missing!?");
    }
}