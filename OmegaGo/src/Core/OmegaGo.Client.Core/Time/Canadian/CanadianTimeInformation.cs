using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.Canadian
{
    class CanadianTimeInformation : TimeInformation
    {
        private TimeSpan mainTimeLeft;
        private TimeSpan periodTimeLeft;
        private int periodStonesLeft;

        public bool IsViolating()
        {
            return mainTimeLeft <= TimeSpan.Zero && periodTimeLeft <= TimeSpan.Zero;
        }

        public CanadianTimeInformation(TimeSpan mainTimeLeft, TimeSpan periodTimeLeft, int periodStonesLeft)
        {
            this.mainTimeLeft = mainTimeLeft;
            this.periodTimeLeft = periodTimeLeft;
            this.periodStonesLeft = periodStonesLeft;
        }

        public override string MainText {
            get {
                if (mainTimeLeft > TimeSpan.Zero)
                {
                    return mainTimeLeft.ToString(@"mm\:ss");
                }
                return periodTimeLeft.ToString(@"mm\:ss");
            }
        }

        public override string SubText {
            get {
                if (mainTimeLeft > TimeSpan.Zero)
                {
                    return "Main time";
                }
                return periodStonesLeft + " stones left for this period";
            }
        }
        public override TimeControlStyle Style => TimeControlStyle.Canadian;
    }
}
