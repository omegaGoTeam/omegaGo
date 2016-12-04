using MvvmCross.Platform;
using MvvmCross.Platform.Converters;
using OmegaGo.UI.Services.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Converters
{
    public class EnumLocalizingConverter : MvxValueConverter<Enum, string>
    {
        private static readonly Lazy<ILocalizationService> Localizer = new Lazy<ILocalizationService>(Mvx.Resolve<ILocalizationService>);

        protected override string Convert(Enum value, Type targetType, object parameter, CultureInfo culture) => Localizer.Value[value.ToString()];
    }
}
