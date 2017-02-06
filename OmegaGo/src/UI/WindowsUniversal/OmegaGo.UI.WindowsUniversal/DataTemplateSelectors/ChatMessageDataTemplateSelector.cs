using OmegaGo.Core.Online.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OmegaGo.UI.WindowsUniversal.DataTemplateSelectors
{
    public sealed class ChatMessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IncomingTemplate { get; set; }
        public DataTemplate OutgoingTemplate { get; set; }
        
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            ChatMessage chatMessage = (ChatMessage)item;

            return chatMessage.Kind == ChatMessageKind.Incoming ?
                IncomingTemplate : 
                OutgoingTemplate;
        }
    }
}
