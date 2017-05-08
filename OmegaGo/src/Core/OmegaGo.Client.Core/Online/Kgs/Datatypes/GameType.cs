namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// Holds types of KGS channels. We only support challenges, free and ranked games.
    /// </summary>
    public static class GameType
    {
        public const string Challenge = "challenge"; // (Actually not a game, a challenge is a user trying to set up a custom game)
        public const string Free = "free";
        public const string Ranked = "ranked";
        
        // We do not support the following game types:

        //demonstration
        //review
        //rengo_review
        //teaching
        //simul
        //rengo
        //free
        //ranked
        //tournament
    }
}