using System;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Time.None
{
    /// <summary>
    ///     The "no time control" time control means that both players have infinite time available.
    /// </summary>
    public class NoTimeControl : TimeControl
    {
        public override TimeControlStyle Name => TimeControlStyle.None;

        public override void UpdateFromKgsFloat(float secondsLeft)
        {
        }

        public override string GetGtpInitializationCommand()
        {
            return null; // no time limit
        }

        public override TimeLeftArguments GetGtpTimeLeftCommandArguments()
        {
            return null;
        }

        protected override TimeInformation GetDisplayTime(TimeSpan addThisTime) => new NoTimeInformation();

        protected override void UpdateSnapshot(TimeSpan timeSpent)
        {
        }

        protected override bool IsViolating(TimeSpan addThisTime)
        {
            return false;
        }

        public override void UpdateFromClock(Clock clock)
        {
            // This will never happen, I think, and even if it does, we don't keep track of anything in this "time control".
        }
    }
}