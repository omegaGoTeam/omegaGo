using System;

namespace OmegaGo.Core.Time.Absolute
{
    /// <summary>
    /// In absolute time control, a player has a number of minutes that must suffice for the entire game.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Time.TimeControl" />
    public class AbsoluteTimeControl : TimeControl
    {
        public override TimeControlStyle Name => TimeControlStyle.Absolute;

        protected override TimeInformation GetDisplayTime(TimeSpan addThisTime)
        {
            return new AbsoluteTimeInformation(_mainTime - addThisTime);
        }

        protected override void UpdateSnapshot(TimeSpan timeSpent)
        {
            _mainTime = _mainTime - timeSpent;
        }

        protected override bool IsViolating(TimeSpan addThisTime)
        {
            return (_mainTime - addThisTime).Ticks <= 0;
        }

        public override void UpdateFromKgsFloat(float secondsLeftIThink)
        {
            LastTimeClockStarted = DateTime.Now;
            _mainTime = TimeSpan.FromSeconds(secondsLeftIThink);
        }

        public override string GetGtpInitializationCommand()
        {
            return "time_settings " + (int) _mainTime.TotalSeconds + " 0 0";
        }

        public override TimeLeftArguments GetGtpTimeLeftCommandArguments()
        {
            return new Time.TimeLeftArguments((int) _mainTime.TotalSeconds, 0);
        }

        private TimeSpan _mainTime;

        public AbsoluteTimeControl(int seconds)
        {
            _mainTime = TimeSpan.FromSeconds(seconds);
        }
    }
}
