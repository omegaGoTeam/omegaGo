using System.Collections.Generic;

namespace OmegaGo.Core.Online.Kgs
{
    public abstract class KgsGameContainer : KgsChannel
    {
        
    }
    public class KgsRoom : KgsGameContainer
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public KgsRoom(int id)
        {
            this.ChannelId = id;
        }
        public override string ToString()
        {
            return (Joined ? "[JOINED] " : "") + "[" + ChannelId + "] " + Name;
        }
    }

    public class KgsGlobalGamesList : KgsGameContainer
    {
        public KgsGlobalGamesList(int id)
        {
            this.ChannelId = id;
        }
    }
}