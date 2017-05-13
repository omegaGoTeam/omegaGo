using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform.UI;
using MvvmCross.Plugins.Visibility;

namespace OmegaGo.UI.Converters
{
    public class NonEmptyStringVisibilityConverter : MvxBaseVisibilityValueConverter<string>
    {
        protected override MvxVisibility Convert(string value, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value) ? MvxVisibility.Visible : MvxVisibility.Collapsed;
        }
    }
}
