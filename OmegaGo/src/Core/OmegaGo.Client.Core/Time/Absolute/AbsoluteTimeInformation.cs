using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.Absolute
{
    class AbsoluteTimeInformation : TimeInformation
    {
        public TimeSpan RemainingMainTime { get; }
        public AbsoluteTimeInformation(TimeSpan remainingTime)
        {
            RemainingMainTime = remainingTime;
        }

        public override string MainText => RemainingMainTime.ToString(@"mm\:ss");
        public override string SubText => "No overtime available!";
        public override TimeControlStyle Style => TimeControlStyle.Absolute;
    }
}
