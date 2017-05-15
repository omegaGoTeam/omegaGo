using OmegaGo.UI.ViewModels;
using System;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OmegaGo.UI.WindowsUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ObserverGameView : TransparencyViewBase
    {
        public ObserverGameViewModel VM => (ObserverGameViewModel)ViewModel;

        public override string TabTitle => Localizer.Observing;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Observe.png");


        public ObserverGameView()
        {
            this.InitializeComponent();
            this.KeyUp += View_KeyUp;
        }
        
        private void focusButton_Click(object sender, RoutedEventArgs e)
        {
            AppShell.FocusModeOn = !AppShell.FocusModeOn;
        }

        private void View_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            // If the event comes from timeline slider, just ignor it. Slider is able to do "speedy navigation".
            if (e.OriginalSource == gameTimelineSlider)
                return;
            
            if (VM.IsAnalyzeModeEnabled)
            {
                switch (e.Key)
                {
                    case Windows.System.VirtualKey.Left:
                        gameTreeControl.GoToParentNode();
                        break;
                    case Windows.System.VirtualKey.Right:
                        gameTreeControl.GoToFirstChildNode();
                        break;
                    case Windows.System.VirtualKey.Up:
                        gameTreeControl.GoToPreviousLevelNode();
                        break;
                    case Windows.System.VirtualKey.Down:
                        gameTreeControl.GoToNextLevelNode();
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Windows.System.VirtualKey.Left:
                        gameTimelineSlider.Value--;
                        break;
                    case Windows.System.VirtualKey.Right:
                        gameTimelineSlider.Value++;
                        break;
                }
            }
        }

        private void layoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            gameTimelineSlider.Focus(FocusState.Programmatic);
            
            systemLog.Items.VectorChanged += (s, ev) =>
            {
                if (ev.CollectionChange == CollectionChange.ItemInserted)
                {
                    object newObject = systemLog.Items[(int)ev.Index];

                    systemLog.ScrollIntoView(newObject);
                }
            };
        }
    }
}
