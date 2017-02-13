﻿namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// A proposal is an offer to start a game. It includes the rules and may include some or all of the players. If all the players needed are present in the proposal, then it is called a "complete" proposal.
    /// 
    ///Note: The first player in a proposal must always be the challenge owner.When manipulating a proposal, in general leave the player list in the order it started; if you want to change the roles for each player, change the role field and not the player field. You will get strange results if you don't do this.
    /// </summary>
    public class Proposal : IGameFlags
    {
        /// <summary>
        /// The <see cref="Kgs.Downstream.GameType"/>  of the game.
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// The rules for the game.
        /// </summary>
        public RulesDescription Rules { get; set; }
        /// <summary>
        /// If set, that means nigiri will be used to determine who plays white.
        /// </summary>
        public bool Nigiri { get; set; }
        /// <summary>
        /// A list of players. All roles for this game type must be present.
        /// </summary>
        public KgsPlayer[] Players { get; set; }
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
}