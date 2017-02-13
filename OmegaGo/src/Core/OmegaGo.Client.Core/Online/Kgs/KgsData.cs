﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// Contains information downloaded from KGS. This information is continuously updated whenever we receive
    /// up-to-date information from KGS.
    /// </summary>
    public class KgsData
    {
        private KgsConnection kgsConnection;
        public Dictionary<int, KgsRoom> Rooms { get; } = new Dictionary<int, KgsRoom>();
        public Dictionary<int, KgsChannel> Channels { get; } = new Dictionary<int, KgsChannel>();
        public Dictionary<string, KgsUser> Users { get; } = new Dictionary<string, KgsUser>();
        public Dictionary<string, KgsGlobalGamesList> GlobalGameLists = new Dictionary<string, KgsGlobalGamesList>();
        private readonly Dictionary<int, KgsGame> joinedGames = new Dictionary<int, KgsGame>();
        public Dictionary<int, KgsGameContainer> Containers = new Dictionary<int, KgsGameContainer>();
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
                Containers[channel] = room;
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
        public void JoinGlobalChannel(int channelId, string containerType)
        {
            var nth = new KgsGlobalGamesList(channelId, containerType);
            GlobalGameLists.Add(containerType, nth);
            Containers[channelId] = nth;
            Channels[channelId] = nth;
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
                var nUser = new KgsUser();
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
        public void JoinGame(KgsGame ongame)
        {
            Channels[ongame.Metadata.ChannelId] = new KgsGameChannel(ongame.Metadata.ChannelId);
            JoinedChannels.Add(ongame.Metadata.ChannelId);
            joinedGames.Add(ongame.Metadata.ChannelId, ongame);
        }
        public KgsGame GetGame(int channelId)
        {
            return joinedGames[channelId];
        }
    }
}