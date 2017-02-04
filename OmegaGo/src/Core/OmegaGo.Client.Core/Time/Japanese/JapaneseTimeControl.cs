using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.Japanese
{
    class JapaneseTimeControl : TimeControl
    {
        public override TimeControlStyle Name => TimeControlStyle.Japanese;
        public override TimeInformation GetDisplayTime(TimeSpan addThisTime)
        {
            throw new NotImplementedException();
        }

        public override void UpdateSnapshot(TimeSpan timeSpent)
        {
            throw new NotImplementedException();
        }

        public override bool IsViolating(TimeSpan addThisTime)
        {
            throw new NotImplementedException();
        }
    }
}
