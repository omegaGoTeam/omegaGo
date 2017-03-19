using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.None
{
    class NoTimeInformation : TimeInformation
    {
        public override string MainText => "";
        public override string SubText => "No time limit";
        public override TimeControlStyle Style => TimeControlStyle.None;
    }
}
