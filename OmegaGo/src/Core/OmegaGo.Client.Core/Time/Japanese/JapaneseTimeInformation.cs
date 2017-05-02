using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;

namespace OmegaGo.Core.Time.Japanese
{
    public class JapaneseTimeInformation : TimeInformation
    {
        public TimeSpan TimeLeft { get; }
        public int PeriodsLeft { get; }
        public bool InByoYomi { get; }

        public JapaneseTimeInformation(TimeSpan timeLeft, int periodsLeft, bool inByoYomi)
        {
            this.TimeLeft = timeLeft;
            this.PeriodsLeft = periodsLeft;
            this.InByoYomi = inByoYomi;
        }

        public override string MainText
        {
            get
            {
                if (PeriodsLeft < 0)
                {
                    return "00:00";
                }
                return TimeLeft.ToCountdownString();
            }
        }
        public override string SubText
        {
            get
            {
                if (PeriodsLeft < 0)
                {
                    return "Sorry...";
                }
                if (this.InByoYomi)
                {
                    return PeriodsLeft + " periods left after this";
                }
                else
                {
                    return "Main time";
                }
            }
        }
        public override TimeControlStyle Style => TimeControlStyle.Japanese;

        public bool IsViolating()
        {
            if (PeriodsLeft < 0) return true;
            return TimeLeft < TimeSpan.Zero;
        }
    }
}
