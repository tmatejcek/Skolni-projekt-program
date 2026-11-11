using System.Text;
public class StorageException : Exception
{
    public StorageException(int line, string reason)
        : base($"Line {line}: {reason}") { }
}

public class Storage
{
    const string Header = "ULOZENI-TURNAJE|1";
    readonly string path;

    public Storage(string path)
    {
        this.path = path;
    }

    // ---------- SAVE ----------

    public void Save(SaveData data)
    {
        var lines = new List<string>();

        // Example: players are done, do the rest the same way.
        foreach (var p in data.Players)
            lines.Add($"HRAC|{p.Nickname}|{p.Name}");
        
        if (data.Tournament != null)
        {
            lines.Add($"TURNAJ|{data.Tournament.Name}|{data.Tournament.Discipline}|{data.Tournament.RoundCount}");
        }

        foreach (var nickname in data.RegisteredPlayers)
            lines.Add($"PRIHLASKA|{nickname}");

        foreach (var p in data.Rounds)
        {
            string state;
            if (p.IsClosed == true)
                state = "UZAVRENE";
            else
                state = "OTEVRENE";
            lines.Add($"KOLO|{p.Number}|{state}");
        }

        foreach (var p in data.Matches)
            lines.Add($"ZAPAS|{p.Round}|{p.Number}|{p.Player1}|{p.Player2}|{p.Result}");

        foreach (var p in data.Byes)
            lines.Add($"VOLNY|{p.Round}|{p.Nickname}");
            
        

        int count = lines.Count;            // number of records, before adding header/footer
        lines.Insert(0, Header);
        lines.Add($"KONEC|{count}");

        // TODO later (step 9): write to path + ".tmp" first, then File.Move(..., overwrite: true)
        File.WriteAllLines(path, lines, Encoding.UTF8);
    }

    // ---------- LOAD ----------

    public SaveData Load()
    {
        var data = new SaveData();

        if (!File.Exists(path))
            return data;                    // no file = start empty, not an error

        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        // TODO (step 11): check that the first line == Header
        // TODO (step 11): check the last line is KONEC|count and the count matches

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            int lineNumber = i + 1;

            if (line == "" || line.StartsWith("#") || line == Header || line.StartsWith("KONEC|"))
                continue;

            string[] f = line.Split('|');

            switch (f[0])
            {
                // Example: players are done, do the rest the same way.
                case "HRAC":
                    if (f.Length != 3) throw new StorageException(lineNumber, "HRAC needs 3 fields");
                    data.Players.Add(new PlayerRecord(f[1], f[2]));
                    break;

                case "TURNAJ":
                    if (f.Length != 4) throw new StorageException(lineNumber, "TURNAJ needs 4 fields");
                    if (!int.TryParse(f[3], out int roundCount))
                        throw new StorageException(lineNumber, "round count must be a number");
                    data.Tournament = new TournamentRecord(f[1], f[2], roundCount);
                    break;
                case "PRIHLASKA":
                    if (f.Length != 2) throw new StorageException(lineNumber, "PRIHLASKA needs 2 fields");
                    data.RegisteredPlayers.Add(f[1]);
                    break;
                case "KOLO":
                    if (f.Length != 3) throw new StorageException(lineNumber, "KOLO needs 3 fields");
                    bool isClosed;
                    if (f[2] == "UZAVRENE")
                        isClosed = true;
                    else if (f[2] == "OTEVRENE")
                        isClosed = false;
                    else
                        throw new StorageException(lineNumber, "round state must be UZAVRENE or OTEVRENE");
                    if (!int.TryParse(f[1], out int roundNumber))
                        throw new StorageException(lineNumber, "roundNumber must be a number");
                    data.Rounds.Add(new RoundRecord(roundNumber, isClosed));
                    break;
                    
                case "ZAPAS":
                    if (f.Length != 6) throw new StorageException(lineNumber, "ZAPAS needs 6 fields");
                    if (!int.TryParse(f[1], out int numberRound))
                        throw new StorageException(lineNumber, "numberRound must be a number");
                    if (!int.TryParse(f[2], out int numberNumber))
                        throw new StorageException(lineNumber, "numberNumber must be a number");
                    if (f[5] != "VYHRA1" && f[5] != "VYHRA2" && f[5] != "REMIZA" && f[5] != "-")
                        throw new StorageException(lineNumber, "invalid match result");
                    data.Matches.Add(new MatchRecord(numberRound, numberNumber, f[3], f[4], f[5]));
                    break;
                    
                case "VOLNY":
                    if (f.Length != 3) throw new StorageException(lineNumber, "VOLNY needs 3 fields");
                    if (!int.TryParse(f[1], out int round))
                        throw new StorageException(lineNumber, "round must be a number");
                    data.Byes.Add(new ByeRecord(round, f[2]));
                    break;

                default:
                    throw new StorageException(lineNumber, $"unknown record type '{f[0]}'");
            }
        }

        return data;
    }
}