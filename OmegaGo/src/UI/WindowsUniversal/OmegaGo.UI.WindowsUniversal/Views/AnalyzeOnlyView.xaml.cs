using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Infrastructure;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OmegaGo.UI.WindowsUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnalyzeOnlyView : TransparencyViewBase
    {
        public AnalyzeOnlyViewModel VM => (AnalyzeOnlyViewModel)ViewModel;

        public override string TabTitle => Localizer.AnalyzeMode;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Observe.png");


        public AnalyzeOnlyView()
        {
            this.InitializeComponent();
        }

        private void focusButton_Click(object sender, RoutedEventArgs e)
        {
            AppShell.FocusModeOn = !AppShell.FocusModeOn;
        }
    }
}
