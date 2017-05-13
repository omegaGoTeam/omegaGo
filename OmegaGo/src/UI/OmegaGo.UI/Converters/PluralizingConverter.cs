using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using MvvmCross.Platform.Converters;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.Converters
{
    public class PluralizingConverter : MvxValueConverter<int, string>
    {
        private static readonly Lazy<ILocalizationService> Localizer =
            new Lazy<ILocalizationService>(Mvx.Resolve<ILocalizationService>);

        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = Math.Abs(value);
            if (intValue == 1) return Localizer.Value[parameter + "1"];
            if (intValue > 1 && intValue < 5) return Localizer.Value[parameter + "2"];
            //0 or 5 and more
            return Localizer.Value[parameter + "5"];
        }
    }
}
