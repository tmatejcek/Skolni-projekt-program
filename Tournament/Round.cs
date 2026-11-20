namespace TurnajovySystem.Logic
{
    public sealed class Round
    {
        private readonly List<Match> matches = new List<Match>();

        public int Number { get; }
        public IReadOnlyList<Match> Matches
        {
            get { return matches; }
        }

        public Player? Bye { get; private set; }
        public bool IsClosed { get; private set; }

        public bool AllResultsEntered
        {
            get
            {
                foreach (Match match in matches)
                {
                    if (match.Result == MatchResult.None)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        internal Round(int number)
        {
            Number = number;
        }

        public bool Contains(Player player)
        {
            if (ReferenceEquals(Bye, player))
            {
                return true;
            }

            foreach (Match match in matches)
            {
                if (match.Involves(player))
                {
                    return true;
                }
            }

            return false;
        }

        internal void AddMatch(Player player1, Player player2)
        {
            EnsureOpen();

            if (ReferenceEquals(player1, player2))
            {
                throw new TournamentException(
                    $"Hráč '{player1.Nickname}' nemůže hrát sám proti sobě.");
            }

            if (Contains(player1))
            {
                throw new TournamentException(
                    $"Hráč '{player1.Nickname}' už je v kole {Number} zařazen, v kole smí být nejvýše jednou.");
            }

            if (Contains(player2))
            {
                throw new TournamentException(
                    $"Hráč '{player2.Nickname}' už je v kole {Number} zařazen, v kole smí být nejvýše jednou.");
            }

            matches.Add(new Match(player1, player2));
        }

        internal void SetBye(Player player)
        {
            EnsureOpen();

            if (Bye != null)
            {
                throw new TournamentException($"Kolo {Number} už má volný los.");
            }

            if (Contains(player))
            {
                throw new TournamentException(
                    $"Hráč '{player.Nickname}' už je v kole {Number} zařazen, v kole smí být nejvýše jednou.");
            }

            Bye = player;
        }

        internal void SetResult(int matchNumber, MatchResult result)
        {
            if (IsClosed)
            {
                throw new TournamentException(
                    $"Kolo {Number} je uzavřené, výsledky už nejdou měnit.");
            }

            if (matchNumber < 1 || matchNumber > matches.Count)
            {
                throw new TournamentException(
                    $"Zápas číslo {matchNumber} v kole {Number} neexistuje (kolo má {matches.Count} zápasů).");
            }

            matches[matchNumber - 1].SetResult(result);
        }

        internal void Close()
        {
            if (IsClosed)
            {
                throw new TournamentException($"Kolo {Number} je už uzavřené.");
            }

            List<int> missing = new List<int>();
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].Result == MatchResult.None)
                {
                    missing.Add(i + 1);
                }
            }

            if (missing.Count > 0)
            {
                throw new TournamentException(
                    $"Kolo {Number} nejde uzavřít, chybí výsledky zápasů: {string.Join(", ", missing)}.");
            }

            IsClosed = true;
        }

        private void EnsureOpen()
        {
            if (IsClosed)
            {
                throw new TournamentException($"Kolo {Number} je uzavřené, nejde už měnit.");
            }
        }
    }
}
