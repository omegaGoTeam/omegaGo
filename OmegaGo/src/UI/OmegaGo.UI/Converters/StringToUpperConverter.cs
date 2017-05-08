using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace OmegaGo.UI.Converters
{
    public class StringToUpperConverter : MvxValueConverter<string,string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToUpper();
        }
    }
}
