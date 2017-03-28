using System;

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
            this._snapshot = new CanadianTimeInformation(mainTime, TimeSpan.Zero, 0);
            this._stonesPerPeriod = stonesPerPeriod;
            this._periodTime = periodTime;
        }

        public override TimeControlStyle Name => TimeControlStyle.Canadian;

        public override void UpdateFromKgsFloat(float secondsLeft)
        {
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
            }
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
                timeRemaining = new CanadianTimeInformation(timeRemaining.MainTimeLeft,
                    timeRemaining.PeriodTimeLeft,
                    timeRemaining.PeriodStonesLeft + 1);
            }
            this._snapshot = timeRemaining;
            return this;
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
            bool stillInMainTime = maintime > TimeSpan.Zero;
            if (stillInMainTime)
            {
                if (maintime > subtrahend)
                {
                    return new CanadianTimeInformation(maintime - subtrahend, minued.PeriodTimeLeft,
                        minued.PeriodStonesLeft);
                }
                minued = new CanadianTimeInformation(TimeSpan.Zero, this._periodTime, this._stonesPerPeriod);
                subtrahend = subtrahend - maintime;
            }
            // Now we're eliminating periods.
            return new CanadianTimeInformation(TimeSpan.Zero,
                minued.PeriodTimeLeft - subtrahend, minued.PeriodStonesLeft);
        }

        private CanadianTimeInformation ImproveByPlacingAStone(CanadianTimeInformation snapshot)
        {
            if (snapshot.MainTimeLeft > TimeSpan.Zero) return snapshot;
            if (snapshot.PeriodStonesLeft > 1)
            {
                return new CanadianTimeInformation(snapshot.MainTimeLeft,
                    snapshot.PeriodTimeLeft,
                    snapshot.PeriodStonesLeft - 1);
            }
            return new CanadianTimeInformation(snapshot.MainTimeLeft, this._periodTime, this._stonesPerPeriod);
        }
    }
}