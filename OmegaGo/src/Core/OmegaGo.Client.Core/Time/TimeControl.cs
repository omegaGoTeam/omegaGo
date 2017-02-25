using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time
{

    /// <summary>
    /// A time control is the mechanism that ensure both players take a rougly equal amount of time.
    /// Subclasses represent various time control mechanisms. An instance of this class represents
    /// the clock running for a single player.
    /// </summary>
    public abstract class TimeControl
    {
        public abstract TimeControlStyle Name { get; }

        protected DateTime LastTimeClockStarted = DateTime.Now;
        protected bool Running;

        protected abstract TimeInformation GetDisplayTime(TimeSpan addThisTime);

        /// <summary>
        /// Gets the remaining time for this player. Uses the snapshot time and the difference
        /// to <see cref="DateTime.Now"/> to get the remaining time. 
        /// </summary>
        public TimeInformation GetDisplayTime()
        {
            if (Running)
            {
                return GetDisplayTime(DateTime.Now - this.LastTimeClockStarted);
            }
            else
            {
                return GetDisplayTime(TimeSpan.Zero);
            }
        }

        protected abstract void UpdateSnapshot(TimeSpan timeSpent);

        protected abstract bool IsViolating(TimeSpan addThisTime);

        /// <summary>
        /// Starts the clock.
        /// </summary>
        public void StartClock()
        {
            if (Running)
            {
                // It was already running.
                return;
            }
            this.LastTimeClockStarted = DateTime.Now;
            Running = true;
        }

        /// <summary>
        /// Stops the clock. The snapshot is also updated because a move was just made.
        /// </summary>
        public void StopClock()
        {
            if (!Running)
            {
                return; // It was already stopped.
            }
            TimeSpan timeSpent = DateTime.Now - this.LastTimeClockStarted;
            UpdateSnapshot(timeSpent);
            Running = false;
        }

        /// <summary>
        /// Determines whether the player has exceeded their time and should lose.
        /// </summary>
        internal bool IsViolating()
        {
            return IsViolating(
                Running ?
                    (DateTime.Now - this.LastTimeClockStarted) :
                    TimeSpan.Zero);
        }
    }
}
