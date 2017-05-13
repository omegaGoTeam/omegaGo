using System;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Time.Canadian
{
    /// <summary>
    ///     The Canadian time control consists of a main time and then periods where during each period, the player must make X
    ///     moves in Y minutes.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Time.TimeControl" />
    public class CanadianTimeControl : TimeControl
    {
        private readonly TimeSpan _periodTime;
        private readonly int _stonesPerPeriod;

        /// <summary>
        ///     Time that was remaining when I made my last move
        /// </summary>
        private CanadianTimeInformation _snapshot;

        public CanadianTimeControl(TimeSpan mainTime, int stonesPerPeriod, TimeSpan periodTime)
        {
            _snapshot = new CanadianTimeInformation(true, mainTime, TimeSpan.Zero, 0);
            _stonesPerPeriod = stonesPerPeriod;
            _periodTime = periodTime;
        }

        public TimeSpan PeriodTime => _periodTime;
        public int StonesPerPeriod => _stonesPerPeriod;

        public override TimeControlStyle Name => TimeControlStyle.Canadian;

        public override void UpdateFromKgsFloat(float secondsLeft)
        {// Don't use this. Use GAME_STATE instead for now. We don't need historical records of time keeping.
            /*
            this.LastTimeClockStarted = DateTime.Now;
            if (this._snapshot.MainTimeLeft > TimeSpan.Zero)
            {
                this._snapshot = new CanadianTimeInformation(TimeSpan.FromSeconds(secondsLeft),
                    this._snapshot.PeriodTimeLeft, this._snapshot.PeriodStonesLeft);
            }
            else
            {
                this._snapshot = new CanadianTimeInformation(this._snapshot.MainTimeLeft,
                    TimeSpan.FromSeconds(secondsLeft), this._snapshot.PeriodStonesLeft);
            }*/
        }

        public override string GetGtpInitializationCommand()
        {
            return "time_settings " + (int) this._snapshot.MainTimeLeft.TotalSeconds + " " +
                   (int) this._periodTime.TotalSeconds + " " + this._stonesPerPeriod;
        }

        public override TimeLeftArguments GetGtpTimeLeftCommandArguments()
        {
            var maintime = this._snapshot.MainTimeLeft;
            bool stillInMainTime = maintime > TimeSpan.Zero;
            return new TimeLeftArguments(
                (int) (stillInMainTime ? this._snapshot.MainTimeLeft : this._snapshot.PeriodTimeLeft).TotalSeconds,
                stillInMainTime ? 0 : this._snapshot.PeriodStonesLeft
                );
        }

        public CanadianTimeControl UpdateFrom(CanadianTimeInformation timeRemaining)
        {        
            this.LastTimeClockStarted = DateTime.Now;
            if (this.Running)
            {
                timeRemaining = new CanadianTimeInformation(
                    timeRemaining.StillInMainTime,
                    timeRemaining.MainTimeLeft,
                    timeRemaining.PeriodTimeLeft,
                    timeRemaining.PeriodStonesLeft + 1);
            }
            this._snapshot = timeRemaining;
            return this;
        }
        public override void UpdateFromClock(Clock clock)
        {
            LastTimeClockStarted = DateTime.Now;
            if (clock.StonesLeft == 0)
            {
                _snapshot = new Canadian.CanadianTimeInformation(true, TimeSpan.FromSeconds(clock.Time), _snapshot.PeriodTimeLeft, 0);
            }
            else
            {
                _snapshot = new Canadian.CanadianTimeInformation(false, TimeSpan.Zero, TimeSpan.FromSeconds(clock.Time), clock.StonesLeft);
            }
        }
        protected override TimeInformation GetDisplayTime(TimeSpan addThisTime)
        {
            return ReduceBy(this._snapshot, addThisTime);
        }

        protected override void UpdateSnapshot(TimeSpan timeSpent)
        {
            this._snapshot = ReduceBy(this._snapshot, timeSpent);
            this._snapshot = ImproveByPlacingAStone(this._snapshot);
        }


        protected override bool IsViolating(TimeSpan addThisTime)
        {
            return ReduceBy(this._snapshot, addThisTime).IsViolating();
        }

        private CanadianTimeInformation ReduceBy(CanadianTimeInformation minued, TimeSpan subtrahend)
        {
            var maintime = minued.MainTimeLeft;
            bool stillInMainTime = minued.StillInMainTime;
            if (stillInMainTime)
            {
                if (maintime > subtrahend)
                {
                    return new CanadianTimeInformation(true, maintime - subtrahend, minued.PeriodTimeLeft, minued.PeriodStonesLeft);
                }
                minued = new CanadianTimeInformation(false, TimeSpan.Zero, this._periodTime, this._stonesPerPeriod);
                subtrahend = subtrahend - maintime;
            }

            return new CanadianTimeInformation(false, TimeSpan.Zero, minued.PeriodTimeLeft - subtrahend, minued.PeriodStonesLeft);
        }

        private CanadianTimeInformation ImproveByPlacingAStone(CanadianTimeInformation snapshot)
        {
            if (snapshot.StillInMainTime) return snapshot;
            if (snapshot.PeriodStonesLeft > 1)
            {
                return new CanadianTimeInformation(
                    snapshot.StillInMainTime,
                    snapshot.MainTimeLeft,
                    snapshot.PeriodTimeLeft,
                    snapshot.PeriodStonesLeft - 1);
            }
            return new CanadianTimeInformation(snapshot.StillInMainTime, snapshot.MainTimeLeft, this._periodTime, this._stonesPerPeriod);
        }
    }
}