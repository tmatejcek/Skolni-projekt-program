public static class Program
{
    private static readonly Storage storage = new Storage("turnaj.txt");

    public static void Main()
    {
        while (true)
        {
            Console.WriteLine(
                "\nCo chcete udělat?\n" +
                "1. Zaregistrovat hráče\n" +
                "2. Odebrat hráče\n" +
                "3. Zobrazit hráče\n" +
                "4. Založit turnaj\n" +
                "5. Přihlásit hráče do turnaje\n" +
                "6. Odhlásit hráče z turnaje\n" +
                "7. Vytvořit nové kolo\n" +
                "8. Určit dvojice kola\n" +
                "9. Přiřadit volný los\n" +
                "10. Zadat nebo opravit výsledek zápasu\n" +
                "11. Uzavřít kolo\n" +
                "12. Zobrazit zápasy\n" +
                "13. Zobrazit tabulku\n" +
                "14. Ukončit program");

            int choice = Input.ReadMenuChoice("Zadejte číslo volby: ", 1, 14);

            switch (choice)
            {
                case 1:
                    AddPlayer();
                    break;
                case 2:
                    RemovePlayer();
                    break;
                case 3:
                    ListPlayers();
                    break;
                case 4:
                    CreateTournament();
                    break;
                case 5:
                    RegisterPlayer();
                    break;
                case 6:
                    UnregisterPlayer();
                    break;
                case 7:
                    CreateRound();
                    break;
                case 8:
                    AddMatchPair();
                    break;
                case 9:
                    AssignBye();
                    break;
                case 10:
                    SetMatchResult();
                    break;
                case 11:
                    CloseRound();
                    break;
                case 12:
                    DisplayMatches();
                    break;
                case 13:
                    DisplayStandings();
                    break;
                case 14:
                    Console.WriteLine("Ukončuji program.");
                    return;
            }
        }
    }

    private static SaveData LoadData()
    {
        SaveData data = storage.Load();
        string? recoveryMessage = storage.TakeRecoveryMessage();
        if (recoveryMessage != null)
        {
            Console.WriteLine(recoveryMessage);
        }

        return data;
    }

    private static void AddPlayer()
    {
        string nickname = Input.ReadText("Přezdívka hráče: ").Trim();
        string name = Input.ReadText("Jméno hráče: ").Trim();

        if (!IsValidText(nickname) || !IsValidText(name))
        {
            Console.WriteLine("Přezdívka nebo jméno hráče není platné.");
            return;
        }

        SaveData data = LoadData();
        foreach (PlayerRecord player in data.Players)
        {
            if (player.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Hráč s touto přezdívkou už existuje.");
                return;
            }
        }

        data.Players.Add(new PlayerRecord(nickname, name));
        storage.Save(data);
        Console.WriteLine("Hráč " + name + " byl přidán.");
    }

    private static void RemovePlayer()
    {
        string nickname = Input.ReadText("Přezdívka hráče: ").Trim();
        if (Input.ReadYesNo("Opravdu chcete odebrat hráče " + nickname + "? (a/n): "))
        {
            SaveData data = LoadData();
            int playerIndex = FindPlayer(data, nickname);

            if (playerIndex == -1)
            {
                Console.WriteLine("Hráč " + nickname + " není v evidenci.");
                return;
            }

            string savedNickname = data.Players[playerIndex].Nickname;
            foreach (MatchRecord match in data.Matches)
            {
                if (match.Result != "-" && MatchContains(match, savedNickname))
                {
                    Console.WriteLine("Hráče nelze odebrat, už odehrál zápas.");
                    return;
                }
            }

            for (int i = data.Matches.Count - 1; i >= 0; i--)
            {
                if (MatchContains(data.Matches[i], savedNickname))
                {
                    data.Matches.RemoveAt(i);
                }
            }

            for (int i = data.Byes.Count - 1; i >= 0; i--)
            {
                if (data.Byes[i].Nickname.Equals(savedNickname, StringComparison.OrdinalIgnoreCase))
                {
                    data.Byes.RemoveAt(i);
                }
            }

            for (int i = data.RegisteredPlayers.Count - 1; i >= 0; i--)
            {
                if (data.RegisteredPlayers[i].Equals(savedNickname, StringComparison.OrdinalIgnoreCase))
                {
                    data.RegisteredPlayers.RemoveAt(i);
                }
            }

            data.Players.RemoveAt(playerIndex);
            storage.Save(data);
            Console.WriteLine("Hráč " + savedNickname + " byl odebrán.");
        }
        else
        {
            Console.WriteLine("Odebrání hráče bylo zrušeno.");
        }
    }

    private static void ListPlayers()
    {
        SaveData data = LoadData();

        if (data.Players.Count == 0)
        {
            Console.WriteLine("Žádní hráči nejsou v evidenci.");
            return;
        }

        Console.WriteLine("Seznam hráčů:");
        foreach (PlayerRecord player in data.Players)
        {
            Console.WriteLine(player.Nickname + " - " + player.Name);
        }
    }

    private static int FindPlayer(SaveData data, string nickname)
    {
        for (int i = 0; i < data.Players.Count; i++)
        {
            if (data.Players[i].Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static bool IsValidText(string text)
    {
        return !string.IsNullOrWhiteSpace(text) && !text.Contains("|");
    }

    private static void CreateTournament()
    {
        SaveData data = LoadData();

        if (data.Tournament != null)
        {
            Console.WriteLine("Turnaj už je založen.");
            return;
        }

        if (data.Rounds.Count > 0)
        {
            Console.WriteLine("Turnaj nelze založit, protože už existují kola.");
            return;
        }

        string name = Input.ReadText("Název turnaje: ").Trim();
        string discipline = Input.ReadText("Disciplína: ").Trim();
        if (!IsValidText(name) || !IsValidText(discipline))
        {
            Console.WriteLine("Název a disciplína nesmí obsahovat znak '|'.");
            return;
        }

        int roundCount = Input.ReadInteger("Počet kol: ", 1, int.MaxValue);
        data.Tournament = new TournamentRecord(name, discipline, roundCount);
        storage.Save(data);
        Console.WriteLine("Turnaj byl založen.");
    }

    private static void RegisterPlayer()
    {
        SaveData data = LoadData();

        if (data.Tournament == null)
        {
            Console.WriteLine("Nejprve založte turnaj.");
            return;
        }

        if (data.Rounds.Count > 0)
        {
            Console.WriteLine("Hráče lze přihlásit jen před začátkem prvního kola.");
            return;
        }

        string nickname = Input.ReadText("Přezdívka hráče: ").Trim();
        PlayerRecord? player = null;
        foreach (PlayerRecord savedPlayer in data.Players)
        {
            if (savedPlayer.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase))
            {
                player = savedPlayer;
                break;
            }
        }

        if (player == null)
        {
            Console.WriteLine("Hráč s touto přezdívkou není v evidenci.");
            return;
        }

        if (IsRegistered(data, player.Nickname))
        {
            Console.WriteLine("Hráč už je do turnaje přihlášen.");
            return;
        }

        data.RegisteredPlayers.Add(player.Nickname);
        storage.Save(data);
        Console.WriteLine("Hráč " + player.Nickname + " byl přihlášen do turnaje.");
    }

    private static void UnregisterPlayer()
    {
        SaveData data = LoadData();

        if (data.Tournament == null)
        {
            Console.WriteLine("Nejprve založte turnaj.");
            return;
        }

        if (data.Rounds.Count > 0)
        {
            Console.WriteLine("Hráče lze odhlásit jen před začátkem prvního kola.");
            return;
        }

        string nickname = Input.ReadText("Přezdívka hráče: ").Trim();
        int playerIndex = -1;
        for (int i = 0; i < data.RegisteredPlayers.Count; i++)
        {
            if (data.RegisteredPlayers[i].Equals(nickname, StringComparison.OrdinalIgnoreCase))
            {
                playerIndex = i;
                break;
            }
        }

        if (playerIndex == -1)
        {
            Console.WriteLine("Hráč není do turnaje přihlášen.");
            return;
        }

        string savedNickname = data.RegisteredPlayers[playerIndex];
        data.RegisteredPlayers.RemoveAt(playerIndex);
        storage.Save(data);
        Console.WriteLine("Hráč " + savedNickname + " byl odhlášen z turnaje.");
    }

    private static void DisplayStandings()
    {
        SaveData data = LoadData();

        if (data.RegisteredPlayers.Count == 0)
        {
            Console.WriteLine("Do turnaje zatím není přihlášen žádný hráč.");
            return;
        }

        List<string> players = new List<string>();
        List<int> playedMatches = new List<int>();
        List<int> points = new List<int>();

        foreach (string nickname in data.RegisteredPlayers)
        {
            players.Add(nickname);
            playedMatches.Add(0);
            points.Add(0);
        }

        foreach (MatchRecord match in data.Matches)
        {
            int player1Index = FindRegisteredPlayer(players, match.Player1);
            int player2Index = FindRegisteredPlayer(players, match.Player2);

            if (match.Result == "VYHRA1")
            {
                if (player1Index != -1)
                {
                    playedMatches[player1Index]++;
                    points[player1Index] += 3;
                }

                if (player2Index != -1)
                {
                    playedMatches[player2Index]++;
                }
            }
            else if (match.Result == "VYHRA2")
            {
                if (player1Index != -1)
                {
                    playedMatches[player1Index]++;
                }

                if (player2Index != -1)
                {
                    playedMatches[player2Index]++;
                    points[player2Index] += 3;
                }
            }
            else if (match.Result == "REMIZA")
            {
                if (player1Index != -1)
                {
                    playedMatches[player1Index]++;
                    points[player1Index]++;
                }

                if (player2Index != -1)
                {
                    playedMatches[player2Index]++;
                    points[player2Index]++;
                }
            }
        }

        foreach (ByeRecord bye in data.Byes)
        {
            int playerIndex = FindRegisteredPlayer(players, bye.Nickname);
            if (playerIndex != -1)
            {
                points[playerIndex] += 3;
            }
        }

        for (int i = 0; i < players.Count - 1; i++)
        {
            for (int j = i + 1; j < players.Count; j++)
            {
                bool shouldSwap = points[j] > points[i];
                if (points[j] == points[i] &&
                    string.Compare(players[j], players[i], StringComparison.OrdinalIgnoreCase) < 0)
                {
                    shouldSwap = true;
                }

                if (shouldSwap)
                {
                    string temporaryPlayer = players[i];
                    players[i] = players[j];
                    players[j] = temporaryPlayer;

                    int temporaryMatches = playedMatches[i];
                    playedMatches[i] = playedMatches[j];
                    playedMatches[j] = temporaryMatches;

                    int temporaryPoints = points[i];
                    points[i] = points[j];
                    points[j] = temporaryPoints;
                }
            }
        }

        Console.WriteLine("Pořadí | Přezdívka | Odehrané zápasy | Body");
        for (int i = 0; i < players.Count; i++)
        {
            Console.WriteLine(
                (i + 1) + ". | " + players[i] + " | " +
                playedMatches[i] + " | " + points[i]);
        }
    }

    private static int FindRegisteredPlayer(List<string> players, string nickname)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Equals(nickname, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static void CreateRound()
    {
        SaveData data = LoadData();

        if (data.Tournament == null)
        {
            Console.WriteLine("Nejprve založte turnaj.");
            return;
        }

        if (data.RegisteredPlayers.Count == 0)
        {
            Console.WriteLine("Před vytvořením kola přihlaste alespoň jednoho hráče.");
            return;
        }

        foreach (RoundRecord round in data.Rounds)
        {
            if (!round.IsClosed)
            {
                Console.WriteLine("Nejprve musí být uzavřeno otevřené kolo.");
                return;
            }
        }

        int number = 1;
        foreach (RoundRecord round in data.Rounds)
        {
            if (round.Number >= number)
            {
                number = round.Number + 1;
            }
        }

        if (number > data.Tournament.RoundCount)
        {
            Console.WriteLine($"Počet kol turnaje je {data.Tournament.RoundCount}.");
            return;
        }

        data.Rounds.Add(new RoundRecord(number, false));
        storage.Save(data);
        Console.WriteLine($"Kolo {number} bylo vytvořeno.");
    }

    private static void AddMatchPair()
    {
        SaveData data = LoadData();
        RoundRecord? round = null;
        foreach (RoundRecord savedRound in data.Rounds)
        {
            if (!savedRound.IsClosed)
            {
                round = savedRound;
                break;
            }
        }

        if (round == null)
        {
            Console.WriteLine("Neexistuje otevřené kolo.");
            return;
        }

        string player1 = Input.ReadText("Přezdívka prvního hráče: ").Trim();
        string player2 = Input.ReadText("Přezdívka druhého hráče: ").Trim();

        if (player1.Equals(player2, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Hráč nemůže hrát sám proti sobě.");
            return;
        }

        if (!IsRegistered(data, player1) || !IsRegistered(data, player2))
        {
            Console.WriteLine("Oba hráči musí být přihlášeni do turnaje.");
            return;
        }

        foreach (MatchRecord match in data.Matches)
        {
            if (match.Round == round.Number &&
                (MatchContains(match, player1) || MatchContains(match, player2)))
            {
                Console.WriteLine("Hráč už je v tomto kole zařazen.");
                return;
            }
        }

        foreach (ByeRecord bye in data.Byes)
        {
            if (bye.Round == round.Number &&
                (bye.Nickname.Equals(player1, StringComparison.OrdinalIgnoreCase) ||
                 bye.Nickname.Equals(player2, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Hráč už má v tomto kole volný los.");
                return;
            }
        }

        int number = 1;
        foreach (MatchRecord match in data.Matches)
        {
            if (match.Round == round.Number && match.Number >= number)
            {
                number = match.Number + 1;
            }
        }

        data.Matches.Add(new MatchRecord(
            round.Number,
            number,
            player1,
            player2,
            "-"));
        storage.Save(data);
        Console.WriteLine($"Zápas {number} kola {round.Number} byl vytvořen.");
    }

    private static void AssignBye()
    {
        SaveData data = LoadData();
        RoundRecord? round = null;
        foreach (RoundRecord savedRound in data.Rounds)
        {
            if (!savedRound.IsClosed)
            {
                round = savedRound;
                break;
            }
        }

        if (round == null)
        {
            Console.WriteLine("Neexistuje otevřené kolo.");
            return;
        }

        if (data.RegisteredPlayers.Count % 2 == 0)
        {
            Console.WriteLine("Volný los lze přidělit jen při lichém počtu přihlášených hráčů.");
            return;
        }

        foreach (ByeRecord bye in data.Byes)
        {
            if (bye.Round == round.Number)
            {
                Console.WriteLine("Toto kolo už má přidělený volný los.");
                return;
            }
        }

        string nickname = Input.ReadText("Přezdívka hráče s volným losem: ").Trim();
        if (!IsRegistered(data, nickname))
        {
            Console.WriteLine("Hráč není přihlášen do turnaje.");
            return;
        }

        foreach (MatchRecord match in data.Matches)
        {
            if (match.Round == round.Number && MatchContains(match, nickname))
            {
                Console.WriteLine("Hráč už je v tomto kole zařazen v zápase.");
                return;
            }
        }

        data.Byes.Add(new ByeRecord(round.Number, FindRegisteredNickname(data, nickname)));
        storage.Save(data);
        Console.WriteLine($"Hráč {nickname} dostal v kole {round.Number} volný los.");
    }

    private static string FindRegisteredNickname(SaveData data, string nickname)
    {
        foreach (string registeredPlayer in data.RegisteredPlayers)
        {
            if (registeredPlayer.Equals(nickname, StringComparison.OrdinalIgnoreCase))
            {
                return registeredPlayer;
            }
        }

        return nickname;
    }

    private static bool IsRegistered(SaveData data, string nickname)
    {
        foreach (string registeredPlayer in data.RegisteredPlayers)
        {
            if (registeredPlayer.Equals(nickname, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchContains(MatchRecord match, string nickname)
    {
        return match.Player1.Equals(nickname, StringComparison.OrdinalIgnoreCase) ||
               match.Player2.Equals(nickname, StringComparison.OrdinalIgnoreCase);
    }

    private static void SetMatchResult()
    {
        SaveData data = LoadData();
        int roundNumber = Input.ReadInteger("Číslo kola: ", 1, int.MaxValue);
        int matchNumber = Input.ReadInteger("Číslo zápasu: ", 1, int.MaxValue);

        RoundRecord? round = null;
        foreach (RoundRecord savedRound in data.Rounds)
        {
            if (savedRound.Number == roundNumber)
            {
                round = savedRound;
                break;
            }
        }

        if (round == null)
        {
            Console.WriteLine("Kolo neexistuje.");
            return;
        }

        if (round.IsClosed)
        {
            Console.WriteLine("Kolo je uzavřené, výsledky už nejde měnit.");
            return;
        }

        MatchRecord? match = null;
        int matchIndex = -1;
        for (int i = 0; i < data.Matches.Count; i++)
        {
            if (data.Matches[i].Round == roundNumber &&
                data.Matches[i].Number == matchNumber)
            {
                match = data.Matches[i];
                matchIndex = i;
                break;
            }
        }

        if (match == null)
        {
            Console.WriteLine("Zápas neexistuje.");
            return;
        }

        Console.WriteLine("1. Výhra prvního hráče");
        Console.WriteLine("2. Výhra druhého hráče");
        Console.WriteLine("3. Remíza");
        int choice = Input.ReadMenuChoice("Výsledek: ", 1, 3);
        string result = "-";
        if (choice == 1)
        {
            result = "VYHRA1";
        }
        else if (choice == 2)
        {
            result = "VYHRA2";
        }
        else
        {
            result = "REMIZA";
        }

        data.Matches[matchIndex] = new MatchRecord(
            match.Round,
            match.Number,
            match.Player1,
            match.Player2,
            result);
        storage.Save(data);
        Console.WriteLine("Výsledek zápasu byl uložen.");
    }

    private static void CloseRound()
    {
        SaveData data = LoadData();
        int roundNumber = Input.ReadInteger("Číslo kola: ", 1, int.MaxValue);
        RoundRecord? round = null;
        int roundIndex = -1;
        for (int i = 0; i < data.Rounds.Count; i++)
        {
            if (data.Rounds[i].Number == roundNumber)
            {
                round = data.Rounds[i];
                roundIndex = i;
                break;
            }
        }


        if (round == null)
        {
            Console.WriteLine("Kolo neexistuje.");
            return;
        }

        if (round.IsClosed)
        {
            Console.WriteLine("Kolo je už uzavřené.");
            return;
        }

        string missingMatches = "";
        List<string> assignedPlayers = new List<string>();
        int byeCount = 0;
        foreach (MatchRecord match in data.Matches)
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

                assignedPlayers.Add(match.Player1);
                assignedPlayers.Add(match.Player2);
            }
        }

        if (missingMatches != "")
        {
            Console.WriteLine($"Kolo nejde uzavřít, chybí výsledky zápasů: {missingMatches}.");
            return;
        }

        foreach (ByeRecord bye in data.Byes)
        {
            if (bye.Round == roundNumber)
            {
                byeCount++;
                assignedPlayers.Add(bye.Nickname);
            }
        }

        for (int i = 0; i < assignedPlayers.Count; i++)
        {
            for (int j = i + 1; j < assignedPlayers.Count; j++)
            {
                if (assignedPlayers[i].Equals(
                        assignedPlayers[j],
                        StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Hráč je v kole zařazen více než jednou.");
                    return;
                }
            }
        }

        int expectedByeCount = data.RegisteredPlayers.Count % 2;
        if (byeCount != expectedByeCount)
        {
            Console.WriteLine(
                expectedByeCount == 1
                    ? "Před uzavřením kola přidělte jeden volný los."
                    : "V kole s párným počtem hráčů nesmí být volný los.");
            return;
        }

        foreach (string nickname in data.RegisteredPlayers)
        {
            if (FindRegisteredPlayer(assignedPlayers, nickname) == -1)
            {
                Console.WriteLine(
                    $"Hráč {nickname} ještě není zařazen do zápasu ani nemá volný los.");
                return;
            }
        }

        data.Rounds[roundIndex] = new RoundRecord(round.Number, true);
        storage.Save(data);
        Console.WriteLine($"Kolo {roundNumber} bylo uzavřeno.");
    }

    private static void DisplayMatches()
    {
        SaveData data = LoadData();
        if (data.Matches.Count == 0 && data.Byes.Count == 0)
        {
            Console.WriteLine("Zatím nejsou uložené žádné zápasy ani volné losy.");
            return;
        }

        List<MatchRecord> matches = new List<MatchRecord>(data.Matches);
        for (int i = 0; i < matches.Count - 1; i++)
        {
            for (int j = i + 1; j < matches.Count; j++)
            {
                if (matches[j].Round < matches[i].Round ||
                    (matches[j].Round == matches[i].Round &&
                     matches[j].Number < matches[i].Number))
                {
                    MatchRecord temporary = matches[i];
                    matches[i] = matches[j];
                    matches[j] = temporary;
                }
            }
        }

        foreach (MatchRecord match in matches)
        {
            string result = match.Result;
            if (match.Result == "VYHRA1")
            {
                result = $"vyhrál {match.Player1}";
            }
            else if (match.Result == "VYHRA2")
            {
                result = $"vyhrál {match.Player2}";
            }
            else if (match.Result == "REMIZA")
            {
                result = "remíza";
            }
            else if (match.Result == "-")
            {
                result = "neodehráno";
            }

            Console.WriteLine(
                $"Kolo {match.Round}, zápas {match.Number}: " +
                $"{match.Player1} vs {match.Player2} — {result}");
        }

        foreach (ByeRecord bye in data.Byes)
        {
            Console.WriteLine($"Kolo {bye.Round}: {bye.Nickname} má volný los.");
        }
    }
}
