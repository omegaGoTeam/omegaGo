
using Windows.UI.Xaml;
using OmegaGo.UI.ViewModels;
using System;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class TutorialView : TransparencyViewBase
    {
        public TutorialViewModel VM => (TutorialViewModel)this.ViewModel;

        public TutorialView()
        {
            this.InitializeComponent();
        }

        public override string WindowTitle => Localizer.Tutorial;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Tutorial.png");

        private void Scenario_ScenarioCompleted(object sender, System.EventArgs e)
        {
            Frame.GoBack();
        }

        private void TutorialView_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.K)
            {
                Next_Click(sender, new RoutedEventArgs());
            }
        }

        private void Next_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.ButtonNext.Visibility = Visibility.Collapsed;
            this.ButtonNext.Content = "Next";
            VM.Scenario.ClickNext();
        }

        private void Option1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.MakeYourChoiceDialog.Visibility = Visibility.Collapsed;
            VM.Scenario.ClickOption(0);
        }

        private void Option2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.MakeYourChoiceDialog.Visibility = Visibility.Collapsed;
            VM.Scenario.ClickOption(1);
        }


        private void TutorialViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.ButtonNext.Visibility = Visibility.Collapsed;
            this.MakeYourChoiceDialog.Visibility = Visibility.Collapsed;
            var scenario = VM.Scenario;
            scenario.ScenarioCompleted += Scenario_ScenarioCompleted;
            scenario.SetChoices += Scenario_SetChoices;
            scenario.SenseiMessageChanged += Scenario_SenseiMessageChanged;
            scenario.NextButtonShown += Scenario_NextButtonShown;
            scenario.NextButtonTextChanged += Scenario_NextButtonTextChanged;
            scenario.ExecuteCommand();

        }

        private void Scenario_NextButtonTextChanged(object sender, string e)
        {
            this.ButtonNext.Content = e;
        }

        private void Scenario_NextButtonShown(object sender, System.EventArgs e)
        {
            this.ButtonNext.Visibility = Visibility.Visible;
        }

        private void Scenario_SenseiMessageChanged(object sender, string e)
        {
            this.SenseisLine.Text = e;
        }

        private void Scenario_SetChoices(object sender, System.Tuple<string, string> e)
        {
            this.Option1Text.Text = e.Item1;
            this.Option2Text.Text = e.Item2;
            this.MakeYourChoiceDialog.Visibility = Visibility.Visible;
        }

        private void BoardControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            VM.TapBoardControl();
        }
    }
}
