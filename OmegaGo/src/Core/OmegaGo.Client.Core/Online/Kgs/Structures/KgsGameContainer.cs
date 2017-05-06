using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private readonly List<KgsTrueGameChannel> Games = new List<KgsTrueGameChannel>();
        private readonly List<KgsChallenge> Challenges = new List<KgsChallenge>();


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

        // TODO KGS OVERHAUL THIS
        public void AddChannel(GameChannel channel, KgsConnection connection)
        {
            var kinfo = KgsGameInfo.FromChannel(channel, connection);
            if (kinfo != null)
            {
                Games.Add(new KgsTrueGameChannel(channel, connection));
                return;
            }
            var kchallenge = KgsChallenge.FromChannel(channel, connection);
            if (kchallenge != null)
            {
                Challenges.Add(kchallenge);
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
                KgsTrueGameChannel equiv = Games.Find(kgs => kgs.ChannelId == g.ChannelId);
                if (equiv == null)
                {
                }
                else
                {
                    Games.Remove(equiv);
                }
                // TODO Petr : update instead of replace
                AddChannel(g, connection);
            }
        }

        public override string ToString()
        {
            return (Joined ? "[JOINED] " : "") + "[" + ChannelId + "] " + Name;
        }
    }
}