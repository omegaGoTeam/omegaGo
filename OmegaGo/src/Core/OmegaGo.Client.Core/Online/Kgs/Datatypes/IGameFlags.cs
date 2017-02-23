namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// Because multiple data types may contain flags, to ensure that we don't forget any, all
    /// of those data types must implement this interface.
    /// </summary>
    public interface IGameFlags
    {
        #region Flags
        // This region may be copied to other messages that make use of flags.
        /// <summary>
        /// If set, it means that the game has been scored.
        /// </summary>
        bool Over { get; set; }
        /// <summary>
        /// The game cannot continue because the player whose turn it is has left.
        /// </summary>
        bool Adjourned { get; set; }
        /// <summary>
        /// Only users specified by the owner are allowed in.
        /// </summary>
        bool Private { get; set; }
        /// <summary>
        /// Only KGS Plus subscribers are allowed in.
        /// </summary>
        bool Subscribers { get; set; }
        /// <summary>
        /// This game is a server event, and should appear at the top of game lists.
        /// </summary>
        bool Event { get; set; }
        /// <summary>
        /// This game was created by uploading an SGF file.
        /// </summary>
        bool Uploaded { get; set; }
        /// <summary>
        /// This game includes a live audio track.
        /// </summary>
        bool Audio { get; set; }
        /// <summary>
        /// The game is paused. Tournament games are paused when they are first created, to give players time to join before the clocks start.
        /// </summary>
        bool Paused { get; set; }
        /// <summary>
        /// This game has a name (most games are named after the players involved). In some cases, instead of seeing this flag when it is set, a text field name will appear instead.
        /// </summary>
        bool Named { get; set; }
        /// <summary>
        /// This game has been saved to the KGS archives. Most games are saved automatically, but demonstration and review games must be saved by setting this flag.
        /// </summary>
        bool Saved { get; set; }
        /// <summary>
        /// This game may appear on the open or active game lists.
        /// </summary>
        bool Global { get; set; }
        #endregion
    }
}