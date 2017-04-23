using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Chat;

namespace OmegaGo.Core.Modes.LiveGame.Connectors
{
    internal interface IRemoteConnector : IGameConnector
    {
        /// <summary>
        /// Informs the subscribers that a new chat message has arrived
        /// </summary>
        event EventHandler<ChatMessage> NewChatMessageReceived;

        /// <summary>
        /// Chat message from server
        /// </summary>
        void ChatMessageFromServer(ChatMessage chatMessage);

        /// <summary>
        /// Sends a chat message
        /// </summary>
        /// <param name="chatMessage">Chat message to send</param>
        Task SendChatMessageAsync(string chatMessage);
    }
}
