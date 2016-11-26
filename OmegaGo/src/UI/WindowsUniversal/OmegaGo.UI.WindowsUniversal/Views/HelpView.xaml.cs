using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class HelpView : ViewBase
    {
        public HelpViewModel VM => (HelpViewModel)this.ViewModel;

        public HelpView()
        {
            this.InitializeComponent();
        }

        private void GoBack_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.GoBack();

        }
    }
}
