﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.Japanese
{
    /// <summary>
    /// The Japanese time control is standard Japanese byo-yomi,
    /// i.e. maintime + a number of byo-yomi periods where the current period resets upon move.
    /// See http://senseis.xmp.net/?ByoYomi
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

        public override void UpdateFromKgsFloat(float secondsLeft)
        {
            LastTimeClockStarted = DateTime.Now;
            _snapshot = new Japanese.JapaneseTimeInformation(TimeSpan.FromSeconds(secondsLeft),
                _snapshot.PeriodsLeft, _snapshot.InByoYomi);
        }

        public override string GetGtpInitializationCommand()
        {
            // Go Text Protocol does not support japanese byo-yomi but we can approximate but disallowing the engine
            // from ever using additional periods: the engine will think that its first byoyomi period is all it has.
            return "time_settings " + (int) _snapshot.TimeLeft.TotalSeconds + " " + _byoyomiLengthInSeconds + " 1";
        }

        public override TimeLeftArguments GetGtpTimeLeftCommandArguments()
        {
            return new Time.TimeLeftArguments((int) _snapshot.TimeLeft.TotalSeconds, _snapshot.InByoYomi ? 1 : 0);
        }

        protected override TimeInformation GetDisplayTime(TimeSpan addThisTime)
        {
            return ReduceBy(_snapshot, addThisTime);
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

        /// <summary>
        /// Reduces the time remaining on the clock in the <paramref name="minued"/> by <paramref name="subtrahend"/>
        /// and returns the result. This cannot be static because it uses the <see cref="_byoyomiLengthInSeconds"/> field. 
        /// </summary>
        /// <param name="minued">The minued.</param>
        /// <param name="subtrahend">The subtrahend.</param>
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
        
    }
}
