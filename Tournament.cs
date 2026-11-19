namespace TurnajovySystem.Logic;

/// <summary>
/// Hlavní třída logiky. Drží všechny hráče, přihlášené hráče a kola a hlídá pravidla turnaje.
/// Neobsahuje žádný vstup ani výstup, chyby hlásí výjimkou TournamentException.
/// </summary>
public sealed class Tournament
{
    private readonly Dictionary<string, Player> _byNickname = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Player> _players = new();
    private readonly List<Player> _registered = new();
    private readonly List<Round> _rounds = new();

    /// <summary>Byl už turnaj založen? (Hráči existují i bez turnaje.)</summary>
    public bool IsCreated { get; private set; }
    public string Name { get; private set; } = "";
    public string Discipline { get; private set; } = "";
    public int TotalRounds { get; private set; }

    public IReadOnlyList<Player> Players => _players;
    public IReadOnlyList<Player> Registered => _registered;
    public IReadOnlyList<Round> Rounds => _rounds;

    /// <summary>Turnaj je rozehraný, pokud existuje aspoň jedno kolo.</summary>
    public bool HasStarted => _rounds.Count > 0;

    // ---------- Hráči ----------

    public Player AddPlayer(string nickname, string name)
    {
        var player = new Player(nickname, name);

        if (_byNickname.ContainsKey(player.Nickname))
            throw new TournamentException(
                $"Přezdívka '{player.Nickname}' už existuje (velikost písmen se nerozlišuje).");

        _byNickname.Add(player.Nickname, player);
        _players.Add(player);
        return player;
    }

    public bool TryGetPlayer(string nickname, out Player player) =>
        _byNickname.TryGetValue((nickname ?? "").Trim(), out player!);

    public Player GetPlayer(string nickname)
    {
        if (!TryGetPlayer(nickname, out var player))
            throw new TournamentException($"Hráč s přezdívkou '{(nickname ?? "").Trim()}' neexistuje.");
        return player;
    }

    /// <summary>Smaže hráče. Nejde to, pokud už figuruje v nějakém kole.</summary>
    public void RemovePlayer(string nickname)
    {
        var player = GetPlayer(nickname);

        foreach (var round in _rounds)
        {
            if (!round.Contains(player))
                continue;

            bool played = round.Matches.Any(m => m.Involves(player) && m.Result != MatchResult.None);
            throw new TournamentException(played
                ? $"Hráč '{player.Nickname}' už odehrál zápas, smazat ho nejde."
                : $"Hráč '{player.Nickname}' je zařazen v kole {round.Number}, smazat ho nejde.");
        }

        // Do kola nikdy nenastoupil, takže je to buď neregistrovaný hráč, nebo přihlášený před 1. kolem.
        _registered.Remove(player);
        _players.Remove(player);
        _byNickname.Remove(player.Nickname);
    }

    // ---------- Turnaj ----------

    public void Create(string name, string discipline, int totalRounds)
    {
        if (IsCreated)
            throw new TournamentException("Turnaj už je založen, otevřený může být vždy jen jeden.");

        name = (name ?? "").Trim();
        discipline = (discipline ?? "").Trim();

        if (name.Length == 0)
            throw new TournamentException("Název turnaje nesmí být prázdný.");
        if (discipline.Length == 0)
            throw new TournamentException("Disciplína nesmí být prázdná.");
        if (totalRounds < 1)
            throw new TournamentException("Počet kol musí být aspoň 1.");

        Name = name;
        Discipline = discipline;
        TotalRounds = totalRounds;
        IsCreated = true;
    }

    /// <summary>Jde ještě přihlašovat a odhlašovat? Jinak vyhodí výjimku.</summary>
    public void EnsureRegistrationOpen()
    {
        if (!IsCreated)
            throw new TournamentException("Turnaj ještě není založen.");
        if (HasStarted)
            throw new TournamentException(
                "Turnaj už začal (existuje 1. kolo), přihlašovat a odhlašovat hráče už nejde.");
    }

    public void Register(string nickname)
    {
        EnsureRegistrationOpen();
        var player = GetPlayer(nickname);

        if (_registered.Contains(player))
            throw new TournamentException($"Hráč '{player.Nickname}' už je do turnaje přihlášen.");

        _registered.Add(player);
    }

    public void Unregister(string nickname)
    {
        EnsureRegistrationOpen();
        var player = GetPlayer(nickname);

        if (!_registered.Remove(player))
            throw new TournamentException($"Hráč '{player.Nickname}' není do turnaje přihlášen.");
    }

    // ---------- Kola a zápasy ----------

    /// <summary>Jde teď vytvořit další kolo? Jinak vyhodí výjimku.</summary>
    public void EnsureCanCreateRound()
    {
        if (!IsCreated)
            throw new TournamentException("Turnaj ještě není založen.");
        if (_rounds.Count >= TotalRounds)
            throw new TournamentException($"Všech {TotalRounds} kol už bylo vytvořeno.");
        if (_rounds.Count > 0 && !_rounds[^1].IsClosed)
            throw new TournamentException($"Kolo {_rounds[^1].Number} ještě není uzavřené.");
        if (_registered.Count < 2)
            throw new TournamentException("Pro kolo jsou potřeba aspoň 2 přihlášení hráči.");
    }

    /// <summary>
    /// Vytvoří další kolo z ručně určených dvojic (přezdívky). Hráč, který zbude bez soupeře,
    /// dostane volný los; bez soupeře smí zbýt nejvýše jeden.
    /// Když je cokoli špatně, nic se nevytvoří.
    /// </summary>
    public Round CreateRound(IEnumerable<(string Nickname1, string Nickname2)> pairs)
    {
        EnsureCanCreateRound();

        var round = new Round(_rounds.Count + 1);
        foreach (var (nickname1, nickname2) in pairs)
            round.AddMatch(GetRegistered(nickname1), GetRegistered(nickname2));

        if (round.Matches.Count == 0)
            throw new TournamentException("Kolo musí mít aspoň jeden zápas.");

        var left = _registered.Where(p => !round.Contains(p)).ToList();
        if (left.Count > 1)
            throw new TournamentException(
                $"Bez soupeře zůstali: {string.Join(", ", left.Select(p => p.Nickname))}. " +
                "Ve kole smí zbýt nejvýše jeden hráč (volný los).");
        if (left.Count == 1)
            round.SetBye(left[0]);

        _rounds.Add(round);
        return round;
    }

    public Round GetRound(int number)
    {
        if (number < 1 || number > _rounds.Count)
            throw new TournamentException($"Kolo {number} neexistuje.");
        return _rounds[number - 1];
    }

    public void SetResult(int roundNumber, int matchNumber, MatchResult result) =>
        GetRound(roundNumber).SetResult(matchNumber, result);

    public void CloseRound(int roundNumber) =>
        GetRound(roundNumber).Close();

    private Player GetRegistered(string nickname)
    {
        var player = GetPlayer(nickname);
        if (!_registered.Contains(player))
            throw new TournamentException($"Hráč '{player.Nickname}' není do turnaje přihlášen.");
        return player;
    }
}
