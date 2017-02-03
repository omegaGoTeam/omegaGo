using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsData
    {
        private KgsConnection kgsConnection;
        public Dictionary<int, KgsRoom> Rooms { get; } = new Dictionary<int, KgsRoom>();
        private HashSet<int> JoinedChannels { get; } = new HashSet<int>();
        public AutomatchPrefs AutomatchPreferences = null;

        public void JoinRoom(int channelId)
        {
            JoinedChannels.Add(channelId);
            if (!Rooms.ContainsKey(channelId))
            {
                Rooms[channelId] = new Kgs.KgsRoom(channelId);
            }
            Rooms[channelId].Joined = true;

        }
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

        public void JoinSomething(int channelId)
        {
            if (Rooms.ContainsKey(channelId))
                JoinRoom(channelId);
        }
    }

    public class KgsRoom
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Joined { get; set; }

        public KgsRoom(int id)
        {
            this.ChannelId = id;
        }
        public override string ToString()
        {
            return (Joined ? "[JOINED] " : "") + "[" + ChannelId + "] " + Name;
        }
    }
}
