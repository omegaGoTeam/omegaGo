using System;

namespace OmegaGo.Core.Time.None
{
    /// <summary>
    /// The "no time control" time control means that both players have infinite time available.
    /// </summary>
    public class NoTimeControl : TimeControl
    {
        public override TimeControlStyle Name => TimeControlStyle.None;
        protected override TimeInformation GetDisplayTime(TimeSpan addThisTime) => new NoTimeInformation();

        protected override void UpdateSnapshot(TimeSpan timeSpent)
        {
        }

        protected override bool IsViolating(TimeSpan addThisTime)
        {
            return false;
        }

        public override void UpdateFromKgsFloat(float secondsLeftIThink)
        {
            
        }

        public override string GetGtpInitializationCommand()
        {
            return null; // no time limit
        }
    }
}
