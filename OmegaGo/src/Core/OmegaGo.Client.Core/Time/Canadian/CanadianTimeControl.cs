using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.Canadian
{
    public class CanadianTimeControl : TimeControl
    {
        private readonly int _stonesPerPeriod;
        private readonly int _periodMinutes;

        private CanadianTimeInformation _snapshot;

        public CanadianTimeControl(int mainTime, int stonesPerPeriod, int periodMinutes)
        {
            this._snapshot = new CanadianTimeInformation(TimeSpan.FromMinutes(mainTime), TimeSpan.Zero, 0);
            this._stonesPerPeriod = stonesPerPeriod;
            this._periodMinutes = periodMinutes;
        }

        public override TimeControlStyle Name => TimeControlStyle.Canadian;

        private CanadianTimeInformation ReduceBy(CanadianTimeInformation minued, TimeSpan subtrahend)
        {
            throw new NotImplementedException();
            return minued;
        }
        public override TimeInformation GetDisplayTime(TimeSpan addThisTime)
        {
            throw new NotImplementedException();
        }

        public override void UpdateSnapshot(TimeSpan timeSpent)
        {
            _snapshot = ReduceBy(_snapshot, timeSpent);
        }

        public override bool IsViolating(TimeSpan addThisTime)
        {
            return ReduceBy(_snapshot, addThisTime).IsViolating();
        }
    }
}
