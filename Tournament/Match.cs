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
