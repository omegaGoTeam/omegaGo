using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private KgsConnection _kgsConnection;
        private Dictionary<string, KgsUser> _users { get; } = new Dictionary<string, KgsUser>();
        private readonly Dictionary<int, KgsGame> _joinedGames = new Dictionary<int, KgsGame>();

        public KgsData(KgsConnection kgsConnection)
        {
            this._kgsConnection = kgsConnection;
        }

        /// <summary>
        /// Gets a map associating channel IDs to their canonical channel data.
        /// </summary>
        private Dictionary<int, KgsChannel> Channels { get; } = new Dictionary<int, KgsChannel>();

        /// <summary>
        /// Gets those game containers that we have joined, i.e. we can see the games inside.
        /// </summary>
        public ObservableCollection<KgsGameContainer> GameContainers { get; } = new ObservableCollection<KgsGameContainer>();

        /// <summary>
        /// Gets all rooms, even those that we haven't joined.
        /// </summary>
        public ObservableCollection<KgsRoom> AllRooms { get; } = new ObservableCollection<KgsRoom>();



        /// <summary>
        /// Gets the channel with the specified channel ID, if it exists and if it has the appropriate type.
        /// Otherwise, it returns null, but that means there is an error
        /// somewhere in our code or the protocol.
        /// </summary>
        /// <param name="channelId">The channel identifier of a channel of a particular kind.</param>
        /// <returns></returns>
        public T GetChannel<T>(int channelId) where T : KgsChannel
        {
            if (Channels.ContainsKey(channelId))
            {
                T t = Channels[channelId] as T;
                if (t == null)
                {
                    Debug.WriteLine("This isn't supposed to happen - bad cast.");
                }
                return t;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the channel with the specified ID, if it exists. Otherwise, it returns null, but that means there is an error
        /// somewhere in our code or the protocol.
        /// </summary>
        /// <param name="channelId">The channel identifier of the channel to get.</param>
        /// <returns></returns>
        public KgsChannel GetChannel(int channelId)
        {
            if (Channels.ContainsKey(channelId))
            {
                return Channels[channelId];
            }
            else
            {
                return null;
            }
        }

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
            _kgsConnection.Events.RaiseUnjoin(channel);
            if (channel is KgsGameContainer)
            {
                KgsGameContainer container = channel as KgsGameContainer;
                GameContainers.Remove(container);
            }
            if (channel is KgsTrueGameChannel)
            {
                if (_joinedGames.ContainsKey(channel.ChannelId))
                {
                    _joinedGames.Remove(channel.ChannelId);
                }
            }
            SomethingChanged();

            // Old:
            _kgsConnection.Events.RaiseChannelUnjoined(channel);
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
            _kgsConnection.Events.RaiseChannelJoined(channel);
            SomethingChanged();
        }

        public void EnsureChannelExists(KgsChannel channel)
        {
            if (!Channels.ContainsKey(channel.ChannelId))
            {
                Channels.Add(channel.ChannelId, channel);
            }
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
            }
            return Channels[channelId] as KgsRoom;
        }

        /// <summary>
        /// Joins the specified room. If the room isn't loaded in Data yet, it is loaded now.
        /// </summary>
        /// <param name="channelId">The room's channelId.</param>
        public void JoinRoom(int channelId)
        {
            var room = EnsureRoomExists(channelId);
            JoinChannel(room);
            GameContainers.Add(room);
        }
        /// <summary>
        /// Joins the specified challenge. If it isn't loaded in Data yet, it is loaded now.
        /// </summary>
        /// <param name="challenge">The challenge to join.</param>
        public void JoinChallenge(KgsChallenge challenge)
        {
            bool wasJoined = challenge.Joined;
            JoinChannel(challenge);
            if (!wasJoined)
            {
                this._kgsConnection.Events.RaiseChallengeJoined(challenge);
            }
        }

        /// <summary>
        /// Sets the description of a room. If the room isn't loaded in Data yet, it is loaded now.
        /// </summary>
        /// <param name="channelId">The channel identifier of the room.</param>
        /// <param name="description">The description.</param>
        public void SetRoomDescription(int channelId, string description)
        {
            var room = EnsureRoomExists(channelId);
            room.Description = description;
        }
        /// <summary>
        /// Sets the name of a room. If the room isn't loaded in Data yet, it is loaded now.
        /// </summary>
        /// <param name="channelId">The channel identifier of the room.</param>
        /// <param name="name">The name.</param>
        public void SetRoomName(int channelId, string name)
        {
            var room = EnsureRoomExists(channelId);
            room.Name = name;
        }

        /// <summary>
        /// Gets the running game associated with a channel ID. If we haven't opened that game yet, this returns null.
        /// </summary>
        /// <param name="channelId">The channel identifier of the game.</param>
        public KgsGame GetGame(int channelId)
        {
            return _joinedGames.ContainsKey(channelId) ? _joinedGames[channelId] : null;
        }

        /// <summary>
        /// Determines whether the logged-in used is joined in the channel with the specified ID.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        public bool IsJoined(int channelId)
        {
            return Channels.ContainsKey(channelId) && Channels[channelId].Joined;
        }

        /// <summary>
        /// Joins the game channel and opens the given game so it can be referenced by other messages.
        /// </summary>
        /// <param name="ongame">The KGS game that will be referenced later.</param>
        /// <param name="kgsTrueGameChannel">The game channel that we are joining.</param>
        public void JoinGame(KgsGame ongame, KgsTrueGameChannel kgsTrueGameChannel)
        {
            _joinedGames[ongame.Info.ChannelId] = ongame;
            JoinChannel(kgsTrueGameChannel);
        }


        /// <summary>
        /// Gets all open KGS games. This may include games that have already finished if they were not unjoined.
        /// </summary>
        internal IEnumerable<KgsGame> Games
        {
            get
            {
                foreach (var game in _joinedGames.Values)
                {
                    yield return game;
                }
            }
        }     
        
        /// <summary>
        /// Unjoins all joined channels. This is called after a disconnect to get rid of challenge tabs that are no longer relevant.
        /// </summary>
        public void UnjoinEverything()
        {
            foreach (var kvp in Channels)
            {
                if (kvp.Value.Joined)
                {
                    UnjoinChannel(kvp.Value.ChannelId);
                }
            }
        }

        /// <summary>
        /// Adds the user to a channel. This is used only by the lobby to display the users in a room.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="user">The user.</param>
        public void AddUserToChannel(int channelId, User user)
        {
            EnsureUserExists(user);
            Channels[channelId].Users.Add(_users[user.Name]);
        }

        /// <summary>
        /// If we already have information about a user, returns that canonical information. If not, we create it.
        /// </summary>
        /// <param name="user">The user.</param>
        public KgsUser EnsureUserExists(User user)
        {
            if (!_users.ContainsKey(user.Name))
            {
                var nUser = new KgsUser();
                nUser.CopyDataFrom(user);
                _users[user.Name] = nUser;
            }
            return _users[user.Name];
        }

        /// <summary>
        /// Removes a user from a channel. This is used only by the lobby to display the users in a room.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="user">The user.</param>
        public void RemoveUserFromChannel(int channelId, User user)
        {
            // We can't user the argument directly because it's a differente reference.
            var usr = Channels[channelId].Users.FirstOrDefault(kgsUser => kgsUser.Name == user.Name);
            if (usr != null)
            {
                Channels[channelId].Users.Remove(usr);
            }
        }

        /// <summary>
        /// Adds a game to the list of channels we know but haven't joined yet, and returns it.
        /// </summary>
        /// <param name="game">The game.</param>
        public KgsTrueGameChannel CreateGame(KgsTrueGameChannel game)
        {
            Channels[game.ChannelId] = game;
            return game;
        }
        private void SomethingChanged()
        {
            this._kgsConnection.Events.RaiseSomethingChanged();
        }

    }
}
