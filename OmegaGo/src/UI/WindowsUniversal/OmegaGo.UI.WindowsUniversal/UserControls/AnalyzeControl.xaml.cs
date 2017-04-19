using OmegaGo.UI.UserControls.ViewModels;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class AnalyzeControl : UserControlBase
    {
        public static readonly DependencyProperty ViewModelProperty =
                DependencyProperty.Register(
                        "ViewModel",
                        typeof(AnalyzeViewModel),
                        typeof(AnalyzeControl),
                        new PropertyMetadata(null));

        public AnalyzeViewModel ViewModel
        {
            get { return (AnalyzeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public AnalyzeControl()
        {
            this.InitializeComponent();
        }
    }
}
