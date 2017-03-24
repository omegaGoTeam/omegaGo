namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// A user. Always an object. Users are sent in downstream messages, but upstream just the name is provided. It would have been cleaner to have the user flags a separate JSON fields, but users are sent a lot and I didn't want to use so much bandwidth.
    /// </summary>
    public class User
    {
        public string Name { get; set; }
        /// <summary>
        /// The rank of the user. Optional; missing means no rank. Rank may also be "?".
        /// </summary>
        public string Rank { get; set; }
        /// <summary>
        /// A string. Each character in the string represents a flag. See below.
        /// </summary>
        public string Flags { get; set; }
        /// <summary>
        /// Optional. One of normal, robot_ranked, teacher, jr_admin, sr_admin, or super_admin. If not present, then it is normal.
        /// </summary>
        public string AuthLevel { get; set; }

        public const char FlagGuest = 'g';
        public const char FlagConnected = 'c';
        public const char FlagDeleted = 'd';
        public const char FlagSleeping = 's';
        public const char FlagHasAvatar = 'a';
        public const char FlagRobot = 'r';
        public const string FlagWonAKgsTournament = "TT";
        public const char FlagRunnerUpInAKgsTournament = 't';
        public const char FlagPlayingAGame = 'p';
        public const char FlagPlayingAKgsTournamentGame = 'P';
        public const char FlagKgsPlusSubscriber = '*';
        public const char FlagKgsMeijin = '!';
        public const char FlagCanPlayRankedGames = '=';
        public const char FlagPlaysStrongerPlayersFarMoreOftenThanWeakerOnes = '~';

        public override string ToString()
        {
            return Name + "(" + Rank + "," + AuthLevel + ")";
        }
    }
}