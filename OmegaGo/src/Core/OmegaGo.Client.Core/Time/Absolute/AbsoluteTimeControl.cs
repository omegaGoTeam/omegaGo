using System;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Time.Absolute
{
    /// <summary>
    ///     In absolute time control, a player has a number of minutes that must suffice for the entire game.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Time.TimeControl" />
    public class AbsoluteTimeControl : TimeControl
    {
        private TimeSpan _mainTime;

        public AbsoluteTimeControl(int seconds)
        {
            _mainTime = TimeSpan.FromSeconds(seconds);
        }

        public override TimeControlStyle Name => TimeControlStyle.Absolute;

        public override void UpdateFromKgsFloat(float secondsLeft)
        {
            this.LastTimeClockStarted = DateTime.Now;
            this._mainTime = TimeSpan.FromSeconds(secondsLeft);
        }

        public override string GetGtpInitializationCommand()
        {
            return "time_settings " + (int) this._mainTime.TotalSeconds + " 0 0";
        }

        public override TimeLeftArguments GetGtpTimeLeftCommandArguments()
        {
            return new TimeLeftArguments((int) this._mainTime.TotalSeconds, 0);
        }

        protected override TimeInformation GetDisplayTime(TimeSpan addThisTime)
        {
            return new AbsoluteTimeInformation(this._mainTime - addThisTime);
        }

        public override void UpdateFromClock(Clock clock)
        {
            LastTimeClockStarted = DateTime.Now;
            _mainTime = TimeSpan.FromSeconds(clock.Time);
        }
        protected override void UpdateSnapshot(TimeSpan timeSpent)
        {
            this._mainTime = this._mainTime - timeSpent;
        }
        protected override bool IsViolating(TimeSpan addThisTime)
        {
            return (this._mainTime - addThisTime).Ticks <= 0;
        }
    }
}