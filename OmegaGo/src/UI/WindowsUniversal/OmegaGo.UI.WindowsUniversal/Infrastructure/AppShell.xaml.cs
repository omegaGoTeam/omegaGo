using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure
{
    /// <summary>
    /// The Shell around the whole app
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public AppShell()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Custom app title bar
        /// </summary>
        public FrameworkElement AppTitleBar => TitleBar;

        /// <summary>
        /// Main frame that hosts app views
        /// </summary>
        public Frame MainFrame => Frame;
    }
}
