using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    public abstract class KgsGameContainer : KgsChannel
    {
        public string Name { get; set; }
        private readonly List<KgsTrueGameChannel> Games = new List<KgsTrueGameChannel>();
        private readonly List<KgsChallenge> Challenges = new List<KgsChallenge>();

        public void AddGame(GameChannel channel, KgsConnection connection)
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
            Games.RemoveAll(kgi => kgi.ChannelId == gameId);
            Challenges.RemoveAll(kgi => kgi.ChannelId == gameId);
        }
        public override string ToString()
        {
            return (Joined ? "[JOINED] " : "") + "[" + ChannelId + "] " + Name;
        }

        public IEnumerable<KgsTrueGameChannel> GetGames()
        {
            return Games;
        }
        public IEnumerable<KgsChallenge> GetChallenges()
        {
            return Challenges;
        }
        public IEnumerable<KgsGameChannel> GetAllChannels()
        {
            return Games.Concat<KgsGameChannel>(Challenges);
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
                AddGame(g, connection);
            }
        }

       
    }
}