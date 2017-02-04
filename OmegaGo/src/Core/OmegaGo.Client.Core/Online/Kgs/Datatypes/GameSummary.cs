using System.Collections.Generic;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public class GameSummary
    {
        public int Size { get; set; }
        public string Timestamp { get; set; }
        public string GameType { get; set; }
        public object Score { get; set; }
        public object Revision { get; set; }
        public Dictionary<string, User> Players { get; set; }
        public object Tag { get; set; }
        public bool Private { get; set; }
        public bool InPlay { get; set; }
    }
}