using OmegaGo.UI.UserControls.ViewModels;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class AnalysisControl : UserControlBase
    {
        public static readonly DependencyProperty ViewModelProperty =
                DependencyProperty.Register(
                        "ViewModel",
                        typeof(AnalysisViewModel),
                        typeof(AnalysisControl),
                        new PropertyMetadata(null));

        public AnalysisViewModel ViewModel
        {
            get { return (AnalysisViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public AnalysisControl()
        {
            this.InitializeComponent();
        }
    }
}
