using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.Canadian
{
    class CanadianTimeInformation : TimeInformation
    {
        public TimeSpan MainTimeLeft { get;  }
        public TimeSpan PeriodTimeLeft { get;}
        public int PeriodStonesLeft { get; }

        public bool IsViolating()
        {
            return this.MainTimeLeft <= TimeSpan.Zero && this.PeriodTimeLeft <= TimeSpan.Zero;
        }

        public CanadianTimeInformation(TimeSpan mainTimeLeft, TimeSpan periodTimeLeft, int periodStonesLeft)
        {
            this.MainTimeLeft = mainTimeLeft;
            this.PeriodTimeLeft = periodTimeLeft;
            this.PeriodStonesLeft = periodStonesLeft;
        }

        public override string MainText {
            get {
                if (this.MainTimeLeft > TimeSpan.Zero)
                {
                    return this.MainTimeLeft.ToString(@"mm\:ss");
                }
                return this.PeriodTimeLeft.ToString(@"mm\:ss");
            }
        }

        public override string SubText {
            get {
                if (this.MainTimeLeft > TimeSpan.Zero)
                {
                    return "Main time";
                }
                return this.PeriodStonesLeft + " stones left for this period";
            }
        }
        public override TimeControlStyle Style => TimeControlStyle.Canadian;
    }
}
