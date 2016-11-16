namespace OmegaGo.Core.Sgf.Parsing.Warnings
{
    /// <summary>
    /// Represents a warning during SGF parsing
    /// </summary>
    public struct SgfParseWarning
    {
        public SgfParseWarning( SgfParseWarningKind warningKind, int position )
        {
            Kind = warningKind;
            Position = position;
        }

        /// <summary>
        /// Type of warning
        /// </summary>
        public SgfParseWarningKind Kind { get; }

        /// <summary>
        /// Position
        /// </summary>
        public int Position { get; }
    }
}
