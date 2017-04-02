using System;

namespace OmegaGo.Core.Time.Canadian
{
    //TODO Martin - FromIgs does absolutely not belong here
    public class CanadianTimeInformation : TimeInformation
    {
        public TimeSpan MainTimeLeft { get;  }
        public TimeSpan PeriodTimeLeft { get;}
        public int PeriodStonesLeft { get; }

        public override string MainText
        {
            get
            {
                if (MainTimeLeft > TimeSpan.Zero)
                {
                    return MainTimeLeft.ToString(@"mm\:ss");
                }
                if (PeriodTimeLeft > TimeSpan.Zero)
                {
                    return PeriodTimeLeft.ToString(@"mm\:ss");
                }
                return "Time exceeded";
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

        public CanadianTimeInformation(TimeSpan mainTimeLeft, TimeSpan periodTimeLeft, int periodStonesLeft)
        {
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
