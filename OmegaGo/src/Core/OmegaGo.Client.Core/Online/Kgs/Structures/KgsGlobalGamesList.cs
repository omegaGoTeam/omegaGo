namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents a global list of games. There are three such lists: ACTIVES, CHALLENGES and FANS.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Structures.KgsGameContainer" />
    public class KgsGlobalGamesList : KgsGameContainer
    {
        public KgsGlobalGamesList(int channelId, string name)
        {
            ChannelId = channelId;
            Name = name;
            switch (Name)
            {
                case "CHALLENGES":
                    Kind = GlobalGamesListKind.Challenges;
                    break;
                case "ACTIVES":
                    Kind = GlobalGamesListKind.ActiveGames;
                    break;
                case "FANS":
                    Kind = GlobalGamesListKind.Favourites;
                    break;
            }
        }

        /// <summary>
        /// Gets information on which of the three global lists this is.
        /// </summary>
        public GlobalGamesListKind Kind { get; }

        public override string ToString()
        {
            switch (Kind)
            {
                case GlobalGamesListKind.ActiveGames:
                    return "All observable games";
                case GlobalGamesListKind.Challenges:
                    return "All open challenges";
                case GlobalGamesListKind.Favourites:
                    return "Favourite games";
                default:
                    return base.ToString();
            }
        }
    }

    public enum GlobalGamesListKind
    {
        /// <summary>
        /// This list contains all open challenges.
        /// </summary>
        Challenges,
        /// <summary>
        /// This list contains all games in progress. It doesn't contain challenges.
        /// </summary>
        ActiveGames,
        /// <summary>
        /// This list contains all game channels favourited by the logged-in users. We do not make use of this in omegaGo.
        /// </summary>
        Favourites,
    }
}