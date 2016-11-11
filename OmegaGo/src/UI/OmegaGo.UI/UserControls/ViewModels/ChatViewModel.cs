using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Online.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class ChatViewModel : MvxViewModel 
    {
        private IChatService _chatService;
        private ObservableCollection<ChatMessage> _messages;
        private string _messageText;

        private MvxCommand _sendMessageCommand;
        
        public IChatService ChatService
        {
            get { return _chatService; }
            set
            {
                _chatService.MessageRecieved += ChatService_MessageRecieved;
                SetProperty(ref _chatService, value);
            }
        }

        public ObservableCollection<ChatMessage> Messages
        {
            get { return _messages; }
        }

        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); SendMessageCommand.RaiseCanExecuteChanged(); }
        }

        public ChatViewModel()
        {
            _messageText = "";
            _messages = new ObservableCollection<ChatMessage>();

            _messages.Add(new ChatMessage() { UserName = "Franta Perníkáč", Time = DateTimeOffset.Now, Kind = ChatMessageKind.Incoming, Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam a mauris et neque facilisis pharetra condimentum ac nisi. Fusce tincidunt a sem eu fermentum. Curabitur volutpat enim turpis, et vulputate diam auctor a" });
            _messages.Add(new ChatMessage() { UserName = "Player", Time = DateTimeOffset.Now, Kind = ChatMessageKind.Outgoing, Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam a mauris et neque facilisis pharetra condimentum ac nisi. Fusce tincidunt a sem eu fermentum. Curabitur volutpat enim turpis, et vulputate diam auctor a" });
            _messages.Add(new ChatMessage() { UserName = "Franta Perníkáč", Time = DateTimeOffset.Now, Kind = ChatMessageKind.Incoming, Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam a mauris et neque facilisis pharetra condimentum ac nisi. Fusce tincidunt a sem eu fermentum. Curabitur volutpat enim turpis, et vulputate diam auctor a" });
            _messages.Add(new ChatMessage() { UserName = "Franta Perníkáč", Time = DateTimeOffset.Now, Kind = ChatMessageKind.Incoming, Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam a mauris et neque facilisis pharetra condimentum ac nisi. Fusce tincidunt a sem eu fermentum. Curabitur volutpat enim turpis, et vulputate diam auctor a" });
        }

        private void ChatService_MessageRecieved(object sender, ChatMessage e)
        {
            this.Dispatcher.RequestMainThreadAction(() => Messages.Add(e));
        }

        //public MvxCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new MvxCommand(() => ChatService.SendMessage(MessageText)));
        public MvxCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new MvxCommand(
            () => 
            {
                Messages.Add(new ChatMessage() { Kind = ChatMessageKind.Outgoing, Text = MessageText, Time = DateTimeOffset.Now, UserName = "Player" });
                MessageText = "";
            }, 
            () => MessageText != ""));
    }
}
