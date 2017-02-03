using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class JoinComplete : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
            connection.Data.JoinedChannels.Add(this.ChannelId);
        }
    }
}
