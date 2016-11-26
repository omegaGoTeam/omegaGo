
using Windows.UI.Xaml;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class TutorialView : ViewBase
    {
        public TutorialViewModel VM => (TutorialViewModel)this.ViewModel;

        public TutorialView()
        {
            this.InitializeComponent();
        }

        private void Scenario_ScenarioCompleted(object sender, System.EventArgs e)
        {
            Frame.GoBack();
        }

        private void Scenario_NextButtonVisibilityChanged(object sender, bool e)
        {
            this.ButtonNext.Visibility = e ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TutorialView_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.K)
            {
                // TODO don't send click, if such is impossible, to avoid throwing exceptions
                Next_Click(sender, new RoutedEventArgs());
            }
        }

        private void Next_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.Scenario.CurrentLine?.Next(VM.Scenario);
        }

        private void Option1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.Scenario.CurrentLine?.SelectOption(0, VM.Scenario);
        }

        private void Option2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.Scenario.CurrentLine?.SelectOption(1, VM.Scenario);
        }


        private void TutorialViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            var scenario = VM.Scenario;
            scenario.NextButtonVisibilityChanged += Scenario_NextButtonVisibilityChanged;
            scenario.ScenarioCompleted += Scenario_ScenarioCompleted;
            scenario.ClearChoices += Scenario_ClearChoices;
            scenario.SetChoices += Scenario_SetChoices;
            scenario.FirstLine.Speak(VM.Scenario);

        }

        private void Scenario_SetChoices(object sender, System.Tuple<string, string> e)
        {
            this.Option1Text.Text = e.Item1;
            this.Option2Text.Text = e.Item2;
            this.MakeYourChoiceDialog.Visibility = Visibility.Visible;
        }

        private void Scenario_ClearChoices(object sender, System.EventArgs e)
        {
            this.MakeYourChoiceDialog.Visibility = Visibility.Collapsed;
        }
    }
}
