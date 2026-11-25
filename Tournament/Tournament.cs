public sealed class Tournament
{
    public Tournament(SaveData data)
    {
        Data = data;
    }

    public SaveData Data { get; }

    public void AddPlayer(string nickname, string name)
    {
        EnsureValidText(nickname, "Přezdívka");
        EnsureValidText(name, "Jméno");

        foreach (PlayerRecord player in Data.Players)
        {
            if (SameNickname(player.Nickname, nickname))
            {
                throw new TournamentException("Hráč s touto přezdívkou už existuje.");
            }
        }

        Data.Players.Add(new PlayerRecord(nickname, name));
    }

    public void RemovePlayer(string nickname)
    {
        int playerIndex = FindPlayer(nickname);
        if (playerIndex == -1)
        {
            throw new TournamentException($"Hráč {nickname} není v evidenci.");
        }

        string savedNickname = Data.Players[playerIndex].Nickname;
        foreach (MatchRecord match in Data.Matches)
        {
            if (match.Result != "-" && MatchContains(match, savedNickname))
            {
                throw new TournamentException("Hráče nelze odebrat, už odehrál zápas.");
            }
        }

        foreach (ByeRecord bye in Data.Byes)
        {
            if (SameNickname(bye.Nickname, savedNickname))
            {
                throw new TournamentException("Hráče nelze odebrat, už dostal volný los.");
            }
        }

        for (int i = Data.Matches.Count - 1; i >= 0; i--)
        {
            if (MatchContains(Data.Matches[i], savedNickname))
            {
                Data.Matches.RemoveAt(i);
            }
        }

        for (int i = Data.RegisteredPlayers.Count - 1; i >= 0; i--)
        {
            if (SameNickname(Data.RegisteredPlayers[i], savedNickname))
            {
                Data.RegisteredPlayers.RemoveAt(i);
            }
        }

        Data.Players.RemoveAt(playerIndex);
    }

    public void CreateTournament(string name, string discipline, int roundCount)
    {
        EnsureValidText(name, "Název turnaje");
        EnsureValidText(discipline, "Disciplína");

        if (roundCount < 1)
        {
            throw new TournamentException("Počet kol musí být kladné číslo.");
        }

        if (Data.Tournament != null)
        {
            throw new TournamentException("Turnaj už je založen.");
        }

        if (Data.Rounds.Count > 0)
        {
            throw new TournamentException("Turnaj nelze založit, protože už existují kola.");
        }

        Data.Tournament = new TournamentRecord(name, discipline, roundCount);
    }

    public void RegisterPlayer(string nickname)
    {
        EnsureTournamentExists();

        if (Data.Rounds.Count > 0)
        {
            throw new TournamentException("Hráče lze přihlásit jen před začátkem prvního kola.");
        }

        int playerIndex = FindPlayer(nickname);
        if (playerIndex == -1)
        {
            throw new TournamentException("Hráč s touto přezdívkou není v evidenci.");
        }

        string savedNickname = Data.Players[playerIndex].Nickname;
        if (IsRegistered(savedNickname))
        {
            throw new TournamentException("Hráč už je do turnaje přihlášen.");
        }

        Data.RegisteredPlayers.Add(savedNickname);
    }

    public void UnregisterPlayer(string nickname)
    {
        EnsureTournamentExists();

        if (Data.Rounds.Count > 0)
        {
            throw new TournamentException("Hráče lze odhlásit jen před začátkem prvního kola.");
        }

        int playerIndex = FindRegisteredPlayer(nickname);
        if (playerIndex == -1)
        {
            throw new TournamentException("Hráč není do turnaje přihlášen.");
        }

        Data.RegisteredPlayers.RemoveAt(playerIndex);
    }

    public void CreateRound()
    {
        if (Data.Tournament == null)
        {
            throw new TournamentException("Nejprve založte turnaj.");
        }

        if (Data.RegisteredPlayers.Count < 2)
        {
            throw new TournamentException("Před vytvořením kola přihlaste alespoň dva hráče.");
        }

        foreach (RoundRecord round in Data.Rounds)
        {
            if (!round.IsClosed)
            {
                throw new TournamentException("Nejprve musí být uzavřeno otevřené kolo.");
            }
        }

        int number = 1;
        foreach (RoundRecord round in Data.Rounds)
        {
            if (round.Number >= number)
            {
                number = round.Number + 1;
            }
        }

        if (number > Data.Tournament.RoundCount)
        {
            throw new TournamentException($"Počet kol turnaje je {Data.Tournament.RoundCount}.");
        }

        Data.Rounds.Add(new RoundRecord(number, false));
    }

    public void AddMatchPair(string player1, string player2)
    {
        RoundRecord round = GetOpenRound();
        if (SameNickname(player1, player2))
        {
            throw new TournamentException("Hráč nemůže hrát sám proti sobě.");
        }

        if (!IsRegistered(player1) || !IsRegistered(player2))
        {
            throw new TournamentException("Oba hráči musí být přihlášeni do turnaje.");
        }

        foreach (MatchRecord match in Data.Matches)
        {
            if (match.Round == round.Number &&
                (MatchContains(match, player1) || MatchContains(match, player2)))
            {
                throw new TournamentException("Hráč už je v tomto kole zařazen.");
            }
        }

        foreach (ByeRecord bye in Data.Byes)
        {
            if (bye.Round == round.Number &&
                (SameNickname(bye.Nickname, player1) || SameNickname(bye.Nickname, player2)))
            {
                throw new TournamentException("Hráč už má v tomto kole volný los.");
            }
        }

        int number = 1;
        foreach (MatchRecord match in Data.Matches)
        {
            if (match.Round == round.Number && match.Number >= number)
            {
                number = match.Number + 1;
            }
        }

        Data.Matches.Add(new MatchRecord(
            round.Number,
            number,
            FindRegisteredNickname(player1),
            FindRegisteredNickname(player2),
            "-"));
    }

    public void AssignBye(string nickname)
    {
        RoundRecord round = GetOpenRound();
        if (Data.RegisteredPlayers.Count % 2 == 0)
        {
            throw new TournamentException(
                "Volný los lze přidělit jen při lichém počtu přihlášených hráčů.");
        }

        foreach (ByeRecord bye in Data.Byes)
        {
            if (bye.Round == round.Number)
            {
                throw new TournamentException("Toto kolo už má přidělený volný los.");
            }
        }

        if (!IsRegistered(nickname))
        {
            throw new TournamentException("Hráč není přihlášen do turnaje.");
        }

        string savedNickname = FindRegisteredNickname(nickname);
        List<string> candidates = GetByeCandidates(round.Number);
        if (!ContainsNickname(candidates, savedNickname))
        {
            throw new TournamentException("Hráč už je v tomto kole zařazen.");
        }

        int minimumByeCount = int.MaxValue;
        foreach (string candidate in candidates)
        {
            int count = CountPreviousByes(candidate, round.Number);
            if (count < minimumByeCount)
            {
                minimumByeCount = count;
            }
        }

        if (CountPreviousByes(savedNickname, round.Number) > minimumByeCount)
        {
            throw new TournamentException(
                "Volný los dejte nejprve hráči, který ho v minulých kolech dostal méněkrát.");
        }

        Data.Byes.Add(new ByeRecord(round.Number, savedNickname));
    }

    public void SetMatchResult(int roundNumber, int matchNumber, MatchResult result)
    {
        RoundRecord? round = FindRound(roundNumber);
        if (round == null)
        {
            throw new TournamentException("Kolo neexistuje.");
        }

        if (round.IsClosed)
        {
            throw new TournamentException("Kolo je uzavřené, výsledky už nejde měnit.");
        }

        if (result != MatchResult.Player1Win &&
            result != MatchResult.Player2Win &&
            result != MatchResult.Draw)
        {
            throw new TournamentException("Výsledek zápasu není platný.");
        }

        int matchIndex = -1;
        for (int i = 0; i < Data.Matches.Count; i++)
        {
            if (Data.Matches[i].Round == roundNumber &&
                Data.Matches[i].Number == matchNumber)
            {
                matchIndex = i;
                break;
            }
        }

        if (matchIndex == -1)
        {
            throw new TournamentException("Zápas neexistuje.");
        }

        MatchRecord match = Data.Matches[matchIndex];
        string savedResult = "-";
        if (result == MatchResult.Player1Win)
        {
            savedResult = "VYHRA1";
        }
        else if (result == MatchResult.Player2Win)
        {
            savedResult = "VYHRA2";
        }
        else if (result == MatchResult.Draw)
        {
            savedResult = "REMIZA";
        }

        Data.Matches[matchIndex] = new MatchRecord(
            match.Round, match.Number, match.Player1, match.Player2, savedResult);
    }

    public void CloseRound(int roundNumber)
    {
        RoundRecord? round = FindRound(roundNumber);
        if (round == null)
        {
            throw new TournamentException("Kolo neexistuje.");
        }

        if (round.IsClosed)
        {
            throw new TournamentException("Kolo je už uzavřené.");
        }

        List<string> assignedPlayers = new List<string>();
        string missingMatches = "";
        foreach (MatchRecord match in Data.Matches)
        {
            if (match.Round == roundNumber)
            {
                if (match.Result == "-")
                {
                    if (missingMatches != "")
                    {
                        missingMatches += ", ";
                    }

                    missingMatches += match.Number;
                }

                AddUniquePlayer(assignedPlayers, match.Player1);
                AddUniquePlayer(assignedPlayers, match.Player2);
            }
        }

        if (missingMatches != "")
        {
            throw new TournamentException(
                $"Kolo nejde uzavřít, chybí výsledky zápasů: {missingMatches}.");
        }

        int byeCount = 0;
        foreach (ByeRecord bye in Data.Byes)
        {
            if (bye.Round == roundNumber)
            {
                byeCount++;
                AddUniquePlayer(assignedPlayers, bye.Nickname);
            }
        }

        if (assignedPlayers.Count != CountAssignedRecords(roundNumber))
        {
            throw new TournamentException("Hráč je v kole zařazen více než jednou.");
        }

        int expectedByeCount = Data.RegisteredPlayers.Count % 2;
        if (byeCount != expectedByeCount)
        {
            throw new TournamentException(
                expectedByeCount == 1
                    ? "Před uzavřením kola přidělte jeden volný los."
                    : "V kole s párným počtem hráčů nesmí být volný los.");
        }

        foreach (string nickname in Data.RegisteredPlayers)
        {
            if (!ContainsNickname(assignedPlayers, nickname))
            {
                throw new TournamentException(
                    $"Hráč {nickname} ještě není zařazen do zápasu ani nemá volný los.");
            }
        }

        int roundIndex = Data.Rounds.IndexOf(round);
        Data.Rounds[roundIndex] = new RoundRecord(round.Number, true);
    }

    private int CountAssignedRecords(int roundNumber)
    {
        int count = 0;
        foreach (MatchRecord match in Data.Matches)
        {
            if (match.Round == roundNumber)
            {
                count += 2;
            }
        }

        foreach (ByeRecord bye in Data.Byes)
        {
            if (bye.Round == roundNumber)
            {
                count++;
            }
        }

        return count;
    }

    private static void AddUniquePlayer(List<string> players, string nickname)
    {
        if (!ContainsNickname(players, nickname))
        {
            players.Add(nickname);
        }
    }

    private List<string> GetByeCandidates(int roundNumber)
    {
        List<string> assignedPlayers = new List<string>();
        foreach (MatchRecord match in Data.Matches)
        {
            if (match.Round == roundNumber)
            {
                AddUniquePlayer(assignedPlayers, match.Player1);
                AddUniquePlayer(assignedPlayers, match.Player2);
            }
        }

        List<string> candidates = new List<string>();
        foreach (string nickname in Data.RegisteredPlayers)
        {
            if (!ContainsNickname(assignedPlayers, nickname))
            {
                candidates.Add(nickname);
            }
        }

        return candidates;
    }

    private int CountPreviousByes(string nickname, int beforeRound)
    {
        int count = 0;
        foreach (ByeRecord bye in Data.Byes)
        {
            if (bye.Round < beforeRound && SameNickname(bye.Nickname, nickname))
            {
                count++;
            }
        }

        return count;
    }

    private RoundRecord GetOpenRound()
    {
        foreach (RoundRecord round in Data.Rounds)
        {
            if (!round.IsClosed)
            {
                return round;
            }
        }

        throw new TournamentException("Neexistuje otevřené kolo.");
    }

    private void EnsureTournamentExists()
    {
        if (Data.Tournament == null)
        {
            throw new TournamentException("Nejprve založte turnaj.");
        }
    }

    private static void EnsureValidText(string text, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Contains("|"))
        {
            throw new TournamentException($"{fieldName} není platné.");
        }
    }

    private bool IsRegistered(string nickname)
    {
        return FindRegisteredPlayer(nickname) != -1;
    }

    private int FindPlayer(string nickname)
    {
        for (int i = 0; i < Data.Players.Count; i++)
        {
            if (SameNickname(Data.Players[i].Nickname, nickname))
            {
                return i;
            }
        }

        return -1;
    }

    private int FindRegisteredPlayer(string nickname)
    {
        for (int i = 0; i < Data.RegisteredPlayers.Count; i++)
        {
            if (SameNickname(Data.RegisteredPlayers[i], nickname))
            {
                return i;
            }
        }

        return -1;
    }

    private string FindRegisteredNickname(string nickname)
    {
        int index = FindRegisteredPlayer(nickname);
        return index == -1 ? nickname : Data.RegisteredPlayers[index];
    }

    private RoundRecord? FindRound(int number)
    {
        foreach (RoundRecord round in Data.Rounds)
        {
            if (round.Number == number)
            {
                return round;
            }
        }

        return null;
    }

    private static bool ContainsNickname(List<string> players, string nickname)
    {
        foreach (string player in players)
        {
            if (SameNickname(player, nickname))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchContains(MatchRecord match, string nickname)
    {
        return SameNickname(match.Player1, nickname) ||
               SameNickname(match.Player2, nickname);
    }

    private static bool SameNickname(string first, string second)
    {
        return first.Equals(second, StringComparison.OrdinalIgnoreCase);
    }
}
