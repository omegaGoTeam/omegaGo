using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time
{

    /// <summary>
    /// A time control is the mechanism that ensure both players take a rougly equal amount of time.
    /// Subclasses represent various time control mechanisms.
    /// </summary>
    public abstract class TimeControl
    {
        public abstract TimeControlStyle Name { get; }

        private DateTime LastMoveMadeWhen = DateTime.Now;
        private bool Running;
        
        public abstract TimeInformation GetDisplayTime(TimeSpan addThisTime);
        public TimeInformation GetDisplayTime()
        {
            if (Running)
            {
                return GetDisplayTime(DateTime.Now - LastMoveMadeWhen);
            }
            else
            {
                return GetDisplayTime(TimeSpan.Zero);
            }
        }
        public abstract void UpdateSnapshot(TimeSpan timeSpent);

        public abstract bool IsViolating(TimeSpan addThisTime);

        public void StartClock()
        {
            if (Running) throw new InvalidOperationException("Clock was already running.");
            LastMoveMadeWhen = DateTime.Now;
            Running = true;
        }
        public void StopClock()
        {
            if (!Running) throw new InvalidOperationException("Clock was not yet running.");
            TimeSpan timeSpent = DateTime.Now - LastMoveMadeWhen;
            UpdateSnapshot(timeSpent);
            Running = false;
        }

        internal bool IsViolating()
        {
            if (Running)
            {
                return IsViolating(DateTime.Now - LastMoveMadeWhen);
            }
            else
            {
                return IsViolating(TimeSpan.Zero);
            }
        }
    }
}
