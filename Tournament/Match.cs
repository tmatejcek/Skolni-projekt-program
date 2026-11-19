namespace TurnajovySystem.Logic;

public enum MatchResult
{
    None,   // zatím nezadáno
    Win1,   // vyhrál první hráč
    Win2,   // vyhrál druhý hráč
    Draw    // remíza
}

/// <summary>
/// Jeden zápas. Vytváří ho a mění jen Round / Tournament, aby se nedaly obejít pravidla.
/// </summary>
public sealed class Match
{
    public Player Player1 { get; }
    public Player Player2 { get; }
    public MatchResult Result { get; private set; } = MatchResult.None;

    internal Match(Player player1, Player player2)
    {
        if (ReferenceEquals(player1, player2))
            throw new TournamentException($"Hráč '{player1.Nickname}' nemůže hrát sám proti sobě.");

        Player1 = player1;
        Player2 = player2;
    }

    public bool Involves(Player player) =>
        ReferenceEquals(Player1, player) || ReferenceEquals(Player2, player);

    internal void SetResult(MatchResult result)
    {
        if (result == MatchResult.None)
            throw new TournamentException("Výsledek musí být výhra 1. hráče, výhra 2. hráče nebo remíza.");

        Result = result;
    }
}
