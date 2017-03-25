using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform.Converters;

namespace OmegaGo.UI.Converters
{
    public class EnumSelectionConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumInstance = (Enum)value;
            return string.Equals(Enum.GetName(enumInstance.GetType(), enumInstance), parameter.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
            {
                return Enum.Parse(targetType, parameter.ToString());
            }
            return null;
        }
    }
}
