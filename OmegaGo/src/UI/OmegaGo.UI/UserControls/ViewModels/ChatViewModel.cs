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
    public sealed class ChatViewModel : ControlViewModelBase
    {
        private IChatService _chatService;
        private string _messageText;

        private MvxCommand _sendMessageCommand;
        
        public IChatService ChatService
        {
            get { return _chatService; }
            set
            {
                SetProperty(ref _chatService, value);
                _chatService.MessageReceived += ChatServiceMessageReceived;
            }
        }

        public ObservableCollection<ChatMessage> Messages { get; }

        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); SendMessageCommand.RaiseCanExecuteChanged(); }
        }

        private string _humanAuthor;
        public string HumanAuthor
        {
            get { return _humanAuthor; }
            set { SetProperty(ref _humanAuthor, value); }
        }

        public ChatViewModel()
        {
            _messageText = "";
            Messages = new ObservableCollection<ChatMessage>();
            /*
            Messages.Add(new ChatMessage() { UserName = "Franta Perníkáč", Time = DateTimeOffset.Now, Kind = ChatMessageKind.Incoming, Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam a mauris et neque facilisis pharetra condimentum ac nisi. Fusce tincidunt a sem eu fermentum. Curabitur volutpat enim turpis, et vulputate diam auctor a" });
            Messages.Add(new ChatMessage() { UserName = "Player", Time = DateTimeOffset.Now, Kind = ChatMessageKind.Outgoing, Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam a mauris et neque facilisis pharetra condimentum ac nisi. Fusce tincidunt a sem eu fermentum. Curabitur volutpat enim turpis, et vulputate diam auctor a" });
            Messages.Add(new ChatMessage() { UserName = "Franta Perníkáč", Time = DateTimeOffset.Now, Kind = ChatMessageKind.Incoming, Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam a mauris et neque facilisis pharetra condimentum ac nisi. Fusce tincidunt a sem eu fermentum. Curabitur volutpat enim turpis, et vulputate diam auctor a" });
            Messages.Add(new ChatMessage() { UserName = "Franta Perníkáč", Time = DateTimeOffset.Now, Kind = ChatMessageKind.Incoming, Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam a mauris et neque facilisis pharetra condimentum ac nisi. Fusce tincidunt a sem eu fermentum. Curabitur volutpat enim turpis, et vulputate diam auctor a" });*/
        }

        private void ChatServiceMessageReceived(object sender, ChatMessage e)
        {
            this.Dispatcher.RequestMainThreadAction(() => Messages.Add(e));
        }

        
        //public MvxCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new MvxCommand(() => ChatService.SendMessage(MessageText)));
        public MvxCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new MvxCommand(
            () =>
            {
                ChatService.SendMessage(MessageText);
                Messages.Add(new ChatMessage() { Kind = ChatMessageKind.Outgoing, Text = MessageText, Time = DateTimeOffset.Now, UserName = HumanAuthor });
                MessageText = "";
            }, 
            () => MessageText != ""));
    }
}
