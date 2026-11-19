using System.Globalization;

namespace TurnajovySystem.Logic;

public sealed record StandingsRow(int Rank, Player Player, int Points, int Played);

/// <summary>
/// Tabulka. Nikam se neukládá, pokaždé se přepočítá z výsledků kol.
/// Body: výhra 3, remíza 1, prohra 0, volný los 3 (bez odehraného zápasu).
/// Při shodě bodů rozhoduje abeceda podle přezdívky.
/// </summary>
public sealed class Standings
{
    public const int WinPoints = 3;
    public const int DrawPoints = 1;
    public const int LossPoints = 0;
    public const int ByePoints = 3;

    private static readonly StringComparer NicknameComparer = CreateComparer();

    public IReadOnlyList<StandingsRow> Rows { get; }

    private Standings(IReadOnlyList<StandingsRow> rows)
    {
        Rows = rows;
    }

    public static Standings Calculate(Tournament tournament)
    {
        var points = tournament.Registered.ToDictionary(p => p, _ => 0);
        var played = tournament.Registered.ToDictionary(p => p, _ => 0);

        foreach (var round in tournament.Rounds)
        {
            foreach (var match in round.Matches)
            {
                switch (match.Result)
                {
                    case MatchResult.Win1:
                        points[match.Player1] += WinPoints;
                        points[match.Player2] += LossPoints;
                        break;
                    case MatchResult.Win2:
                        points[match.Player1] += LossPoints;
                        points[match.Player2] += WinPoints;
                        break;
                    case MatchResult.Draw:
                        points[match.Player1] += DrawPoints;
                        points[match.Player2] += DrawPoints;
                        break;
                    default:
                        continue; // výsledek zatím není zadaný, zápas se nezapočítává
                }

                played[match.Player1]++;
                played[match.Player2]++;
            }

            if (round.Bye != null)
                points[round.Bye] += ByePoints; // volný los: body ano, odehraný zápas ne
        }

        var sorted = tournament.Registered
            .OrderByDescending(p => points[p])
            .ThenBy(p => p.Nickname, NicknameComparer)
            .ThenBy(p => p.Nickname, StringComparer.Ordinal)
            .ToList();

        var rows = new List<StandingsRow>();
        for (int i = 0; i < sorted.Count; i++)
            rows.Add(new StandingsRow(i + 1, sorted[i], points[sorted[i]], played[sorted[i]]));

        return new Standings(rows);
    }

    private static StringComparer CreateComparer()
    {
        try
        {
            return StringComparer.Create(new CultureInfo("cs-CZ"), ignoreCase: true);
        }
        catch (CultureNotFoundException)
        {
            return StringComparer.OrdinalIgnoreCase; // systém bez kultur, použije se prosté porovnání
        }
    }
}
