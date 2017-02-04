using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public abstract class KgsInterruptResponse : KgsResponse
    {
        public abstract void Process(KgsConnection connection);
    }
}
