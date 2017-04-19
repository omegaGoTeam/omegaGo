using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Chat
{
    /// <summary>
    /// Chat service
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Informs subscribers about newly received chat message
        /// </summary>
        event EventHandler<ChatMessage> NewMessageReceived;

        /// <summary>
        /// All messages in the chat
        /// </summary>
        IReadOnlyList<ChatMessage> Messages { get; }

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message">Chat message to send</param>        
        Task SendMessageAsync(string message);
    }
}
