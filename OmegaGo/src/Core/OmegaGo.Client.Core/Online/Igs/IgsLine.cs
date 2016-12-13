namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// Represents a line of text sent by the IGS server to this client.
    /// </summary>
    public class IgsLine
    {
        /// <summary>
        /// Every line sent by the server starts with an <see cref="IgsCode"/> (except for help files).
        /// </summary>
        public readonly IgsCode Code;
        /// <summary>
        /// The entire line sent, including the code at the beginning.
        /// </summary>
        public readonly string EntireLine;
        /// <summary>
        /// Gets the trimmed part of the line that follows the <see cref="Code"/>. 
        /// </summary>
        public string PureLine => EntireLine.Contains(" ") ? EntireLine.Substring(EntireLine.IndexOf(' ') + 1) : "";

        public IgsLine(IgsCode code, string line)
        {
            Code = code;
            EntireLine = line;
        }

        public override string ToString() => EntireLine;
    }
}