// One record = one line in the save file.

public record PlayerRecord(string Nickname, string Name);                  // HRAC|nickname|name
public record TournamentRecord(string Name, string Discipline, int RoundCount); // TURNAJ|name|discipline|rounds
public record RoundRecord(int Number, bool IsClosed);                      // KOLO|number|OTEVRENE or UZAVRENE
public record MatchRecord(int Round, int Number, string Player1, string Player2, string Result); // ZAPAS|...
public record ByeRecord(int Round, string Nickname);                       // VOLNY|round|nickname

// Result is one of: "VYHRA1", "VYHRA2", "REMIZA", "-"

public class SaveData
{
    public List<PlayerRecord> Players = new();
    public TournamentRecord? Tournament;          // null = no tournament yet
    public List<string> RegisteredPlayers = new(); // PRIHLASKA|nickname
    public List<RoundRecord> Rounds = new();
    public List<MatchRecord> Matches = new();
    public List<ByeRecord> Byes = new();
}