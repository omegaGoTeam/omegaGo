using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Chat
{
    public sealed class ChatMessage
    {
        private string _userName;
        private string _text;
        private DateTimeOffset _time;
        private ChatMessageKind _kind;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public DateTimeOffset Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public ChatMessageKind Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        public ChatMessage()
        {

        }
        public ChatMessage(string userName, string text, DateTimeOffset time, ChatMessageKind kind)
        {
            this._userName = userName;
            this._text = text;
            this._time = time;
            this._kind = kind;
        }
    }
}
