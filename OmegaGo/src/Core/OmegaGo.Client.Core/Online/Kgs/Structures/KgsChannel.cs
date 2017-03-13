using System.Collections.Generic;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    public class KgsChannel
    {
        public int ChannelId { get; set; }
        public bool Joined { get; set; }
        public HashSet<KgsUser> Users { get; } = new HashSet<KgsUser>();

        public override string ToString()
        {
            return "Channel " + ChannelId;
        }
    }
}