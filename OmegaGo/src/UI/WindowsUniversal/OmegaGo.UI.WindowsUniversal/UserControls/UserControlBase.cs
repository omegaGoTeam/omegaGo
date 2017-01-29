using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    /// <summary>
    /// Base for custom user controls
    /// </summary>
    public abstract class UserControlBase : UserControl
    {
        private Localizer _localizer = null;

        /// <summary>
        /// String localizer
        /// </summary>
        public Localizer Localizer => _localizer ?? (_localizer = new Localizer());
    }
}