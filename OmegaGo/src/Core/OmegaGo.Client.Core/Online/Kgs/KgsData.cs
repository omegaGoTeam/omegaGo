using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsData
    {
        private KgsConnection kgsConnection;
        public Dictionary<int, KgsRoom> Rooms { get; } = new Dictionary<int, KgsRoom>();
        public HashSet<int> JoinedChannels { get; } = new HashSet<int>();

        public KgsData(KgsConnection kgsConnection)
        {
            this.kgsConnection = kgsConnection;
        }

        public void SetRoomDescription(int channelId, string description)
        {
            if (!Rooms.ContainsKey(channelId))
            {
                Rooms[channelId] = new Kgs.KgsRoom(channelId);
            }
            Rooms[channelId].Description = description;
        }
        public void SetRoomName(int channelId, string name)
        {
            if (!Rooms.ContainsKey(channelId))
            {
                Rooms[channelId] = new Kgs.KgsRoom(channelId);
            }
            Rooms[channelId].Name = name;
        }
    }

    public class KgsRoom
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public KgsRoom(int id)
        {
            this.ChannelId = id;
        }
        public override string ToString()
        {
            return "[" + ChannelId + "] " + Name;
        }
    }
}
