using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.LiveGame.Connectors;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Chat;

namespace OmegaGo.Core.Modes.LiveGame.Connectors
{
    public interface IRemoteConnector : IGameConnector
    {
        /// <summary>
        /// Informs the subscribers that a new chat message has arrived
        /// </summary>
        event EventHandler<ChatMessage> NewChatMessageReceived;

        /// <summary>
        /// Informs the subscribers that the server sent information that a PLAYER is satisfied
        /// with the removed stones.
        /// </summary>
        event EventHandler<GamePlayer> ServerSaysAPlayerIsDone;

        /// <summary>
        /// Chat message from server
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global - while it's not used directly, it forces its implementation in the classes.
        void ChatMessageFromServer(ChatMessage chatMessage);

        /// <summary>
        /// Sends a chat message
        /// </summary>
        /// <param name="chatMessage">Chat message to send</param>
        Task SendChatMessageAsync(string chatMessage);
    }
}
