using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Models.Log
{
    public sealed class LogMessage
    {
        public string Text { get; set; }

        public LogMessage()
        {

        }

        public LogMessage(string text)
        {
            Text = text;
        }
    }
}
