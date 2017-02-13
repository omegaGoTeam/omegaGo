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
    /// Localizing converter using the TypeName of the provided instance.
    /// ConverterParameter can also provide a format string.
    /// </summary>
    public class TypeNameLocalizingConverter : MvxValueConverter
    {
        private static readonly Lazy<ILocalizationService> Localizer = new Lazy<ILocalizationService>(Mvx.Resolve<ILocalizationService>);

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typeName = value.GetType().Name;
            var formatString = (parameter as string) ?? "{0}";
            return Localizer.Value[string.Format(formatString, typeName)];
        }
    }
}
