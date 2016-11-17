using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class MainMenuView : ViewBase
    {
        public MainMenuViewModel VM => (MainMenuViewModel)this.ViewModel;

        public MainMenuView()
        {
            this.InitializeComponent();
        }

        private void ShowHideTips_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // TODO show or hide the RichTextBox with tips
        }
    }
}
