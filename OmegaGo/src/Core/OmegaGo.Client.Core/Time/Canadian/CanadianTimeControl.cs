using System;

namespace OmegaGo.Core.Time.Canadian
{
    /// <summary>
    /// The Canadian time control consists of a main time and then periods where during each period, the player must make X moves in Y minutes.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Time.TimeControl" />
    public class CanadianTimeControl : TimeControl
    {
        private readonly int _stonesPerPeriod;
        private readonly TimeSpan _periodTime;

        /// <summary>
        /// Time that was remaining when I made my last move
        /// </summary>
        private CanadianTimeInformation _snapshot;

        public CanadianTimeControl(TimeSpan mainTime, int stonesPerPeriod, TimeSpan periodTime)
        {
            _snapshot = new CanadianTimeInformation(mainTime, TimeSpan.Zero, 0);
            _stonesPerPeriod = stonesPerPeriod;
            _periodTime = periodTime;
        }

        public override TimeControlStyle Name => TimeControlStyle.Canadian;

        private CanadianTimeInformation ReduceBy(CanadianTimeInformation minued, TimeSpan subtrahend)
        {
            TimeSpan maintime = minued.MainTimeLeft;
            bool stillInMainTime = maintime > TimeSpan.Zero;
            if (stillInMainTime)
            {
                if (maintime > subtrahend)
                {
                    return new CanadianTimeInformation(maintime - subtrahend, minued.PeriodTimeLeft,
                        minued.PeriodStonesLeft);
                }
                minued = new CanadianTimeInformation(TimeSpan.Zero, _periodTime, _stonesPerPeriod);
                subtrahend = subtrahend - maintime;
            }
            // Now we're eliminating periods.
            return new CanadianTimeInformation(TimeSpan.Zero,
                minued.PeriodTimeLeft - subtrahend, minued.PeriodStonesLeft);
        }

        protected override TimeInformation GetDisplayTime(TimeSpan addThisTime)
        {
            return ReduceBy(_snapshot, addThisTime);
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
            return new CanadianTimeInformation(snapshot.MainTimeLeft,
                _periodTime, _stonesPerPeriod);
        }

        protected override void UpdateSnapshot(TimeSpan timeSpent)
        {
            _snapshot = ReduceBy(_snapshot, timeSpent);
            _snapshot = ImproveByPlacingAStone(_snapshot);
        }


        protected override bool IsViolating(TimeSpan addThisTime)
        {
            return ReduceBy(_snapshot, addThisTime).IsViolating();
        }

        public override void UpdateFromKgsFloat(float secondsLeftIThink)
        {
            LastTimeClockStarted = DateTime.Now;
            if (_snapshot.MainTimeLeft > TimeSpan.Zero)
            {
                _snapshot = new Canadian.CanadianTimeInformation(TimeSpan.FromSeconds(secondsLeftIThink), _snapshot.PeriodTimeLeft,
                    _snapshot.PeriodStonesLeft);
            }
            else
            {
                _snapshot = new Canadian.CanadianTimeInformation(_snapshot.MainTimeLeft, TimeSpan.FromSeconds(secondsLeftIThink),
                    _snapshot.PeriodStonesLeft);
            }
        }

        public CanadianTimeControl UpdateFrom(CanadianTimeInformation timeRemaining)
        {
            LastTimeClockStarted = DateTime.Now;
            if (Running)
            {
                timeRemaining = new CanadianTimeInformation(timeRemaining.MainTimeLeft,
                    timeRemaining.PeriodTimeLeft,
                    timeRemaining.PeriodStonesLeft + 1);
            }
            _snapshot = timeRemaining; // TODO Petr:  minus current time, I guess?
            return this;
        }
    }
}
