﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Chat
{
    public sealed class ChatMessage
    {
        public string UserName { get;  }

        public string Text { get; set; }

        public DateTimeOffset Time { get;  }

        public string TimeString => Time.ToString("HH:mm");

        public ChatMessageKind Kind { get; }

        public ChatMessage(string userName, string text, DateTimeOffset time, ChatMessageKind kind)
        {
#if DEBUG
            if (String.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Chat senders must have names.", nameof(userName));
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Chat messages can't be empty.", nameof(text));
#endif
            this.UserName = userName;
            this.Text = text;
            this.Time = time;
            this.Kind = kind;
        }
    }
}
