using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    abstract class KgsInterruptChannelMessage : KgsInterruptResponse
    {
        public int ChannelId { get; set; }
    }
}
