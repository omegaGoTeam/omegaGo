using MvvmCross.Platform.Converters;
using MvvmCross.Platform.WindowsCommon.Converters;
using OmegaGo.Core.Modes.LiveGame.Phases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Globalization;

namespace OmegaGo.UI.WindowsUniversal.Converters
{
    public sealed class GamePhaseVisibilityConverter : IValueConverter
    {
        public GamePhaseType RequiredGamePhase
        {
            get;
            set;
        }
                
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            GamePhaseType currentPhase = (GamePhaseType)value;

            if (currentPhase == RequiredGamePhase)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
