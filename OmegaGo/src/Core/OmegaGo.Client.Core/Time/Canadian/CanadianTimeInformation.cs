using System;
using OmegaGo.Core.Extensions;

namespace OmegaGo.Core.Time.Canadian
{
    public class CanadianTimeInformation : TimeInformation
    {
        public TimeSpan MainTimeLeft { get;  }
        public TimeSpan PeriodTimeLeft { get;}
        public int PeriodStonesLeft { get; }

        public bool StillInMainTime { get; }

        public override string MainText
        {
            get
            {
                if (MainTimeLeft > TimeSpan.Zero)
                {
                    return MainTimeLeft.ToCountdownString();
                }
                if (PeriodTimeLeft > TimeSpan.Zero)
                {
                    return PeriodTimeLeft.ToCountdownString();
                }
                return "00:00";
            }
        }

        public override string SubText
        {
            get
            {
                if (MainTimeLeft > TimeSpan.Zero)
                {
                    return "Main time";
                }
                return PeriodStonesLeft + " stones left for this period";
            }
        }

        public override TimeControlStyle Style => TimeControlStyle.Canadian;

        public CanadianTimeInformation(bool inMainTime, TimeSpan mainTimeLeft, TimeSpan periodTimeLeft, int periodStonesLeft)
        {
            StillInMainTime = inMainTime;
            MainTimeLeft = mainTimeLeft;
            PeriodTimeLeft = periodTimeLeft;
            PeriodStonesLeft = periodStonesLeft;
        }

        public bool IsViolating()
        {
            return MainTimeLeft <= TimeSpan.Zero && PeriodTimeLeft <= TimeSpan.Zero;
        }

    }
}
