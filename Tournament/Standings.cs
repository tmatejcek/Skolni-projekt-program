public sealed class StandingRow
{
    public StandingRow(string nickname)
    {
        Nickname = nickname;
    }

    public int Position { get; set; }
    public string Nickname { get; }
    public int PlayedMatches { get; set; }
    public int Points { get; set; }
}

public static class Standings
{
    public static List<StandingRow> Calculate(SaveData data)
    {
        List<StandingRow> rows = new List<StandingRow>();

        foreach (string nickname in data.RegisteredPlayers)
        {
            rows.Add(new StandingRow(nickname));
        }

        foreach (MatchRecord match in data.Matches)
        {
            int player1Index = FindPlayer(rows, match.Player1);
            int player2Index = FindPlayer(rows, match.Player2);

            if (match.Result == "VYHRA1")
            {
                AddPlayedMatch(rows, player1Index);
                AddPlayedMatch(rows, player2Index);
                AddPoints(rows, player1Index, 3);
            }
            else if (match.Result == "VYHRA2")
            {
                AddPlayedMatch(rows, player1Index);
                AddPlayedMatch(rows, player2Index);
                AddPoints(rows, player2Index, 3);
            }
            else if (match.Result == "REMIZA")
            {
                AddPlayedMatch(rows, player1Index);
                AddPlayedMatch(rows, player2Index);
                AddPoints(rows, player1Index, 1);
                AddPoints(rows, player2Index, 1);
            }
        }

        foreach (ByeRecord bye in data.Byes)
        {
            int playerIndex = FindPlayer(rows, bye.Nickname);
            AddPoints(rows, playerIndex, 3);
        }

        for (int i = 0; i < rows.Count - 1; i++)
        {
            for (int j = i + 1; j < rows.Count; j++)
            {
                bool shouldSwap = rows[j].Points > rows[i].Points;
                if (rows[j].Points == rows[i].Points &&
                    string.Compare(rows[j].Nickname, rows[i].Nickname, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    shouldSwap = true;
                }

                if (shouldSwap)
                {
                    StandingRow temporary = rows[i];
                    rows[i] = rows[j];
                    rows[j] = temporary;
                }
            }
        }

        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].Position = i + 1;
        }

        return rows;
    }

    private static int FindPlayer(List<StandingRow> rows, string nickname)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static void AddPlayedMatch(List<StandingRow> rows, int playerIndex)
    {
        if (playerIndex != -1)
        {
            rows[playerIndex].PlayedMatches++;
        }
    }

    private static void AddPoints(List<StandingRow> rows, int playerIndex, int amount)
    {
        if (playerIndex != -1)
        {
            rows[playerIndex].Points += amount;
        }
    }
}
