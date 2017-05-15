using Windows.UI;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using OmegaGo.UI.WindowsUniversal.Helpers.UI;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class HamburgerButton : Button
    {
        public HamburgerButton()
        {
            this.InitializeComponent();
        }

        private void HamburgerButton_Loaded(object sender, RoutedEventArgs e)
        {
            IGameSettings gameSettings = Mvx.Resolve<IGameSettings>();

            if (gameSettings.Display.ControlStyle != Controls.Styles.ControlStyle.OperatingSystem)
            {
                HamburgerButtonBorder.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
