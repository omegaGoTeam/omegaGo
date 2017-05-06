using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// Contains information downloaded from KGS. This information is continuously updated whenever we receive
    /// up-to-date information from KGS. More information is available in documentation for <see cref="KgsConnection.Data"/> 
    /// </summary>
    public class KgsData 
    {
        private KgsConnection kgsConnection;
        public List<KgsChallenge> OpenChallenges { get; } = new List<KgsChallenge>();
        public Dictionary<int, KgsRoom> Rooms { get; } = new Dictionary<int, KgsRoom>();
        public Dictionary<string, KgsUser> Users { get; } = new Dictionary<string, KgsUser>();
        public Dictionary<string, KgsGlobalGamesList> GlobalGameLists = new Dictionary<string, KgsGlobalGamesList>();
        private readonly Dictionary<int, KgsGame> joinedGames = new Dictionary<int, KgsGame>();
        public Dictionary<int, KgsGameContainer> Containers = new Dictionary<int, KgsGameContainer>();
        private HashSet<int> JoinedChannels { get; } = new HashSet<int>();

        public KgsData(KgsConnection kgsConnection)
        {
            this.kgsConnection = kgsConnection;
        }

        /// <summary>
        /// Gets a map associating channel IDs to their canonical channel data.
        /// </summary>
        public Dictionary<int, KgsChannel> Channels { get; } = new Dictionary<int, KgsChannel>();

        /// <summary>
        /// Gets those game containers that we have joined, i.e. we can see the games inside.
        /// </summary>
        public ObservableCollection<KgsGameContainer> GameContainers { get; } = new ObservableCollection<KgsGameContainer>();

        /// <summary>
        /// Gets all rooms, even those that we haven't joined.
        /// </summary>
        public ObservableCollection<KgsRoom> AllRooms { get; } = new ObservableCollection<KgsRoom>();

        // ------------------------- METHODS AND EVENTS  ----------------------------

        public delegate void KgsDataUpdate<in T>(T concernedItem);

        public event KgsDataUpdate<KgsChannel> ChannelJoined;
        public event KgsDataUpdate<KgsChannel> ChannelUnjoined;

        /// <summary>
        /// Unjoins the specified channel. If it wasn't joined, the events trigger anyway.
        /// </summary>
        /// <param name="channelId">Its ID.</param>
        public void UnjoinChannel(int channelId)
        {
            if (!Channels.ContainsKey(channelId))
            {
                return;
            }
            KgsChannel channel = Channels[channelId];
            channel.Joined = false;
            ChannelUnjoined?.Invoke(channel);

            // Old:
            JoinedChannels.Remove(channelId);
            kgsConnection.Events.RaiseUnjoin(channel);
        }


        /// <summary>
        /// Joins the specified channel. If it was already joined, events trigger anyway.
        /// This only affects <see cref="Channels"/>.
        /// </summary>
        /// <param name="channel">The channel.</param>
        private void JoinChannel(KgsChannel channel)
        {
            if (!Channels.ContainsKey(channel.ChannelId))
            {
                Channels.Add(channel.ChannelId, channel);
            }
            channel.Joined = true;
            ChannelJoined?.Invoke(channel);
        }

        /// <summary>
        /// Joins the specified global games list.
        /// </summary>
        /// <param name="channelId">The global games list channel ID.</param>
        /// <param name="containerType">Type of the container.</param>
        public void JoinGlobalChannel(int channelId, string containerType)
        {
            var globalGamesList = new KgsGlobalGamesList(channelId, containerType);
            JoinChannel(globalGamesList);
            GameContainers.Add(globalGamesList);

            // Old:
            GlobalGameLists.Add(containerType, globalGamesList);
            Containers[channelId] = globalGamesList;
        }

        /// <summary>
        /// Gets the room with the specified ID. If it doesn't exist, it is created.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        private KgsRoom EnsureRoomExists(int channelId)
        {
            if (Channels.ContainsKey(channelId))
            {
            }
            else
            {
                var room = new KgsRoom(channelId);
                Channels[channelId] = room;
                AllRooms.Add(room);

                // Old:
                Rooms[channelId] = room;
                Containers[channelId] = room;
            }
            return Channels[channelId] as KgsRoom;
        }

        /// <summary>
        /// Joins the specified room.
        /// </summary>
        /// <param name="channelId">The room's channelId.</param>
        public void JoinRoom(int channelId)
        {
            var room = EnsureRoomExists(channelId);
            JoinChannel(room);
            GameContainers.Add(room);
        }



        // ------------------------- BEFORE OVERHAUL ----------------------------


        internal IEnumerable<KgsGame> Games
        {
            get
            {
                foreach (var game in joinedGames.Values)
                {
                    yield return game;
                }
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
            if (Channels.ContainsKey(channelId))
            {
                Channels[channelId].Joined = true;
                JoinedChannels.Add(channelId);
            } 
        }
        public void JoinChallenge(int channelId)
        {
            if (!Channels.ContainsKey(channelId))
            {
                Channels.Add(channelId, new KgsChannel()
                {
                    ChannelId = channelId
                });
            }
            JoinChannel(channelId);
        }
        public void AddUserToChannel(int channelId, User user)
        {
            EnsureUserExists(user);
            Channels[channelId].Users.Add(Users[user.Name]);
        }
        public void EnsureUserExists(User user)
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
        public void JoinGame(KgsGame ongame)
        {
            Channels[ongame.Info.ChannelId] = new KgsGameChannel(ongame.Info.ChannelId);
            Channels[ongame.Info.ChannelId].Joined = true;
            JoinedChannels.Add(ongame.Info.ChannelId);
            joinedGames.Add(ongame.Info.ChannelId, ongame);
        }
        public KgsGame GetGame(int channelId)
        {
            return joinedGames.ContainsKey(channelId) ? joinedGames[channelId] : null;
        }
        public bool IsJoined(int channelId)
        {
            return Channels.ContainsKey(channelId) && Channels[channelId].Joined;
        }
    }
}
