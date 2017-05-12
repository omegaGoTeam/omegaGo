using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Online.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OmegaGo.Core.Modes.LiveGame.Connectors;
using OmegaGo.UI.Localization;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class ChatViewModel : ControlViewModelBase
    {
        private readonly IChatService _chatService;

        private string _messageText = "";

        private MvxAsyncCommand _sendMessageCommand;

        /// <summary>
        /// Initializes a new instance of chat view model.
        /// </summary>
        /// <param name="chatService">a chat service that should be used</param>
        /// <param name="connector">a connector that provides information about opponents stone removal acceptance</param>
        public ChatViewModel(IChatService chatService, IRemoteConnector connector)
        {
            _chatService = chatService;
            _chatService.NewMessageReceived += ChatService_NewMessageReceived;
            connector.ServerSaysAPlayerIsDone += Connector_ServerSaysAPlayerIsDone;
            Messages = new ObservableCollection<ChatMessage>(chatService.Messages);
        }

        public IMvxCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new MvxAsyncCommand(
                                                     SendMessageAsync,
                                                     () => !string.IsNullOrEmpty(MessageText)));

        /// <summary>
        /// A collection of all chat messages.
        /// </summary>
        public ObservableCollection<ChatMessage> Messages { get; }

        /// <summary>
        /// Text of the currently composed message.
        /// </summary>
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                SetProperty(ref _messageText, value);
                SendMessageCommand.RaiseCanExecuteChanged();
            }
        }             

        /// <summary>
        /// Handles new incoming chat message.
        /// </summary>
        private void ChatService_NewMessageReceived(object sender, ChatMessage e)
        {            
            Dispatcher.RequestMainThreadAction(() => Messages.Add(e));
        }

        /// <summary>
        /// Sends a chat message.
        /// </summary>
        /// <returns></returns>
        private async Task SendMessageAsync()
        {
            if (String.IsNullOrWhiteSpace(MessageText))
            {
                return;
            }

            string msg = MessageText;
            MessageText = "";
            
            await _chatService.SendMessageAsync(msg);
        }

        private void Connector_ServerSaysAPlayerIsDone(object sender, Core.Modes.LiveGame.Players.GamePlayer e)
        {
            ChatService_NewMessageReceived(
                sender,
                new ChatMessage(
                    "System", 
                    String.Format(LocalizedStrings.XIsSatisfiedWithTheRemovedStones, e), 
                    DateTimeOffset.Now,
                    ChatMessageKind.Incoming));
        }
    }
}
