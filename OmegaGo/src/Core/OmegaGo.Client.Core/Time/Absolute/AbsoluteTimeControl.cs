﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Time.Absolute;

namespace OmegaGo.Core.Time
{
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

        private TimeSpan _mainTime;

        public AbsoluteTimeControl(int minutes)
        {
            _mainTime = TimeSpan.FromMinutes(minutes);
        }
    }
}
