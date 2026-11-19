namespace TurnajovySystem.Logic;

/// <summary>
/// Jedno kolo. Hlídá, že hráč je v kole nejvýše jednou a že kolo jde uzavřít jen se všemi výsledky.
/// </summary>
public sealed class Round
{
    private readonly List<Match> _matches = new();

    public int Number { get; }
    public IReadOnlyList<Match> Matches => _matches;

    /// <summary>Hráč s volným losem, nebo null.</summary>
    public Player? Bye { get; private set; }

    public bool IsClosed { get; private set; }

    public bool AllResultsEntered => _matches.All(m => m.Result != MatchResult.None);

    internal Round(int number)
    {
        Number = number;
    }

    /// <summary>Je hráč v tomto kole (v zápase nebo s volným losem)?</summary>
    public bool Contains(Player player) =>
        ReferenceEquals(Bye, player) || _matches.Any(m => m.Involves(player));

    internal void AddMatch(Player player1, Player player2)
    {
        EnsureOpen();

        if (ReferenceEquals(player1, player2))
            throw new TournamentException($"Hráč '{player1.Nickname}' nemůže hrát sám proti sobě.");
        if (Contains(player1))
            throw new TournamentException($"Hráč '{player1.Nickname}' už je v kole {Number} zařazen, v kole smí být nejvýše jednou.");
        if (Contains(player2))
            throw new TournamentException($"Hráč '{player2.Nickname}' už je v kole {Number} zařazen, v kole smí být nejvýše jednou.");

        _matches.Add(new Match(player1, player2));
    }

    internal void SetBye(Player player)
    {
        EnsureOpen();

        if (Bye != null)
            throw new TournamentException($"Kolo {Number} už má volný los.");
        if (Contains(player))
            throw new TournamentException($"Hráč '{player.Nickname}' už je v kole {Number} zařazen, v kole smí být nejvýše jednou.");

        Bye = player;
    }

    /// <param name="matchNumber">Číslo zápasu v kole, od 1.</param>
    internal void SetResult(int matchNumber, MatchResult result)
    {
        if (IsClosed)
            throw new TournamentException($"Kolo {Number} je uzavřené, výsledky už nejdou měnit.");
        if (matchNumber < 1 || matchNumber > _matches.Count)
            throw new TournamentException($"Zápas číslo {matchNumber} v kole {Number} neexistuje (kolo má {_matches.Count} zápasů).");

        _matches[matchNumber - 1].SetResult(result);
    }

    internal void Close()
    {
        if (IsClosed)
            throw new TournamentException($"Kolo {Number} je už uzavřené.");

        var missing = new List<int>();
        for (int i = 0; i < _matches.Count; i++)
        {
            if (_matches[i].Result == MatchResult.None)
                missing.Add(i + 1);
        }

        if (missing.Count > 0)
            throw new TournamentException(
                $"Kolo {Number} nejde uzavřít, chybí výsledky zápasů: {string.Join(", ", missing)}.");

        IsClosed = true;
    }

    private void EnsureOpen()
    {
        if (IsClosed)
            throw new TournamentException($"Kolo {Number} je uzavřené, nejde už měnit.");
    }
}
