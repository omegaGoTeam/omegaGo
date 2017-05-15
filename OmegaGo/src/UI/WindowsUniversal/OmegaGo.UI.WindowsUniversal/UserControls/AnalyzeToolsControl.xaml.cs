using OmegaGo.UI.UserControls.ViewModels;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class AnalyzeToolsControl : UserControlBase
    {
        public static readonly DependencyProperty ViewModelProperty =
                DependencyProperty.Register(
                        "ViewModel",
                        typeof(AnalyzeToolsViewModel),
                        typeof(AnalyzeToolsControl),
                        new PropertyMetadata(null));

        public AnalyzeToolsViewModel ViewModel
        {
            get { return (AnalyzeToolsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public AnalyzeToolsControl()
        {
            this.InitializeComponent();
        }
    }
}
