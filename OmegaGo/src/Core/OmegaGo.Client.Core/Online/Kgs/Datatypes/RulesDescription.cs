namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public class RulesDescription : IRulesDescription
    {
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

        public const string RulesJapanese = "japanese";
        public const string RulesChinese = "chinese";
        public const string RulesAga = "aga";
        public const string RulesNewZealand = "new_zealand";

        public const string TimeSystemNone = "none";
        public const string TimeSystemAbsolute = "absolute";
        public const string TimeSystemJapanese = "byo_yomi";
        public const string TimeSystemCanadian = "canadian";
    }
}