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
    /// <summary>
    /// Converter that localizes the given value as resource keys
    /// </summary>
    public class LocalizingConverter : MvxValueConverter<string, string>
    {
        private static readonly Lazy<ILocalizationService> Localizer = new Lazy<ILocalizationService>( Mvx.Resolve<ILocalizationService> );        

        protected override string Convert( string value, Type targetType, object parameter, CultureInfo culture ) => Localizer.Value[ value ];
    }
}
