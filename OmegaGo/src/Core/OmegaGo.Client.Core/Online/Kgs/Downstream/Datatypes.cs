using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
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
    }

    public static class Role
    {
        public const string White = "white";
        public const string White2 = "white_2";
        public const string Black = "black";
        public const string Black2 = "black_2";
        public const string ChallengeCreator = "challengeCreator";
        public const string Owner = "owner";
    }

    public static class GameType
    {
        public const string Challenge = "challenge";
        public const string Free = "free";
        public const string Ranked = "ranked";
        //challenge(Actually not a game, a challenge is a user trying to set up a custom game)
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
    // TODO game flags
    // TODO sgf events
    // TODO sgf properties
    // TODO (maybe) subscription
    // TODO (maybe) game summary
    public class RulesDescription
    {
        /// <summary>
        /// The size of the board. 2 through 38 are supported by KGS.
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// One of japanese, chinese, aga, or new_zealand.
        /// </summary>
        public string Rules { get; set; }
        /// <summary>
        /// Not present for handicap 0. At most 9.
        /// </summary>
        public int Handicap { get; set; }
        /// <summary>
        /// A floating point number. Must be a multiple of 0.5, -100.0 through +100.0.
        /// </summary>
        public float Komi { get; set; }
        /// <summary>
        /// One of none, absolute, byo_yomi, or canadian.
        /// </summary>
        public string TimeSystem { get; set; }
        /// <summary>
        /// Time for the main time period, in seconds. Not needed when your time system is none. Up to 2147483 seconds.
        /// </summary>
        public int MainTime { get; set; }
        /// <summary>
        /// Time for each byo-yomi period. Only needed for Canadian and Byo-Yomi time systems. Up to 2147483 seconds.
        /// </summary>
        public int ByoYomiTime { get; set; }
        /// <summary>
        /// Number of byo-yomi periods. Only needed for byo-yomi time system. At most 255.
        /// </summary>
        public int ByoYomiPeriods { get; set; }
        /// <summary>
        /// Number of stones per byo-yomi period. Only needed for Canadian time system. At most 255.
        /// </summary>
        public int ByoYomiStones { get; set; }

        public const string RulesJapanese = "japanese";
        public const string RulesChinese = "chinese";
        public const string RulesAga = "aga";
        public const string RulesNewZealand = "new_zealand";

        public const string TimeSystemNone = "none";
        public const string TimeSystemAbsolute = "absolute";
        public const string TimeSystemJapanese = "byo_yomi";
        public const string TimeSystemCanadian = "canadian";
    }

    /// <summary>
    /// A proposal is an offer to start a game. It includes the rules and may include some or all of the players. If all the players needed are present in the proposal, then it is called a "complete" proposal.
    /// 
    ///Note: The first player in a proposal must always be the challenge owner.When manipulating a proposal, in general leave the player list in the order it started; if you want to change the roles for each player, change the role field and not the player field. You will get strange results if you don't do this.
    /// </summary>
    public class Proposal
    {
        public string GameType { get; set; }
        public RulesDescription Rules { get; set; }
        public bool Nigiri { get; set; }
        public KgsPlayer[] Players { get; set; }
        // flags
        #region Flags
        // This region may be copied to other messages that make use of flags.
        /// <summary>
        /// If set, it means that the game has been scored.
        /// </summary>
        public bool Over { get; set; }
        /// <summary>
        /// The game cannot continue because the player whose turn it is has left.
        /// </summary>
        public bool Adjourned { get; set; }
        /// <summary>
        /// Only users specified by the owner are allowed in.
        /// </summary>
        public bool Private { get; set; }
        /// <summary>
        /// Only KGS Plus subscribers are allowed in.
        /// </summary>
        public bool Subscribers { get; set; }
        /// <summary>
        /// This game is a server event, and should appear at the top of game lists.
        /// </summary>
        public bool Event { get; set; }
        /// <summary>
        /// This game was created by uploading an SGF file.
        /// </summary>
        public bool Uploaded { get; set; }
        /// <summary>
        /// This game includes a live audio track.
        /// </summary>
        public bool Audio { get; set; }
        /// <summary>
        /// The game is paused. Tournament games are paused when they are first created, to give players time to join before the clocks start.
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// This game has a name (most games are named after the players involved). In some cases, instead of seeing this flag when it is set, a text field name will appear instead.
        /// </summary>
        public bool Named { get; set; }
        /// <summary>
        /// This game has been saved to the KGS archives. Most games are saved automatically, but demonstration and review games must be saved by setting this flag.
        /// </summary>
        public bool Saved { get; set; }
        /// <summary>
        /// This game may appear on the open or active game lists.
        /// </summary>
        public bool Global { get; set; }
        #endregion
    }

    public class KgsPlayer
    {
        /// <summary>
        /// The <see cref="Downstream.Role"/>  of the player.
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// Upstream only. The name of the player. In incomplete challenges, leave any unassigned roles with no player.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Downstream only. The user who is filling this role. Will not be present in incomplete challenges.
        /// </summary>
        public User User { get; set; }
        // simul games: handicap, komi not supported
    }
}
