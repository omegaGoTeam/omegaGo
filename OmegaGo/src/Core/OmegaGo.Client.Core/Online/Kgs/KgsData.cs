﻿using System;
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
        public Dictionary<int, KgsChannel> Channels { get; } = new Dictionary<int, KgsChannel>();
        public Dictionary<string, KgsUser> Users { get; } = new Dictionary<string, KgsUser>();
        private HashSet<int> JoinedChannels { get; } = new HashSet<int>();
        public AutomatchPrefs AutomatchPreferences = null;
        public KgsData(KgsConnection kgsConnection)
        {
            this.kgsConnection = kgsConnection;
        }
        private void EnsureRoomExists(int channel)
        {
            if (!Rooms.ContainsKey(channel))
            {
                var room = new KgsRoom(channel);
                Rooms[channel] = room;
                Channels[channel] = room;
            }
        }


        public void SetRoomDescription(int channelId, string description)
        {
            EnsureRoomExists(channelId);
            Rooms[channelId].Description = description;
        }
        public void SetRoomName(int channelId, string name)
        {
            EnsureRoomExists(channelId);
            Rooms[channelId].Name = name;
        }
        public void JoinChannel(int channelId)
        {
            Channels[channelId].Joined = true;
            JoinedChannels.Add(channelId);
        }
        public void JoinRoom(int channelId)
        {
            EnsureRoomExists(channelId);
            JoinChannel(channelId);

        }

        public void AddUserToChannel(int channelId, User user)
        {
            EnsureUserExists(user);
            Channels[channelId].Users.Add(Users[user.Name]);
        }

        private void EnsureUserExists(User user)
        {
            if (!Users.ContainsKey(user.Name))
            {
                var nUser = new Kgs.KgsUser();
                nUser.CopyDataFrom(user);
                Users[user.Name] = nUser;
            }
        }

        public void RemoveUserFromChannel(int channelId, User user)
        {
            Channels[channelId].Users.RemoveWhere(kgsUser => kgsUser.Name == user.Name);
        }

        public void UnjoinChannel(int channelId)
        {
            Channels[channelId].Joined = false;
            JoinedChannels.Remove(channelId);
        }
    }

    public class KgsChannel
    {
        public int ChannelId { get; set; }
        public bool Joined { get; set; }
        public HashSet<KgsUser> Users { get; } = new HashSet<KgsUser>();
    }

    public class KgsUser : User
    {
        public void CopyDataFrom(User user)
        {
            this.Name = user.Name;
            this.Flags = user.Flags;
            this.Rank = user.Rank;
            this.AuthLevel = user.AuthLevel;
        }
    }
    public class KgsRoom : KgsChannel
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
}
