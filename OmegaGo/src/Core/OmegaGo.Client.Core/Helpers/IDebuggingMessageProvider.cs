using System;

namespace OmegaGo.Core.Helpers
{
    /// <summary>
    /// Interface for classes providing debugging events
    /// </summary>
    public interface IDebuggingMessageProvider
    {
        /// <summary>
        /// Indicates a debugging message
        /// </summary>
        event EventHandler<string> DebuggingMessage;
    }
}
