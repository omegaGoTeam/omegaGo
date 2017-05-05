using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Connectors;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.LiveGame.Remote
{
    
    public class ChatService : IChatService
    {
        private readonly IRemoteConnector _connector;
        private readonly List<ChatMessage> _messages = new List<ChatMessage>();

        public event EventHandler<ChatMessage> NewMessageReceived;

        internal ChatService(IRemoteConnector connector)
        {
            _connector = connector;
            _connector.NewChatMessageReceived += _connector_NewChatMessageReceived;
            Messages = new ReadOnlyCollection<ChatMessage>(_messages);
        }

        /// <summary>
        /// Chat messages list
        /// </summary>
        public IReadOnlyList<ChatMessage> Messages { get; }

        /// <summary>
        /// Sends a chat message
        /// </summary>
        /// <param name="message">Chat message to send</param>
        public Task SendMessageAsync(string message)
        {
            return _connector.SendChatMessageAsync(message);
        }

        /// <summary>
        /// Raises the NewMessageReceived event
        /// </summary>
        /// <param name="chatMessage">Chat message received</param>
        private void OnNewMessageReceived(ChatMessage chatMessage)
        {
            _messages.Add(chatMessage);
            NewMessageReceived?.Invoke(this, chatMessage);
        }

        /// <summary>
        /// The game connector received a new chat message
        /// </summary>
        private void _connector_NewChatMessageReceived(object sender, ChatMessage e)
        {
            OnNewMessageReceived(e);
        }
    }
}
