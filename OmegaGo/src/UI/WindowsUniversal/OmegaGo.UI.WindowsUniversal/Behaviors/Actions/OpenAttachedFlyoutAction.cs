using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace OmegaGo.UI.WindowsUniversal.Behaviors.Actions
{
    /// <summary>
    /// Interactivity Action that opens the sender's Attached flyout
    /// </summary>
    public class OpenAttachedFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var element = (FrameworkElement) sender;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
            return null;
        }
    }
}
