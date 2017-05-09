using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public class MultiplayerLobbyViewBase : TransparencyViewBase
    {
        private const string BlurBehaviorElementName = "BlurBehavior";

        public void Blur()
        {
            var blur = FindName(BlurBehaviorElementName) as Blur;
            if (blur != null) blur.Value = 3;
        }

        public void Unblur()
        {
            var blur = FindName(BlurBehaviorElementName) as Blur;
            if (blur != null) blur.Value = 0;
        }
    }
}
