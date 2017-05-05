using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.Connectors;

namespace OmegaGo.Core.Online.Chat
{
    /// <summary>
    /// The class implementing this interface (<see cref="ChatService"/>) groups functionality related to in-game chat. A single instance
    /// of <see cref="IChatService"/> is connected to a single online game. The UI viewmodel uses it to send messages (via the game's <see cref="IRemoteConnector"/>)
    /// and its event (triggered by the same connector) inform the UI when a new message is to be displayed.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Occurs when a new chat message is to be shown to the user. This may be both an incoming and an outgoing message. A call to <see cref="SendMessageAsync(string)"/>
        /// usually triggers a <see cref="NewMessageReceived"/> event (unless something is wrong with the server connnection).  
        /// </summary>
        event EventHandler<ChatMessage> NewMessageReceived;

        /// <summary>
        /// Gets a read-only list of all messages recorded in this game.
        /// </summary>
        IReadOnlyList<ChatMessage> Messages { get; }

        /// <summary>
        /// Asks the associated <see cref="IRemoteConnector"/> to send an outgoing chat message to the server. The message will be associated with the game this 
        /// chat service is associated with. 
        /// </summary>
        /// <param name="message">Chat message to send</param>        
        Task SendMessageAsync(string message);
    }
}
