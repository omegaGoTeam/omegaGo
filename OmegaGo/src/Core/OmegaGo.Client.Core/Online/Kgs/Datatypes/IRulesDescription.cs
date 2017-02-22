namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// Because multiple data types may contain anything the rules description may contain,
    /// all those types implement this interface so we don't forget anything.
    /// </summary>
    public interface IRulesDescription
    {
        #region Rules Description
        /// <summary>
        /// The size of the board. 2 through 38 are supported by KGS.
        /// </summary>
        int Size { get; set; }
        /// <summary>
        /// One of japanese, chinese, aga, or new_zealand.
        /// </summary>
        string Rules { get; set; }
        /// <summary>
        /// Not present for handicap 0. At most 9.
        /// </summary>
        int Handicap { get; set; }
        /// <summary>
        /// A floating point number. Must be a multiple of 0.5, -100.0 through +100.0.
        /// </summary>
        float Komi { get; set; }
        /// <summary>
        /// One of none, absolute, byo_yomi, or canadian.
        /// </summary>
        string TimeSystem { get; set; }
        /// <summary>
        /// Time for the main time period, in seconds. Not needed when your time system is none. Up to 2147483 seconds.
        /// </summary>
        int MainTime { get; set; }
        /// <summary>
        /// Time for each byo-yomi period. Only needed for Canadian and Byo-Yomi time systems. Up to 2147483 seconds.
        /// </summary>
        int ByoYomiTime { get; set; }
        /// <summary>
        /// Number of byo-yomi periods. Only needed for byo-yomi time system. At most 255.
        /// </summary>
        int ByoYomiPeriods { get; set; }
        /// <summary>
        /// Number of stones per byo-yomi period. Only needed for Canadian time system. At most 255.
        /// </summary>
        int ByoYomiStones { get; set; }
        #endregion
    }
}