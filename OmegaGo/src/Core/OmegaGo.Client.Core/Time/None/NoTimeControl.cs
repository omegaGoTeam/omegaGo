using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Time.None;

namespace OmegaGo.Core.Time
{
    /// <summary>
    /// The "no time control" time control means that both players have infinite time available.
    /// </summary>
    public class NoTimeControl : TimeControl
    {
        public override TimeControlStyle Name => TimeControlStyle.None;
        public override TimeInformation GetDisplayTime(TimeSpan addThisTime) => new NoTimeInformation();
        public override void UpdateSnapshot(TimeSpan timeSpent)
        {
        }

        public override bool IsViolating(TimeSpan addThisTime)
        {
            return false;
        }
    }
}
