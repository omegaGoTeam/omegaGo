using OmegaGo.UI.ViewModels;
using System.Linq;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameCreationView : TransparencyViewBase
    {
        public GameCreationViewModel VM => (GameCreationViewModel)this.ViewModel;

        public GameCreationView()

        {
            this.InitializeComponent();
        }

        private void CompensationInput_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var keyChar = e.Key.ToString().FirstOrDefault();
            if ( !char.IsDigit( keyChar))
            {
                e.Handled = true;
            }
        }
    }
}
