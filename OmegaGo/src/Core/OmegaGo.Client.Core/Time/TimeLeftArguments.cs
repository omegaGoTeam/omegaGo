namespace OmegaGo.Core.Time
{
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeLeftArguments"/> class.
        /// </summary>
        /// <param name="seconds">Seconds left in the current period.</param>
        /// <param name="stones">Number of stones that still have to be placed in the current period.</param>
        public TimeLeftArguments(int seconds, int stones)
        {
            this.NumberOfSecondsRemaining = seconds;
            this.NumberOfStonesRemaining = stones;
        }
    }
}