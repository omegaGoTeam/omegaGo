using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.GameCreationBundle;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;
        private readonly IDialogService _dialogService;

        private IMvxCommand _navigateToTutorial;
        private IMvxCommand _navigateToSingleplayer;
        private IMvxCommand _navigateToGameCreation;
        private IMvxCommand _navigateToMultiplayerDashboard;
        private IMvxCommand _navigateToLibrary;
        private IMvxCommand _navigateToStatistics;
        private IMvxCommand _navigateToSettings;
        private IMvxCommand _navigateToHelp;

        public bool ShowTutorialButton => _gameSettings.Display.ShowTutorialInMainMenu;

        public MainMenuViewModel( IGameSettings gameSettings, IDialogService dialogService )
        {
            _gameSettings = gameSettings;
            _dialogService = dialogService;
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

        // TODO Martin: fix this display with custom control
        private string DO_MUTE = "MUTE";
        private string DO_UNMUTE = "UNMUTE";

        public string MuteGlyph {
            get
            {
                return _gameSettings.Audio.Mute
                    ? "\uE74F" //"&#xE74F"
                    : "\uE995"; //"&#xE995";

            }
            set {
                if (value == DO_MUTE)
                {
                    _gameSettings.Audio.Mute = true;
                } else if (value == DO_UNMUTE)
                {
                    _gameSettings.Audio.Mute = false;
                }
                RaisePropertyChanged();
            } 
        } 

        public IMvxCommand ToggleMute => new MvxCommand(() =>
        {
            if (_gameSettings.Audio.Mute)
            {
                MuteGlyph = DO_UNMUTE;
            } else
            {
                MuteGlyph = DO_MUTE;
            }
        });

        private async void ShowLanguageChangeDialog()
        {
            await _dialogService.ShowAsync(Localizer.LanguageChangeInfo);
        }

        public IMvxCommand NavigateToTutorial => _navigateToTutorial ??
                                                 (_navigateToTutorial = new MvxCommand(() => ShowViewModel<TutorialViewModel>()));

        public IMvxCommand NavigateToSingleplayer => _navigateToSingleplayer ??
                                                     (_navigateToSingleplayer = new MvxCommand(() => ShowViewModel<SingleplayerViewModel>()));

        public IMvxCommand NavigateToGameCreation => _navigateToGameCreation ??
                                                     (_navigateToGameCreation = new MvxCommand(() =>
                                                     {
                                                         Mvx.RegisterSingleton<GameCreationBundle>(new HotseatBundle());
                                                         ShowViewModel<GameCreationViewModel>();
                                                     }));

        public IMvxCommand NavigateToMultiplayerDashboard => _navigateToMultiplayerDashboard ??
                                                             (_navigateToMultiplayerDashboard =
                                                                 new MvxCommand(() => ShowViewModel<IgsHomeViewModel>()));

        public IMvxCommand NavigateToLibrary => _navigateToLibrary ??
                                                (_navigateToLibrary = new MvxCommand(() => ShowViewModel<LibraryViewModel>()));

        public IMvxCommand NavigateToStatistics => _navigateToStatistics ??
                                                   (_navigateToStatistics = new MvxCommand(() => ShowViewModel<StatisticsViewModel>()));

        public IMvxCommand NavigateToSettings => _navigateToSettings ??
                                                 (_navigateToSettings = new MvxCommand(() => ShowViewModel<SettingsViewModel>()));

    

        public IMvxCommand NavigateToHelp => _navigateToHelp ?? (_navigateToHelp = new MvxCommand(() => ShowViewModel<HelpViewModel>()));

    }
}
