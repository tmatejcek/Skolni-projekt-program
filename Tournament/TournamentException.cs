namespace TurnajovySystem.Logic;

/// <summary>
/// Porušení pravidel turnaje. Zpráva je srozumitelná a je určená k vypsání uživateli.
/// </summary>
public class TournamentException : Exception
{
    public TournamentException(string message) : base(message)
    {
    }
}
