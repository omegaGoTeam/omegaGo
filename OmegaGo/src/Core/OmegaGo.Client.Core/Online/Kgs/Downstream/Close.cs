using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class Close : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
            connection.Data.UnjoinChannel(ChannelId);
            // TODO (future expansion): If this is a room, it can be removed from the list of rooms.
            // However, it is also possible that a different downstream message is responsible for this.
            // Research needed.
        }
    }
}
