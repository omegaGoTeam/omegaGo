using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the settings
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;

        public SettingsViewModel( IGameSettings gameSettings )
        {
            _gameSettings = gameSettings;
        }

        /// <summary>
        /// Game languages list
        /// </summary>
        public ObservableCollection<GameLanguage> Languages { get; } =
            new ObservableCollection<GameLanguage>( GameLanguages.SupportedLanguages.Values );

        /// <summary>
        /// Selected language
        /// </summary>
        public GameLanguage SelectedLanguage
        {
            get
            {
                if ( GameLanguages.SupportedLanguages.ContainsKey( _gameSettings.Language ) )
                {
                    return GameLanguages.SupportedLanguages[ _gameSettings.Language ];
                }
                else
                {
                    return GameLanguages.DefaultLanguage;
                }
            }
            set
            {
                if ( value != null )
                {
                    if ( _gameSettings.Language != value.CultureTag )
                    {
                        _gameSettings.Language = value.CultureTag;
                        RaisePropertyChanged();
                        LanguageChanged = true;
                    }
                }
            }
        }

        private bool _languageChanged = false;

        /// <summary>
        /// Indicated whether the user has changed the language selection at least once
        /// </summary>
        public bool LanguageChanged
        {
            get
            {
                return _languageChanged;
            }
            set { SetProperty( ref _languageChanged, value ); }
        }

        // Display 
        public ObservableCollection<BoardTheme> BoardThemes { get; } =
            new ObservableCollection<BoardTheme>( (BoardTheme[])Enum.GetValues(typeof(BoardTheme)) );
        public int SelectedBoardTheme
        {
            get { return (int)_gameSettings.Display.BoardTheme; }
            set { _gameSettings.Display.BoardTheme = (BoardTheme)value; RaisePropertyChanged(); }
        }
        public bool ShowTutorialInMainMenu
        {
            get { return _gameSettings.Display.ShowTutorialInMainMenu; }
            set { _gameSettings.Display.ShowTutorialInMainMenu = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<StoneTheme> StoneThemes { get; } =
         new ObservableCollection<StoneTheme>((StoneTheme[])Enum.GetValues(typeof(StoneTheme)));
        public int SelectedStonesTheme
        {
            get { return (int)_gameSettings.Display.StonesTheme; }
            set { _gameSettings.Display.StonesTheme = (StoneTheme)value; RaisePropertyChanged(); }
        }
        public bool HighlightLastMove
        {
            get { return _gameSettings.Display.HighlightLastMove; }
            set { _gameSettings.Display.HighlightLastMove = value; RaisePropertyChanged(); }
        }
        public bool HighlightRecentCaptures
        {
            get { return _gameSettings.Display.HighlightRecentCaptures; }
            set { _gameSettings.Display.HighlightRecentCaptures = value; RaisePropertyChanged(); }
        }
        public bool HighlightIllegalKoMoves
        {
            get { return _gameSettings.Display.HighlightIllegalKoMoves; }
            set { _gameSettings.Display.HighlightIllegalKoMoves = value; RaisePropertyChanged(); }
        }
        public bool ShowCoordinates
        {
            get { return _gameSettings.Display.ShowCoordinates; }
            set { _gameSettings.Display.ShowCoordinates = value; RaisePropertyChanged(); }
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
        // Audio
        public int MasterVolume
        {
            get { return _gameSettings.Audio.MasterVolume; }
            set { _gameSettings.Audio.MasterVolume = value; RaisePropertyChanged();
            }
        }
        public int MusicVolume
        {
            get { return _gameSettings.Audio.MusicVolume; }
            set { _gameSettings.Audio.MusicVolume = value; RaisePropertyChanged(); }
        }
        public int SfxVolume
        {
            get { return _gameSettings.Audio.SfxVolume; }
            set { _gameSettings.Audio.SfxVolume = value; RaisePropertyChanged();
            }
        }
        public bool PlayWhenYouPlaceStone
        {
            get { return _gameSettings.Audio.PlayWhenYouPlaceStone; }
            set { _gameSettings.Audio.PlayWhenYouPlaceStone = value; RaisePropertyChanged(); }
        }
        public bool PlayWhenOthersPlaceStone
        {
            get { return _gameSettings.Audio.PlayWhenOthersPlaceStone; }
            set { _gameSettings.Audio.PlayWhenOthersPlaceStone = value; RaisePropertyChanged(); }
        }
        public bool PlayWhenNotificationReceived
        {
            get { return _gameSettings.Audio.PlayWhenNotificationReceived; }
            set { _gameSettings.Audio.PlayWhenNotificationReceived = value; RaisePropertyChanged(); }
        }
        // AI
        public ObservableCollection<string> AiPrograms { get; } =
            new ObservableCollection<string>(OmegaGo.Core.AI.AISystems.AiPrograms.Select(program => program.Name));
        public string SelectedAiProgram
        {
            get { return _gameSettings.Assistant.ProgramName; }
            set
            {
                _gameSettings.Assistant.ProgramName = value;
                RaisePropertyChanged();
            }
        }

        public bool EnableHints
        {
            get { return _gameSettings.Assistant.EnableHints; }
            set { _gameSettings.Assistant.EnableHints = value; RaisePropertyChanged(); }
        }
        public bool EnableInOnlineGames
        {
            get { return _gameSettings.Assistant.EnableInOnlineGames; }
            set { _gameSettings.Assistant.EnableInOnlineGames = value; RaisePropertyChanged(); }
        }
    }
}
