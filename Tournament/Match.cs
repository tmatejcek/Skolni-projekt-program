public enum MatchResult
{
    None,
    Player1Win,
    Player2Win,
    Draw
}

public sealed class TournamentException : Exception
{
    public TournamentException(string message)
        : base(message)
    {
    }
}

public sealed class Match
{
    public Match(Player player1, Player player2)
    {
        Player1 = player1;
        Player2 = player2;
        Result = MatchResult.None;
    }

    public Player Player1 { get; }
    public Player Player2 { get; }
    public MatchResult Result { get; private set; }

    public bool Involves(Player player)
    {
        if (Player1 == player || Player2 == player)
        {
            return true;
        }

        return false;
    }

    public void SetResult(MatchResult result)
    {
        if (result == MatchResult.None)
        {
            throw new TournamentException("Zápas musí mít výsledek: výhra prvního hráče, výhra druhého hráče nebo remíza.");
        }

        Result = result;
    }
}
