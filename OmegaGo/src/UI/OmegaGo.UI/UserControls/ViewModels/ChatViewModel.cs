using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Online.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class ChatViewModel : ControlViewModelBase
    {
        private readonly IChatService _chatService;

        private string _messageText = "";
        private string _humanAuthor;

        private MvxAsyncCommand _sendMessageCommand;

        /// <summary>
        /// Creates chat
        /// </summary>
        /// <param name="chatService"></param>
        public ChatViewModel(IChatService chatService)
        {
            _chatService = chatService;
            _chatService.NewMessageReceived += ChatService_NewMessageReceived;
            Messages = new ObservableCollection<ChatMessage>(chatService.Messages);
        }

        public IMvxCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new MvxAsyncCommand(
                                                     SendMessageAsync,
                                                     () => !string.IsNullOrEmpty(MessageText)));

        /// <summary>
        /// All chat messages
        /// </summary>
        public ObservableCollection<ChatMessage> Messages { get; }

        /// <summary>
        /// Text of the message
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
        /// Handles new incoming chat message
        /// </summary>
        private void ChatService_NewMessageReceived(object sender, ChatMessage e)
        {            
            Dispatcher.RequestMainThreadAction(() => Messages.Add(e));
        }

        /// <summary>
        /// Sends a chat message
        /// </summary>
        /// <returns></returns>
        private async Task SendMessageAsync()
        {
            await _chatService.SendMessageAsync(MessageText);            
            MessageText = "";
        }
    }
}
