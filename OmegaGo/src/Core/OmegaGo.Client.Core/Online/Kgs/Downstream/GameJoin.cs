using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class GameJoin : GameState
    {
        public User[] Users { get; set; }
        public GameSummary GameSummary { get; set; }
        public SgfEvent[] SgfEvents { get; set; }
        public override void Process(KgsConnection connection)
        {
            throw new NotImplementedException();
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
