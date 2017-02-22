using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class GameUpdate : KgsInterruptChannelMessage
    {
        public SgfEvent[] SgfEvents { get; set; }
        public override void Process(KgsConnection connection)
        {
            foreach (var ev in SgfEvents)
            {
                ev.ExecuteAsIncoming(connection, connection.Data.GetGame(ChannelId));
            }
        }
    }
}
