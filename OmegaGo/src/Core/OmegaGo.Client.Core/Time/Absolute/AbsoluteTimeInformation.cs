using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;

namespace OmegaGo.Core.Time.Absolute
{
    public class AbsoluteTimeInformation : TimeInformation
    {
        public TimeSpan RemainingMainTime { get; }
        public AbsoluteTimeInformation(TimeSpan remainingTime)
        {
            RemainingMainTime = remainingTime;
        }

        public override string MainText
            => RemainingMainTime > TimeSpan.Zero ? RemainingMainTime.ToCountdownString() : "00:00";
        public override string SubText => "No overtime available!";
        public override TimeControlStyle Style => TimeControlStyle.Absolute;
    }
}
