using System.Collections.Generic;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsChannel
    {
        public int ChannelId { get; set; }
        public bool Joined { get; set; }
        public HashSet<KgsUser> Users { get; } = new HashSet<KgsUser>();
    }
}