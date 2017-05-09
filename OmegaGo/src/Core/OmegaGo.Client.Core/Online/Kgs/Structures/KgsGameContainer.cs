using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Base class for game channels that can contain other game channels.
    /// Those may either be a room (<see cref="KgsRoom"/>) or a global list (<see cref="KgsGlobalGamesList"/>).
    /// These can be browsed for challenges and games that may be then be joined.  
    /// Only those games and challenges that omegaGo understands are loaded.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Structures.KgsChannel" />
    public abstract class KgsGameContainer : KgsChannel
    {
        private string _name;

        /// <summary>
        /// Gets or sets the name of the room or global list.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<KgsGameChannel> AllChannelsCollection { get; } = new ObservableCollection<KgsGameChannel>();

        public ObservableCollection<KgsChallenge> Challenges { get; } = new ObservableCollection<KgsChallenge>();

        public ObservableCollection<KgsTrueGameChannel> Games { get; } = new ObservableCollection<KgsTrueGameChannel>();

        /// <summary>
        /// Gets all non-challenge games in this container.
        /// </summary>
        public IEnumerable<KgsTrueGameChannel> GetGames()
        {
            return Games;
        }

        /// <summary>
        /// Gets all challenges in this container.
        /// </summary>
        public IEnumerable<KgsChallenge> GetChallenges()
        {
            return Challenges;
        }

        /// <summary>
        /// Gets all channels (games and challenges) in this container.
        /// </summary>
        public IEnumerable<KgsGameChannel> GetAllChannels()
        {
            return Games.Concat<KgsGameChannel>(Challenges);
        }
        
        public void AddChannel(GameChannel channel, KgsConnection connection)
        {
            var kChallenge = KgsChallenge.FromChannel(channel);
            if (kChallenge != null)
            {
                connection.Data.EnsureChannelExists(kChallenge);
                Challenges.Add(kChallenge);
                AllChannelsCollection.Add(kChallenge);
                return;
            }
            var kGame = KgsTrueGameChannel.FromChannel(channel);
            if (kGame != null)
            {
                connection.Data.EnsureChannelExists(kGame);
                Games.Add(kGame);
                AllChannelsCollection.Add(kGame);
                return;
            }
        }

        public void RemoveGame(int gameId)
        {
            var removeWhat = AllChannelsCollection.FirstOrDefault(kgc => kgc.ChannelId == gameId);
            if (removeWhat != null)
            {
                AllChannelsCollection.Remove(removeWhat);
            }

            // Old:
            Games.RemoveAll(kgi => kgi.ChannelId == gameId);
            Challenges.RemoveAll(kgi => kgi.ChannelId == gameId);
        }

        public void UpdateGames(GameChannel[] games, KgsConnection connection)
        {
            foreach (var g in games)
            {
                var existingGame = AllChannelsCollection.FirstOrDefault(kgc => kgc.ChannelId == g.ChannelId);
                if (existingGame != null)
                {
                    existingGame.UpdateFrom(g);
                }
                else
                {
                    AddChannel(g, connection);
                }
            }
        }

        public override string ToString()
        {
            return (Joined ? "[JOINED] " : "") + "[" + ChannelId + "] " + Name;
        }
    }
}