using System.Collections.Generic;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
    public abstract class KgsGameContainer : KgsChannel
    {
        public string Name { get; set; }
        private readonly List<KgsGameInfo> Games = new List<KgsGameInfo>();

        public void AddGame(GameChannel channel)
        {
            var kinfo = KgsGameInfo.FromChannel(channel);
            if (kinfo == null)
            {
                // This game is not supported by our client.
                return;
            }
            Games.Add(kinfo);
        }

        public void RemoveGame(int gameId)
        {
            Games.RemoveAll(kgi => kgi.ChannelId == gameId);
        }
        public override string ToString()
        {
            return (Joined ? "[JOINED] " : "") + "[" + ChannelId + "] " + Name;
        }

        public IEnumerable<KgsGameInfo> GetGames()
        {
            return Games;
        }

        public void UpdateGames(GameChannel[] games)
        {
            foreach (var g in games)
            {
                KgsGameInfo equiv = Games.Find(kgs => kgs.ChannelId == g.ChannelId);
                if (equiv == null)
                {
                }
                else
                {
                    Games.Remove(equiv);
                }
                // TODO update instead of replace
                AddGame(g);
            }
        }
    }
}