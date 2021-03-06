﻿using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Feedback;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Tsumego;

namespace OmegaGo.UI.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;
        private readonly IDialogService _dialogService;
        private readonly IFeedbackService _feedbackService;

        private IMvxCommand _navigateToTutorial;
        private IMvxCommand _navigateToSingleplayer;
        private IMvxCommand _navigateToGameCreation;
        private IMvxCommand _navigateToIgsHome;
        private IMvxCommand _navigateToKgsHome;
        private IMvxCommand _navigateToLibrary;
        private IMvxCommand _navigateToStatistics;
        private IMvxCommand _navigateToSettings;
        private IMvxCommand _navigateToHelp;

        private IMvxCommand _launchFeedbackCommand;
        
        public MainMenuViewModel(IGameSettings gameSettings, IDialogService dialogService, IFeedbackService feedbackService )
        {
            _gameSettings = gameSettings;
            _dialogService = dialogService;
            _feedbackService = feedbackService;
        }

        public bool ShowTutorialButton => _gameSettings.Display.ShowTutorialInMainMenu;

        public bool ShowFeedbackButton => _feedbackService.IsAvailable;

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

        // TODO (future work)  Martin: fix this display with custom control
        private string DO_MUTE = "MUTE";
        private string DO_UNMUTE = "UNMUTE";

        public string MuteGlyph
        {
            get
            {
                return _gameSettings.Audio.Mute
                    ? "\uE74F" 
                    : "\uE995"; 

            }
            set
            {
                if (value == DO_MUTE)
                {
                    _gameSettings.Audio.Mute = true;
                }
                else if (value == DO_UNMUTE)
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
            }
            else
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

        public IMvxCommand NavigateToIgsHome => _navigateToIgsHome ??
                                                             (_navigateToIgsHome =
                                                                 new MvxCommand(() => ShowViewModel<IgsHomeViewModel>()));

        public IMvxCommand NavigateToKgsHome => _navigateToKgsHome ??
                                                (_navigateToKgsHome =
                                                    new MvxCommand(() => ShowViewModel<KgsHomeViewModel>()));

        public IMvxCommand NavigateToLibrary => _navigateToLibrary ??
                                                (_navigateToLibrary = new MvxCommand(() => ShowViewModel<LibraryViewModel>()));

        public IMvxCommand NavigateToStatistics => _navigateToStatistics ??
                                                   (_navigateToStatistics = new MvxCommand(() => ShowViewModel<StatisticsViewModel>()));

        public IMvxCommand NavigateToSettings => _navigateToSettings ??
                                                 (_navigateToSettings = new MvxCommand(() => ShowViewModel<SettingsViewModel>()));



        public IMvxCommand NavigateToHelp => _navigateToHelp ?? (_navigateToHelp = new MvxCommand(() => ShowViewModel<HelpViewModel>()));

        public IMvxCommand LaunchFeedbackCommand => _launchFeedbackCommand ??
                                                    (_launchFeedbackCommand = new MvxAsyncCommand(LaunchFeedbackAsync));

        private async Task LaunchFeedbackAsync()
        {
            await _feedbackService.LaunchAsync();
        }
    }
}
