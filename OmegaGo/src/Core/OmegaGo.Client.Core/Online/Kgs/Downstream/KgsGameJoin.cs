using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public class KgsGameJoin : KgsGameState
    {
        public User[] Users { get; set; }
        public GameSummary GameSummary { get; set; }
        public SgfEvent[] SgfEvents { get; set; }

        /// <summary>
        /// Gets the rules, if they're supplied with the Game Join message. If not, then returns null.
        /// I don't know when null is returned, but sometimes it apparently is.
        /// </summary>
        public RulesDescription Rules
        {
            get
            {
                foreach (var ev in SgfEvents)
                {
                    if (ev.Props != null)
                    {
                        foreach (var prop in ev.Props)
                        {
                            if (prop.Name == "RULES")
                            {
                                return prop;
                            }
                        }
                    }
                }
                return null;
            }
        }
        public override void Process(KgsConnection connection)
        {
            KgsGameInfo info = KgsGameInfo.FromGameJoin(this);
            if (info == null) return;
            var channel = connection.Data.GetChannel(this.ChannelId);
            if (channel == null)
            {
                channel = connection.Data.CreateGame(KgsTrueGameChannel.FromGameInfo(info, this.ChannelId));
            }


            var blackPlayer = new KgsPlayerBuilder(Game.StoneColor.Black, connection)
                .Name(info.Black.Name)
                .Rank(info.Black.Rank)
                .Build();
            if (info.Black.Name == connection.Username)
            {
                blackPlayer =
                    new HumanPlayerBuilder(Game.StoneColor.Black).Name(info.Black.Name).Rank(info.Black.Rank).Build();
            }
            var whitePlayer = new KgsPlayerBuilder(Game.StoneColor.White, connection)
                .Name(info.White.Name)
                .Rank(info.White.Rank)
                .Build();
            if (info.White.Name == connection.Username)
            {
                whitePlayer =
                    new HumanPlayerBuilder(Game.StoneColor.White).Name(info.White.Name).Rank(info.White.Rank).Build();
            }
            var ongame = new KgsGameBuilder(info, connection)
                .BlackPlayer(blackPlayer)
                .WhitePlayer(whitePlayer)
                .Build();
            connection.Data.JoinGame(ongame, channel as KgsTrueGameChannel);
            foreach (var ev in SgfEvents)
            {
                ev.ExecuteAsIncoming(connection, ongame);
            }
            connection.Events.RaiseGameJoined(ongame);
        }
    }
}
