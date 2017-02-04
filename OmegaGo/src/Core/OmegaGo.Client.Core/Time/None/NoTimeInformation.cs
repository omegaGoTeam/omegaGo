using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.None
{
    class NoTimeInformation : TimeInformation
    {
        public override string MainText => "No time limit";
        public override string SubText => "Relax...";
        public override TimeControlStyle Style => TimeControlStyle.None;
    }
}
