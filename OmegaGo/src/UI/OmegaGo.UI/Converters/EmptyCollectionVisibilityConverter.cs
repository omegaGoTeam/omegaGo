using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Platform.Converters;
using MvvmCross.Platform.UI;
using MvvmCross.Plugins.Visibility;

namespace OmegaGo.UI.Converters
{
    public class EmptyCollectionVisibilityConverter : MvxBaseVisibilityValueConverter<ICollection>
    {
        protected override MvxVisibility Convert(ICollection value, object parameter, CultureInfo culture)
        {
            return value.Count > 0 ? MvxVisibility.Collapsed : MvxVisibility.Visible;
        }
    }
}
