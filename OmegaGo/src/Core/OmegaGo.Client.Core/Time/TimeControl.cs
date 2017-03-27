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
    /// <remarks>
    /// An important concept here is "snapshot time". Snapshot time is the time that was remaining on the clock 
    /// when it was last updated (usually because a player just made a move).
    /// </remarks>
    public abstract class TimeControl
    {
        /// <summary>
        /// Gets the identifier of the time control system.
        /// </summary>
        public abstract TimeControlStyle Name { get; }

        /// <summary>
        /// The time when the clock was last started. When the clock is next stopped, the diffence between the current time and 
        /// this value will be added to the player's elapsed time.
        /// </summary>
        protected DateTime LastTimeClockStarted { private get; set; } = DateTime.Now;

        /// <summary>
        /// Indicates whether the clock is currently counting down.
        /// </summary>
        protected bool Running { get; private set; }
        /// <summary>
        /// Gets the time that would be remaining on the clock if <paramref name="addThisTime"/> would be added to the "snapshot time"
        /// (i.e. the time remaining when the clock was last updated).
        /// </summary>
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

        /// <summary>
        /// Adds <paramref name="timeSpent"/> to the "snapshot time" which was the time remaining when the clock was last updated.
        /// </summary>
        /// <param name="timeSpent">The time spent since the clock was last updated.</param>
        protected abstract void UpdateSnapshot(TimeSpan timeSpent);
        /// <summary>
        /// Determines whether, if <paramref name="addThisTime"/> were added to the elapsed time, this clock would reach zero.
        /// </summary>
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
        /// Stops the clock. The snapshot time is also updated.
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

        /// <summary>
        /// Updates the snapshot time based on the "TIMELEFT" SGF property sent by KGS.
        /// </summary>
        /// <param name="secondsLeft">The seconds left in this period for this player.</param>
        public abstract void UpdateFromKgsFloat(float secondsLeft);

        /// <summary>
        /// Gets the GTP time_settings command that should be used to initialize this time control to the value at the beginning
        /// of a game, as per http://www.lysator.liu.se/~gunnar/gtp/gtp2-spec-draft2/gtp2-spec.html#SECTION00073400000000000000.
        /// Returns null if no command should be sent (because we play with no time limit or because the GTP protocol does not
        /// understand this time control.
        /// </summary>
        /// <returns></returns>
        public abstract string GetGtpInitializationCommand();

        /// <summary>
        /// Gets arguments for the GTP time_left command that should be called prior to every move generation request.
        /// Returns null if no command should be sent.
        /// </summary>
        public abstract TimeLeftArguments GetGtpTimeLeftCommandArguments();
    }

    /// <summary>
    /// Contains information used by the GTP protocol to send information about remaining time (in Canadian overtime, mostly) 
    /// to a Go engine. 
    /// </summary>
    public class TimeLeftArguments
    {
        /// <summary>
        /// Gets the number of seconds remaining in the current period (or main time, if not yet in a period).
        /// </summary>
        public int NumberOfSecondsRemaining { get; }
        /// <summary>
        /// Gets the number of stones that must still be made in the current period. Zero means "main time".
        /// </summary>
        public int NumberOfStonesRemaining { get; }
        public TimeLeftArguments(int seconds, int stones)
        {
            NumberOfSecondsRemaining = seconds;
            NumberOfStonesRemaining = stones;
        }
    }
}
