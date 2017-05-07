using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Time
{
    /// <summary>
    ///     A time control is the mechanism that ensure both players take a rougly equal amount of time.
    ///     Subclasses represent various time control mechanisms. An instance of this class represents
    ///     the clock running for a single player.
    /// </summary>
    /// <remarks>
    ///     An important concept here is "snapshot time". Snapshot time is the time that was remaining on the clock
    ///     when it was last updated (usually because a player just made a move).
    /// </remarks>
    public abstract class TimeControl
    {
        /// <summary>
        ///     Gets the identifier of the time control system.
        /// </summary>
        // ReSharper disable once UnusedMember.Global -- We don't use this (we do a lot of direct casting), but it makes sense for this to be here.
        public abstract TimeControlStyle Name { get; }
        
        /// <summary>
        ///     The time when the clock was last started. When the clock is next stopped, the diffence between the current time and
        ///     this value will be added to the player's elapsed time.
        /// </summary>
        protected DateTime LastTimeClockStarted { private get; set; } = DateTime.Now;


        /// <summary>
        ///     Updates the snapshot time based on the "TIMELEFT" SGF property sent by KGS.
        /// </summary>
        /// <param name="withGrace">If true, an additional second should be added to the elapsed time, as though we were one second in the future.</param>
        public TimeInformation GetDisplayTime(bool withGrace)
        {
            if (this.Running)
            {
                return GetDisplayTime(DateTime.Now - this.LastTimeClockStarted + (withGrace ? TimeSpan.FromSeconds(1) : TimeSpan.Zero));
            }
            else
            {
                return GetDisplayTime(TimeSpan.Zero + (withGrace ? TimeSpan.FromSeconds(1) : TimeSpan.Zero));
            }
        }

        /// <summary>
        ///     Starts the clock.
        /// </summary>
        public void StartClock()
        {
            if (this.Running)
            {
                // It was already running.
                return;
            }
            this.LastTimeClockStarted = DateTime.Now;
            this.Running = true;
        }

        /// <summary>
        ///     Stops the clock. The snapshot time is also updated.
        /// </summary>
        public void StopClock()
        {
            if (!this.Running)
            {
                return; // It was already stopped.
            }
            var timeSpent = DateTime.Now - this.LastTimeClockStarted;
            UpdateSnapshot(timeSpent);
            this.Running = false;
        }
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
        /// <returns></returns>
        public abstract TimeLeftArguments GetGtpTimeLeftCommandArguments();

        /// <summary>
        /// Updates the snapshot based on clock information sent by KGS's GAME_STATE request.
        /// </summary>
        /// <param name="clock">The clock.</param>
        public abstract void UpdateFromClock(Clock clock);

        /// <summary>
        ///     Updates the snapshot time based on the "TIMELEFT" SGF property sent by KGS.
        /// </summary>
        /// <param name="secondsLeft">The seconds left in this period for this player.</param>
        public abstract void UpdateFromKgsFloat(float secondsLeft);

        /// <summary>
        ///     Determines whether the player has exceeded their time and should lose.
        /// </summary>
        public bool IsViolating()
        {
            return IsViolating(this.Running
                ? DateTime.Now - this.LastTimeClockStarted
                : TimeSpan.Zero);
        }

        /// <summary>
        ///     Gets the time that would be remaining on the clock if <paramref name="addThisTime" /> would be added to the
        ///     "snapshot time"
        ///     (i.e. the time remaining when the clock was last updated).
        /// </summary>
        protected abstract TimeInformation GetDisplayTime(TimeSpan addThisTime);

        /// <summary>
        ///     Adds <paramref name="timeSpent" /> to the "snapshot time" which was the time remaining when the clock was last
        ///     updated.
        /// </summary>
        /// <param name="timeSpent">The time spent since the clock was last updated.</param>
        protected abstract void UpdateSnapshot(TimeSpan timeSpent);

        /// <summary>
        ///     Determines whether the player would have exceeded their time if <paramref name="addThisTime" /> were added to the
        ///     time that elapsed from the player's clock.
        ///     For example, suppose that when I last made a move, I had 5 seconds left on the clock. This is stored with this
        ///     TimeControl instance. Now,
        ///     if it's my turn and my clocks runs again for, say 2 seconds, I would call this method with the argument "2 seconds"
        ///     and it would
        ///     return false, because 5 seconds (snapshot) - 2 seconds (the argument) still hasn't reached zero.
        /// </summary>
        protected abstract bool IsViolating(TimeSpan addThisTime);

        /// <summary>
        ///     Indicates whether the clock is currently counting down.
        /// </summary>
        protected bool Running { get; private set; }

    }
}