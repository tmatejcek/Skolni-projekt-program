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

            List<string> f = SplitLine(line);

            switch (f[0])
            {
                // Example: players are done, do the rest the same way.
                case "HRAC":
                    if (f.Count != 3) throw new StorageException(lineNumber, "HRAC needs 3 fields");
                    data.Players.Add(new PlayerRecord(f[1], f[2]));
                    break;

                // TODO: case "TURNAJ":    use int.TryParse for the round count
                // TODO: case "PRIHLASKA":
                // TODO: case "KOLO":
                // TODO: case "ZAPAS":      check the result is VYHRA1 / VYHRA2 / REMIZA / -
                // TODO: case "VOLNY":

                default:
                    throw new StorageException(lineNumber, $"unknown record type '{f[0]}'");
            }
        }

        return data;
    }

    // ---------- HELPERS ----------

    // "A|B" -> "A\|B",  "A\B" -> "A\\B"
    static string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace("|", "\\|");
    }

    // "HRAC|pepa|Josef" -> ["HRAC", "pepa", "Josef"]
    // "TURNAJ|A\|B|chess|3" -> ["TURNAJ", "A|B", "chess", "3"]
    static List<string> SplitLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();

        // TODO: go through the line char by char:
        //   - if the char is '\'  -> add the NEXT char to current as-is and skip it (i++)
        //   - if the char is '|'  -> fields.Add(current.ToString()); current.Clear();
        //   - otherwise           -> current.Append(char)
        // At the end don't forget: fields.Add(current.ToString());

        return fields;
    }
}