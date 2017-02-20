using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.Services.Notifications
{
    public class BubbleNotification : ControlViewModelBase
    {
        private string _text;

        public BubbleNotification(string text)
        {
            Text = text;
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private bool _closeable = true;

        public bool Closeable
        {
            get { return _closeable; }
            set { SetProperty(ref _closeable, value); }
        }
    }
}
