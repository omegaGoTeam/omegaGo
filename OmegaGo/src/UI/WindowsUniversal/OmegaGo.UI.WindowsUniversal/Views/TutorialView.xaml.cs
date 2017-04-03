
using Windows.UI.Xaml;
using OmegaGo.UI.ViewModels;
using System;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    // ReSharper disable once UnusedMember.Global
    public sealed partial class TutorialView
    {
        public TutorialViewModel VM => (TutorialViewModel)this.ViewModel;

        public TutorialView()
        {
            InitializeComponent();
        }

        public override string TabTitle => this.Localizer.Tutorial;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Tutorial.png");

        private void Scenario_ScenarioCompleted(object sender, EventArgs e)
        {
            this.Frame.GoBack();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonNext.Visibility = Visibility.Collapsed;
            this.ButtonNext.Content = Localizer.Tutorial_Next;
            this.VM.Scenario.ClickNext();
        }

        private void Option1_Click(object sender, RoutedEventArgs e)
        {
            this.MakeYourChoiceDialog.Visibility = Visibility.Collapsed;
            this.VM.Scenario.ClickOption(0);
        }

        private void Option2_Click(object sender, RoutedEventArgs e)
        {
            this.MakeYourChoiceDialog.Visibility = Visibility.Collapsed;
            this.VM.Scenario.ClickOption(1);
        }


        private void TutorialViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.ButtonNext.Visibility = Visibility.Collapsed;
            this.MakeYourChoiceDialog.Visibility = Visibility.Collapsed;
            var scenario = this.VM.Scenario;
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

        private void Scenario_NextButtonShown(object sender, EventArgs e)
        {
            this.ButtonNext.Visibility = Visibility.Visible;
        }

        private void Scenario_SenseiMessageChanged(object sender, string e)
        {
            this.SenseisLine.Text = e;
        }

        private void Scenario_SetChoices(object sender, Tuple<string, string> e)
        {
            this.Option1Text.Text = e.Item1;
            this.Option2Text.Text = e.Item2;
            this.MakeYourChoiceDialog.Visibility = Visibility.Visible;
        }

        private void BoardControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.VM.TapBoardControl();
        }
    }
}
