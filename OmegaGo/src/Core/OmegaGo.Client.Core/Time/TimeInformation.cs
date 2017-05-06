namespace OmegaGo.Core.Time
{
    /// <summary>
    /// Represents the time remaining on the clock for a single player in a game.
    /// </summary>
    public abstract class TimeInformation
    {
        /// <summary>
        /// Gets the non-localized debug text that should be displayed in large letters. Usually displays time remaining in a period.
        /// Do not use this if you can use the localizer in the UI project.
        /// </summary>
        public abstract string MainText { get; }

        /// <summary>
        /// Gets the non-localized debug text that should be displayed in smaller letters under the main text. Usually displays type of period.
        /// Do not use this if you can use the localizer in the UI project.
        /// </summary>
        public abstract string SubText { get; }

        /// <summary>
        /// Gets the name of the time control used for this game.
        /// </summary>
        public abstract TimeControlStyle Style { get; }
    }
}