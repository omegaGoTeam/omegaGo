using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class GameUpdate : KgsInterruptChannelMessage
    {
        public SgfEvent[] SgfEvents { get; set; }
        public override void Process(KgsConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
