using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Coding4Fun.Toolkit.Controls;
using OmegaGo.Core.Online.Kgs.Structures;
using VisualTreeExtensions = Microsoft.Toolkit.Uwp.UI.VisualTreeExtensions;

namespace OmegaGo.UI.WindowsUniversal.DataTemplateSelectors.Multiplayer.KGS
{
    public class KgsGameContainerComboBoxTemplateSelector : DataTemplateSelector
    {
        private readonly FrameworkElement _container;

        public KgsGameContainerComboBoxTemplateSelector(FrameworkElement container)
        {
            _container = container;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is KgsGlobalGamesList)
            {

                return (DataTemplate)_container.Resources["KgsGlobalChannelTemplate"];
            }
            else
            {
                return (DataTemplate)_container.Resources["KgsRoomChannelTemplate"];
            }
        }
    }
}
