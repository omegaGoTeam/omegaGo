using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class JoinComplete : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
        }
    }
}
