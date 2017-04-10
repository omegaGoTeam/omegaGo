using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Converters
{
    public sealed class BoolDoubleConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inputBool = (bool)value;

            if (inputBool)
            {
                return 1d;
            }

            return 0d;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double inputInteger = (double)value;

            if (inputInteger == 1d)
            {
                return true;
            }

            if (inputInteger == 0d)
            {
                return false;
            }

            return null;
        }
    }
}
