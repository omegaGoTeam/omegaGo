namespace OmegaGo.Core.Time
{
    public abstract class TimeInformation
    {
        public abstract string MainText { get; }
        public abstract string SubText { get; }
        public abstract TimeControlStyle Style { get; }
    }
}