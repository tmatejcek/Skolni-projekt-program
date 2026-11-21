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
                "12. Zobrazit zápasy kola\n" +
                "13. Zobrazit tabulku\n" +
                "14. Přehled turnaje\n" +
                "15. Ukončit program");

            int choice = Input.ReadMenuChoice("Zadejte číslo volby: ", 1, 15);

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
                    DisplayTournamentOverview();
                    break;
                case 15:
                    Console.WriteLine("Ukončuji program.");
                    return;
            }
        }
    }

    private static string GetActionName(int choice)
    {
        switch (choice)
        {
            case 1:
                return "Zaregistrovat hráče";
            case 2:
                return "Odebrat hráče";
            case 3:
                return "Zobrazit hráče";
            case 4:
                return "Založit turnaj";
            case 5:
                return "Přihlásit hráče do turnaje";
            case 6:
                return "Odhlásit hráče z turnaje";
            case 7:
                return "Vytvořit nové kolo";
            case 8:
                return "Určit dvojice kola";
            case 9:
                return "Přiřadit volný los";
            case 10:
                return "Zadat nebo opravit výsledek zápasu";
            case 11:
                return "Uzavřít kolo";
            case 12:
                return "Zobrazit zápasy kola";
            case 13:
                return "Zobrazit tabulku";
            case 14:
                return "Přehled turnaje";
            case 15:
                return "Ukončit program";
            default:
                return "Neznámá volba";
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

    private static void SaveAfterAction(SaveData data, Action action, string successMessage)
    {
        try
        {
            action();
            storage.Save(data);
            Console.WriteLine("Hotovo: " + successMessage);
        }
        catch (TournamentException exception)
        {
            Console.WriteLine("Akce se neprovedla: " + exception.Message);
        }
        catch (IOException exception)
        {
            Console.WriteLine("Změnu se nepodařilo uložit: " + exception.Message);
        }
        catch (UnauthorizedAccessException exception)
        {
            Console.WriteLine("K souboru s daty není povolen přístup: " + exception.Message);
        }
    }

    private static void AddPlayer()
    {
        string nickname = Input.ReadText("Přezdívka hráče: ").Trim();
        string name = Input.ReadText("Jméno hráče: ").Trim();
        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);

        SaveAfterAction(
            data,
            () => tournament.AddPlayer(nickname, name),
            "Hráč " + name + " (" + nickname + ") byl přidán do evidence.");
    }

    private static void RemovePlayer()
    {
        string nickname = Input.ReadText("Přezdívka hráče: ").Trim();
        if (!Input.ReadYesNo("Opravdu chcete odebrat hráče " + nickname + "? (a/n): "))
        {
            Console.WriteLine("Odebrání hráče bylo zrušeno.");
            return;
        }

        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);
        SaveAfterAction(
            data,
            () => tournament.RemovePlayer(nickname),
            "Hráč " + nickname + " byl odebrán z evidence.");
    }

    private static void ListPlayers()
    {
        SaveData data = LoadData();
        if (data.Players.Count == 0)
        {
            Console.WriteLine("Žádní hráči nejsou v evidenci.");
            return;
        }

        Console.WriteLine("Hráči v evidenci:");
        foreach (PlayerRecord player in data.Players)
        {
            Console.WriteLine(player.Nickname + " - " + player.Name);
        }
    }

    private static void CreateTournament()
    {
        string name = Input.ReadText("Název turnaje: ").Trim();
        string discipline = Input.ReadText("Disciplína: ").Trim();
        int roundCount = Input.ReadInteger("Počet kol: ", 1, int.MaxValue);
        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);

        SaveAfterAction(
            data,
            () => tournament.CreateTournament(name, discipline, roundCount),
            "Turnaj " + name + " (" + discipline + ") byl založen na " + roundCount + " kol.");
    }

    private static void RegisterPlayer()
    {
        string nickname = Input.ReadText("Přezdívka hráče: ").Trim();
        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);

        SaveAfterAction(
            data,
            () => tournament.RegisterPlayer(nickname),
            "Hráč " + nickname + " byl přihlášen do turnaje.");
    }

    private static void UnregisterPlayer()
    {
        string nickname = Input.ReadText("Přezdívka hráče: ").Trim();
        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);

        SaveAfterAction(
            data,
            () => tournament.UnregisterPlayer(nickname),
            "Hráč " + nickname + " byl odhlášen z turnaje.");
    }

    private static void CreateRound()
    {
        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);

        SaveAfterAction(
            data,
            () => tournament.CreateRound(),
            "Nové kolo bylo vytvořeno.");
    }

    private static void AddMatchPair()
    {
        string player1 = Input.ReadText("Přezdívka prvního hráče: ").Trim();
        string player2 = Input.ReadText("Přezdívka druhého hráče: ").Trim();
        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);

        SaveAfterAction(
            data,
            () => tournament.AddMatchPair(player1, player2),
            "Zápas " + player1 + " proti " + player2 + " byl přidán do otevřeného kola.");
    }

    private static void AssignBye()
    {
        string nickname = Input.ReadText("Přezdívka hráče s volným losem: ").Trim();
        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);

        SaveAfterAction(
            data,
            () => tournament.AssignBye(nickname),
            "Hráči " + nickname + " byl přidělen volný los v otevřeném kole.");
    }

    private static void SetMatchResult()
    {
        int roundNumber = Input.ReadInteger("Číslo kola: ", 1, int.MaxValue);
        int matchNumber = Input.ReadInteger("Číslo zápasu: ", 1, int.MaxValue);
        Console.WriteLine("1. Výhra prvního hráče");
        Console.WriteLine("2. Výhra druhého hráče");
        Console.WriteLine("3. Remíza");
        int choice = Input.ReadMenuChoice("Výsledek: ", 1, 3);

        MatchResult result = MatchResult.Draw;
        if (choice == 1)
        {
            result = MatchResult.Player1Win;
        }
        else if (choice == 2)
        {
            result = MatchResult.Player2Win;
        }

        string resultDescription = "remíza";
        if (choice == 1)
        {
            resultDescription = "výhra prvního hráče";
        }
        else if (choice == 2)
        {
            resultDescription = "výhra druhého hráče";
        }

        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);
        SaveAfterAction(
            data,
            () => tournament.SetMatchResult(roundNumber, matchNumber, result),
            "Výsledek zápasu " + matchNumber + " v kole " + roundNumber +
            " byl nastaven: " + resultDescription + ".");
    }

    private static void CloseRound()
    {
        int roundNumber = Input.ReadInteger("Číslo kola: ", 1, int.MaxValue);
        SaveData data = LoadData();
        Tournament tournament = new Tournament(data);

        SaveAfterAction(
            data,
            () => tournament.CloseRound(roundNumber),
            $"Kolo {roundNumber} bylo uzavřeno.");
    }

    private static void DisplayTournamentOverview()
    {
        SaveData data = LoadData();
        if (data.Tournament == null)
        {
            Console.WriteLine("Zatím není založen žádný turnaj.");
            return;
        }

        Console.WriteLine("Přehled turnaje:");
        Console.WriteLine("Název turnaje: " + data.Tournament.Name);
        Console.WriteLine("Disciplína: " + data.Tournament.Discipline);
        Console.WriteLine("Počet kol: " + data.Tournament.RoundCount);

        RoundRecord? currentRound = null;
        foreach (RoundRecord round in data.Rounds)
        {
            if (currentRound == null || round.Number > currentRound.Number)
            {
                currentRound = round;
            }
        }

        if (currentRound == null)
        {
            Console.WriteLine("Aktuální kolo: turnaj ještě nezačal.");
        }
        else
        {
            string state = currentRound.IsClosed ? "uzavřené" : "otevřené";
            Console.WriteLine($"Aktuální kolo: {currentRound.Number} ({state})");
        }

        Console.WriteLine("Přihlášení hráči:");
        if (data.RegisteredPlayers.Count == 0)
        {
            Console.WriteLine("  žádní");
        }
        else
        {
            foreach (string nickname in data.RegisteredPlayers)
            {
                Console.WriteLine("  " + nickname);
            }
        }
    }

    private static void DisplayStandings()
    {
        SaveData data = LoadData();
        List<StandingRow> rows = Standings.Calculate(data);
        if (rows.Count == 0)
        {
            Console.WriteLine("Do turnaje zatím není přihlášen žádný hráč.");
            return;
        }

        Console.WriteLine("Průběžná tabulka:");
        Console.WriteLine("Pořadí | Přezdívka | Odehrané zápasy | Body");
        foreach (StandingRow row in rows)
        {
            Console.WriteLine(
                row.Position + ". | " + row.Nickname + " | " +
                row.PlayedMatches + " | " + row.Points);
        }
    }

    private static void DisplayMatches()
    {
        SaveData data = LoadData();
        int roundNumber = Input.ReadInteger("Číslo kola: ", 1, int.MaxValue);
        RoundRecord? selectedRound = null;
        foreach (RoundRecord round in data.Rounds)
        {
            if (round.Number == roundNumber)
            {
                selectedRound = round;
                break;
            }
        }

        if (selectedRound == null)
        {
            Console.WriteLine("Kolo neexistuje.");
            return;
        }

        Console.WriteLine(
            $"Zápasy kola {selectedRound.Number} — " +
            (selectedRound.IsClosed ? "uzavřené" : "otevřené"));

        bool hasEntries = false;
        foreach (MatchRecord match in data.Matches)
        {
            if (match.Round == roundNumber)
            {
                hasEntries = true;
                break;
            }
        }

        foreach (ByeRecord bye in data.Byes)
        {
            if (bye.Round == roundNumber)
            {
                hasEntries = true;
                break;
            }
        }

        if (!hasEntries)
        {
            Console.WriteLine("V tomto kole zatím nejsou uložené žádné zápasy ani volné losy.");
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
            if (match.Round != roundNumber)
            {
                continue;
            }

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
                $"Zápas {match.Number}: {match.Player1} vs {match.Player2} — {result}");
        }

        foreach (ByeRecord bye in data.Byes)
        {
            if (bye.Round == roundNumber)
            {
                Console.WriteLine($"{bye.Nickname} má volný los.");
            }
        }
    }
}
