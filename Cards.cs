public class Card {
    public int Id { get; set; } // Counterspell: 22
    public string Name { get; set; } // Counterspell
    public string Type { get; set; } // Instant
    public string[] CreatureTypes { get; set; } // Human, Rogue
    public int Intelligence { get; set; } // 23
    public int Strength { get; set; } // 13
    public int Dexterity { get; set; } // 45
}

public class CardDb {
    public Dictionary<int, Card> Cards { get; set; } 
}

public class Deck {
    public List<Card> Cards { get; set; } 
}

public class PlayerFieldData {
    public int Id { get; set; }
    public List<Card> Cards { get; set; } 

}

public class PlayField {
    public PlayerFieldData Opponent { get; set; }
    public PlayerFieldData Current { get; set; }
}