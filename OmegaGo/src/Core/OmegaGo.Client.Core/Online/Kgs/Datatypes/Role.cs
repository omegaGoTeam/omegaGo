namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// A role indicates how a player is involved in a game. A player may hold more than one role in a game. Represented as a string. 
    /// </summary>
    public static class Role
    {
        public const string White = "white";
        public const string Black = "black";

        // We do not use these roles:
        public const string White2 = "white_2";
        public const string Black2 = "black_2";
        public const string ChallengeCreator = "challengeCreator";
        public const string Owner = "owner";
    }
}