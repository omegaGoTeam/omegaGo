namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    /// Stores the answer of the GTP engine to a GTP command.
    /// </summary>
    public class GtpResponse
    {
        public GtpResponse(bool success, string text)
        {
            this.Successful = success;
            this.Text = text;
        }

        /// <summary>
        /// Gets a value indicating whether the GTP command was successful. This is determine from whether the answer 
        /// begins with "=" (success) or "?" (failure). 
        /// 
        /// In general, we assume all GTP commands to be successful, since the commands we use may only fail if the command itself
        /// is malformed, and we sanitize user input. This property exists for debugging purposes.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Successful { get; }

        /// <summary>
        /// Gets the text of the answer, except that the first two characters (either "= " or "? ") are omitted.
        /// </summary>
        public string Text { get; }

        public override string ToString()
        {
            return (Successful ? "= " : "? ") + Text;
        }
    }
}