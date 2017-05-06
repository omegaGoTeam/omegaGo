using System.Collections.Generic;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// A game channel is a game currently being played, or it may be a challenge (an attempt to set up a custom game). There are several cases where game channels are described in a message, and they always contain these fields.
    /// <para>
    /// MESSAGE: This is always a downstream message or part of a downstream message. It should not be stored in <see cref="KgsData"/> directly. 
    /// </para>
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Datatypes.IRulesDescription" />
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Datatypes.IGameFlags" />
    public class GameChannel : IRulesDescription, IGameFlags
    {
        /// <summary>
        /// Channel ID of the game
        /// </summary>
        public int ChannelId { get; set; }
        /// <summary>
        /// Refers to <see cref="Datatypes.GameType"/>. 
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// The proposal that the challenge owner uploaded initially. For challenge game types only.
        /// </summary>
        public Proposal InitialProposal { get; set; }
        #region Rules Description
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
        #endregion
        /// <summary>
        /// The final score of the game. Missing if the game isn't over yet. Scores may be a floating point number, or a string. Numbers indicate the score difference (positive a black win, negative a white win). Strings may be UNKNOWN, UNFINISHED, NO_RESULT, B+RESIGN, W+RESIGN, B+FORFEIT, W+FORFEIT, B+TIME, or W+TIME.
        /// </summary>
        public object Score { get; set; }
        /// <summary>
        /// The current move number of the game.
        /// </summary>
        public int MoveNum { get; set; }
        /// <summary>
        /// The number of people observing the game. Missing if there are none.
        /// </summary>
        public int Observers { get; set; }
        /// <summary>
        /// The room that contains this game.
        /// </summary>
        public int RoomId { get; set; }
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
        /// <summary>
        /// Optional. If the game has a name, it is here.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// An object mapping role to user.
        /// </summary>
        public Dictionary<string, User> Players { get; set; }

        public override string ToString()
        {
            return ChannelId + " (" + GameType + ", in " + RoomId + ")";
        }
    }
}
