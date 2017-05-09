using System.Collections.Generic;

namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// A game summary is a permanent record of a game. Game channels come and go as games are loaded and closed, but a game summary stays for 6 months on the server (and forever in the online archives).
    /// </summary>
    public class GameSummary
    {
        /// <summary>
        /// The size of the board from this game.
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// The time stamp of when this game was started. This is also used as a serverwide ID for the game; no two games will ever have the same timestamp, and the time stamp is used to refer to the game summary. It looks a little like this: 2017-02-05T09:24Z.
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// One of demonstration, review, rengo_review, teaching, simul, rengo, free, ranked, or tournament.
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// Optional. The score of the game. Not present if the game hasn't ended yet. TODO object won't deserialize
        /// </summary>
        public object Score { get; set; }
        /// <summary>
        /// Optional. The revision is used when downloading an SGF file; every SGF file has the path /games/year/month/day/white-black-revision.sgf. If revision is missing here, then it is not in the URL either.
        /// </summary>
        public int Revision { get; set; }
        /// <summary>
        /// An object mapping roles to user objects, telling who was in the game. The user objects in this map are snapshots of the players at the time the game started; you should ignore all the flags in them other than the rank information (which may not be the current rank of the player).

        /// </summary>
        // ReSharper disable once CollectionNeverUpdated.Global
        public Dictionary<string, User> Players { get; set; }
        /// <summary>
        /// Only present in tag archives. The tag associated with the game summary. TODO object won't deserialize
        /// </summary>
        public object Tag { get; set; }
        /// <summary>
        /// Optional. If set, this is a private game. If the flag is missing, that is the same as false.
        /// </summary>
        public bool Private { get; set; }
        /// <summary>
        /// Optional. If set, the game is currently in play. If the flag is missing, that is the same as false.
        /// </summary>
        public bool InPlay { get; set; }
    }
}