﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.Japanese
{
    /// <summary>
    /// The Japanese time control is standard Japanese byo-yomi, i.e. maintime + a number of byo-yomi periods where the current period resets upon move.
    /// </summary>
    public class JapaneseTimeControl : TimeControl
    {
        private readonly int _byoyomiLengthInSeconds;

        /// <summary>
        /// Time that was remaining when a snapshot was last made (just after a move)
        /// </summary>
        private JapaneseTimeInformation _snapshot;

        public JapaneseTimeControl(int mainTimeSeconds, int byoyomiLengthInSeconds, int byoyomiPeriodCount)
        {
            this._byoyomiLengthInSeconds = byoyomiLengthInSeconds;
            this._snapshot = new Japanese.JapaneseTimeInformation(TimeSpan.FromSeconds(mainTimeSeconds),
                byoyomiPeriodCount, false);
        }

        public override TimeControlStyle Name => TimeControlStyle.Japanese;

        protected override TimeInformation GetDisplayTime(TimeSpan addThisTime)
        {
            return ReduceBy(_snapshot, addThisTime);
        }
        private JapaneseTimeInformation ReduceBy(JapaneseTimeInformation minued, TimeSpan subtrahend)
        {
            TimeSpan subtractStill = subtrahend;
            JapaneseTimeInformation result = new Japanese.JapaneseTimeInformation(minued.TimeLeft,
                minued.PeriodsLeft, minued.InByoYomi);
            while (subtractStill > TimeSpan.Zero)
            {
                if (result.TimeLeft > subtractStill)
                {
                    result = new Japanese.JapaneseTimeInformation(result.TimeLeft - subtractStill, result.PeriodsLeft,
                        result.InByoYomi);
                    break;
                }
                else
                {
                    subtractStill -= result.TimeLeft;
                    result = new Japanese.JapaneseTimeInformation(TimeSpan.FromSeconds(_byoyomiLengthInSeconds),
                        result.PeriodsLeft - 1, true);
                }
            }
            return result;
        }

        protected override void UpdateSnapshot(TimeSpan timeSpent)
        {
            // A move was just made.
            _snapshot = ReduceBy(_snapshot, timeSpent);
            if (_snapshot.InByoYomi)
            {
                _snapshot = new Japanese.JapaneseTimeInformation(TimeSpan.FromSeconds(_byoyomiLengthInSeconds),
                    _snapshot.PeriodsLeft, true);
            }
        }

        protected override bool IsViolating(TimeSpan addThisTime)
        {
            return ReduceBy(_snapshot, addThisTime).IsViolating();
        }
    }
}