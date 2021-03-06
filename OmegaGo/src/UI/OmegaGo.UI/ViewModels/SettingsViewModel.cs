﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using OmegaGo.Core.AI;
using OmegaGo.UI.Board.Styles;
using OmegaGo.UI.Controls.Styles;
using OmegaGo.UI.Controls.Themes;
using OmegaGo.UI.Game.Styles;
using OmegaGo.UI.Infrastructure.PresentationHints;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    /// <summary>
    ///     ViewModel for the settings
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;

        private bool _controlStyleChanged;

        private bool _languageChanged;

        public SettingsViewModel(IGameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            var program = this.SelectedAiProgram;
            this.AssistantSettingsViewModel =
                new PlayerSettingsViewModel(
                    new GameCreationViewAiPlayer(program), true);
        }

        public ObservableCollection<ControlStyle> ControlStyles { get; } =
            new ObservableCollection<ControlStyle>((ControlStyle[]) Enum.GetValues(typeof(ControlStyle)));

        public ControlStyle SelectedControlStyle
        {
            get { return _gameSettings.Display.ControlStyle; }
            set
            {
                if (_gameSettings.Display.ControlStyle != value)
                {
                    _gameSettings.Display.ControlStyle = value;
                    RaisePropertyChanged();
                    this.ControlStyleChanged = true;
                }
            }
        }

        public bool ControlStyleChanged
        {
            get { return _controlStyleChanged; }
            set { SetProperty(ref _controlStyleChanged, value); }
        }

        /// <summary>
        ///     Game languages list
        /// </summary>
        public ObservableCollection<GameLanguage> Languages { get; } =
            new ObservableCollection<GameLanguage>(GameLanguages.SupportedLanguages.Values);

        /// <summary>
        ///     Selected language
        /// </summary>
        public GameLanguage SelectedLanguage
        {
            get
            {
                if (GameLanguages.SupportedLanguages.ContainsKey(_gameSettings.Language))
                {
                    return GameLanguages.SupportedLanguages[_gameSettings.Language];
                }
                return GameLanguages.DefaultLanguage;
            }
            set
            {
                if (value != null)
                {
                    if (_gameSettings.Language != value.CultureTag)
                    {
                        _gameSettings.Language = value.CultureTag;
                        RaisePropertyChanged();
                        this.LanguageChanged = true;
                    }
                }
            }
        }


        /// <summary>
        ///     Indicated whether the user has changed the language selection at least once
        /// </summary>
        public bool LanguageChanged
        {
            get { return _languageChanged; }
            set { SetProperty(ref _languageChanged, value); }
        }

        // Display 
        public ObservableCollection<BoardTheme> BoardThemes { get; } =
            new ObservableCollection<BoardTheme>(
                ((BoardTheme[]) Enum.GetValues(typeof(BoardTheme))));

        public int SelectedBoardTheme
        {
            get { return (int) _gameSettings.Display.BoardTheme; }
            set
            {
                _gameSettings.Display.BoardTheme = (BoardTheme) value;
                RaisePropertyChanged();
            }
        }

        public bool AddGraceSecond
        {
            get { return _gameSettings.Display.AddGraceSecond; }
            set
            {
                _gameSettings.Display.AddGraceSecond = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowTutorialInMainMenu
        {
            get { return _gameSettings.Display.ShowTutorialInMainMenu; }
            set
            {
                _gameSettings.Display.ShowTutorialInMainMenu = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<StoneTheme> StoneThemes { get; } =
            new ObservableCollection<StoneTheme>((StoneTheme[]) Enum.GetValues(typeof(StoneTheme)));

        public int SelectedStonesTheme
        {
            get { return (int) _gameSettings.Display.StonesTheme; }
            set
            {
                _gameSettings.Display.StonesTheme = (StoneTheme) value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<BackgroundImage> BackgroundImages { get; } =
            new ObservableCollection<BackgroundImage>((BackgroundImage[]) Enum.GetValues(typeof(BackgroundImage)));

        public BackgroundImage SelectedBackgroundImage
        {
            get { return _gameSettings.Display.BackgroundImage; }
            set
            {
                if (_gameSettings.Display.BackgroundImage != value)
                {
                    _gameSettings.Display.BackgroundImage = value;
                    RaisePropertyChanged();
                    ChangePresentation(new RefreshDisplayPresentationHint());
                }
            }
        }

        public ObservableCollection<AppTheme> AppThemes { get; } =
            new ObservableCollection<AppTheme>((AppTheme[]) Enum.GetValues(typeof(AppTheme)));

        public AppTheme SelectedAppTheme
        {
            get { return _gameSettings.Display.AppTheme; }
            set
            {
                if (_gameSettings.Display.AppTheme != value)
                {
                    _gameSettings.Display.AppTheme = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(() => this.AppThemeLightSelected);
                    RaisePropertyChanged(() => this.AppThemeDarkSelected);
                    ChangePresentation(new RefreshDisplayPresentationHint());
                }
            }
        }

        public bool AppThemeLightSelected
        {
            get { return this.SelectedAppTheme == AppTheme.Light; }
            set
            {
                if (value)
                {
                    this.SelectedAppTheme = AppTheme.Light;
                }
            }
        }

        public bool AppThemeDarkSelected
        {
            get { return this.SelectedAppTheme == AppTheme.Dark; }
            set
            {
                if (value)
                {
                    this.SelectedAppTheme = AppTheme.Dark;
                }
            }
        }

        public float BackgroundImageOpacity
        {
            get { return _gameSettings.Display.BackgroundColorOpacity*100.0f; }
            set
            {
                if (Math.Abs(value - _gameSettings.Display.BackgroundColorOpacity) > 0.1)
                {
                    _gameSettings.Display.BackgroundColorOpacity = value/100.0f;
                    RaisePropertyChanged();
                    ChangePresentation(new RefreshDisplayPresentationHint());
                }
            }
        }

        public BackgroundColor SelectedBackgroundColor
        {
            get { return _gameSettings.Display.BackgroundColor; }
            set
            {
                if (value != _gameSettings.Display.BackgroundColor)
                {
                    _gameSettings.Display.BackgroundColor = value;
                    RaisePropertyChanged();
                    ChangePresentation(new RefreshDisplayPresentationHint());
                }
            }
        }

        public bool HighlightLastMove
        {
            get { return _gameSettings.Display.HighlightLastMove; }
            set
            {
                _gameSettings.Display.HighlightLastMove = value;
                RaisePropertyChanged();
            }
        }

        public bool HighlightRecentCaptures
        {
            get { return _gameSettings.Display.HighlightRecentCaptures; }
            set
            {
                _gameSettings.Display.HighlightRecentCaptures = value;
                RaisePropertyChanged();
            }
        }

        public bool HighlightIllegalKoMoves
        {
            get { return _gameSettings.Display.HighlightIllegalKoMoves; }
            set
            {
                _gameSettings.Display.HighlightIllegalKoMoves = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowCoordinates
        {
            get { return _gameSettings.Display.ShowCoordinates; }
            set
            {
                _gameSettings.Display.ShowCoordinates = value;
                RaisePropertyChanged();
            }
        }

        // Input
        public bool InputConfirmation
        {
            get { return _gameSettings.InputConfirmationRequired; }
            set
            {
                _gameSettings.InputConfirmationRequired = value;
                RaisePropertyChanged();
            }
        }

        public bool AddTouchInputOffset
        {
            get { return _gameSettings.Display.AddTouchInputOffset; }
            set
            {
                _gameSettings.Display.AddTouchInputOffset = value;
                RaisePropertyChanged();
            }
        }

        // Audio
        public int MasterVolume
        {
            get { return _gameSettings.Audio.MasterVolume; }
            set
            {
                _gameSettings.Audio.MasterVolume = value;
                RaisePropertyChanged();
            }
        }

        public bool MuteAll
        {
            get { return _gameSettings.Audio.Mute; }
            set
            {
                _gameSettings.Audio.Mute = value;
                RaisePropertyChanged();
            }
        }

        public int MusicVolume
        {
            get { return _gameSettings.Audio.MusicVolume; }
            set
            {
                _gameSettings.Audio.MusicVolume = value;
                RaisePropertyChanged();
            }
        }

        public int SfxVolume
        {
            get { return _gameSettings.Audio.SfxVolume; }
            set
            {
                if (_gameSettings.Audio.SfxVolume != value)
                {
                    _gameSettings.Audio.SfxVolume = value;
                    RaisePropertyChanged();
                    PlaySampleSound();
                }
            }
        }

        public bool PlayWhenYouPlaceStone
        {
            get { return _gameSettings.Audio.PlayWhenYouPlaceStone; }
            set
            {
                _gameSettings.Audio.PlayWhenYouPlaceStone = value;
                RaisePropertyChanged();
            }
        }

        public bool PlayWhenOthersPlaceStone
        {
            get { return _gameSettings.Audio.PlayWhenOthersPlaceStone; }
            set
            {
                _gameSettings.Audio.PlayWhenOthersPlaceStone = value;
                RaisePropertyChanged();
            }
        }

        public bool PlayWhenNotificationReceived
        {
            get { return _gameSettings.Audio.PlayWhenNotificationReceived; }
            set
            {
                _gameSettings.Audio.PlayWhenNotificationReceived = value;
                RaisePropertyChanged();
            }
        }

        // AI
        public PlayerSettingsViewModel AssistantSettingsViewModel { get; }

        public ObservableCollection<IAIProgram> AiPrograms { get; } = new ObservableCollection<IAIProgram>(
            AISystems.AIPrograms
            );

        public IAIProgram SelectedAiProgram
        {
            get
            {
                var program = ProgramFromClassName(_gameSettings.Assistant.ProgramName);
                if (program == null)
                {
                    program = this.AiPrograms.Last();
                    _gameSettings.Assistant.ProgramName = program.GetType().Name;
                }
                return program;
            }
            set
            {
                _gameSettings.Assistant.ProgramName = value.GetType().Name;
                this.AssistantSettingsViewModel.ChangePlayer(new GameCreationViewAiPlayer(value));
                //RaisePropertyChanged();
            }
        }

        public bool EnableHints
        {
            get { return _gameSettings.Assistant.EnableHints; }
            set
            {
                _gameSettings.Assistant.EnableHints = value;
                RaisePropertyChanged();
            }
        }

        public bool EnableInOnlineGames
        {
            get { return _gameSettings.Assistant.EnableInOnlineGames; }
            set
            {
                _gameSettings.Assistant.EnableInOnlineGames = value;
                RaisePropertyChanged();
            }
        }

        private IAIProgram ProgramFromClassName(string name)
        {
            foreach (var program in this.AiPrograms)
            {
                if (program.GetType().Name == name)
                {
                    return program;
                }
            }
            return null;
        }

        /// <summary>
        ///     Plays a sample sound
        /// </summary>
        private async void PlaySampleSound()
        {
            await Sounds.VolumeTestSound.PlayAsync();
        }
    }
}