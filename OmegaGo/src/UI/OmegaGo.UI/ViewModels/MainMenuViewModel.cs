using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;
        private IMvxCommand _navigateToTutorial;
        private IMvxCommand _navigateToSingleplayer;
        private IMvxCommand _navigateToGameCreation;
        private IMvxCommand _navigateToMultiplayerDashboard;
        private IMvxCommand _navigateToLibrary;
        private IMvxCommand _navigateToStatistics;
        private IMvxCommand _navigateToSettings;
        private IMvxCommand _navigateToAbout;
        private IMvxCommand _navigateToHelp;

        public MainMenuViewModel( IGameSettings gameSettings )
        {
            _gameSettings = gameSettings;
        }

        /// <summary>
        /// Game languages list
        /// </summary>
        public ObservableCollection<GameLanguage> Languages { get; } =
            new ObservableCollection<GameLanguage>(GameLanguages.SupportedLanguages.Values);

        /// <summary>
        /// Selected language
        /// </summary>
        public GameLanguage SelectedLanguage
        {
            get
            {
                if (GameLanguages.SupportedLanguages.ContainsKey(_gameSettings.Language))
                {
                    return GameLanguages.SupportedLanguages[_gameSettings.Language];
                }
                else
                {
                    return GameLanguages.DefaultLanguage;
                }
            }
            set
            {
                if (value != null)
                {
                    if (_gameSettings.Language != value.CultureTag)
                    {
                        _gameSettings.Language = value.CultureTag;
                        RaisePropertyChanged();
                        ShowLanguageChangeDialog();
                    }
                }
            }
        }

        private async void ShowLanguageChangeDialog()
        {
            MessageDialog dialog = new MessageDialog(Localizer.LanguageChangeInfo);
            dialog.ShowAsync()
        }

        public IMvxCommand NavigateToTutorial => _navigateToTutorial ??
                                                 (_navigateToTutorial = new MvxCommand(() => ShowViewModel<TutorialViewModel>()));

        public IMvxCommand NavigateToSingleplayer => _navigateToSingleplayer ??
                                                     (_navigateToSingleplayer = new MvxCommand(() => ShowViewModel<SingleplayerViewModel>()));

        public IMvxCommand NavigateToGameCreation => _navigateToGameCreation ??
                                                     (_navigateToGameCreation = new MvxCommand(() => ShowViewModel<GameCreationViewModel>()));

        public IMvxCommand NavigateToMultiplayerDashboard => _navigateToMultiplayerDashboard ??
                                                             (_navigateToMultiplayerDashboard =
                                                                 new MvxCommand(() => ShowViewModel<MultiplayerDashboardViewModel>()));

        public IMvxCommand NavigateToLibrary => _navigateToLibrary ??
                                                (_navigateToLibrary = new MvxCommand(() => ShowViewModel<LibraryViewModel>()));

        public IMvxCommand NavigateToStatistics => _navigateToStatistics ??
                                                   (_navigateToStatistics = new MvxCommand(() => ShowViewModel<StatisticsViewModel>()));

        public IMvxCommand NavigateToSettings => _navigateToSettings ??
                                                 (_navigateToSettings = new MvxCommand(() => ShowViewModel<SettingsViewModel>()));

        public IMvxCommand NavigateToAbout => _navigateToAbout ?? (_navigateToAbout = new MvxCommand(() => ShowViewModel<AboutViewModel>()));

        public IMvxCommand NavigateToHelp => _navigateToHelp ?? (_navigateToHelp = new MvxCommand(() => ShowViewModel<HelpViewModel>()));


    }
}
