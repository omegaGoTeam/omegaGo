using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public CanadianTimeControl(int mainTime, int stonesPerPeriod, int periodMinutes)
        {
            this._snapshot = new CanadianTimeInformation(TimeSpan.FromMinutes(mainTime), TimeSpan.Zero, 0);
            this._stonesPerPeriod = stonesPerPeriod;
            this._periodTime = TimeSpan.FromMinutes(periodMinutes);
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
                    return new Canadian.CanadianTimeInformation(maintime - subtrahend, minued.PeriodTimeLeft,
                        minued.PeriodStonesLeft);
                }
                else
                {
                    minued = new Canadian.CanadianTimeInformation(TimeSpan.Zero, _periodTime, _stonesPerPeriod);
                    subtrahend = subtrahend - maintime;
                }
            }
            // Now we're eliminating periods.
            return new Canadian.CanadianTimeInformation(TimeSpan.Zero,
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
                return new Canadian.CanadianTimeInformation(snapshot.MainTimeLeft,
                    snapshot.PeriodTimeLeft,
                    snapshot.PeriodStonesLeft - 1);
            }
            else
            {
                return new Canadian.CanadianTimeInformation(snapshot.MainTimeLeft,
                    _periodTime, _stonesPerPeriod);
            }
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

        public CanadianTimeControl UpdateFrom(CanadianTimeInformation timeRemaining)
        {
            this.LastTimeClockStarted = DateTime.Now;
            if (Running)
            {
                timeRemaining = new CanadianTimeInformation(timeRemaining.MainTimeLeft,
                    timeRemaining.PeriodTimeLeft,
                    timeRemaining.PeriodStonesLeft + 1);
            }
            this._snapshot = timeRemaining; // TODO minus current time, I guess?
            return this;
        }
    }
}
