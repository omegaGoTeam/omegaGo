using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Online;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class GameJoin : GameState
    {
        public User[] Users { get; set; }
        public GameSummary GameSummary { get; set; }
        public SgfEvent[] SgfEvents { get; set; }
        public override void Process(KgsConnection connection)
        {
            KgsGameInfo info = null;
            var ongame = new KgsGameBuilder(info)
                .BlackPlayer(null)
                .WhitePlayer(null)
                .Build();
            connection.Events.RaiseGameJoined(ongame);
        }
    }

    internal class SgfEvent
    {
    }

    internal class GameSummary
    {
        // TODO
    }
}
