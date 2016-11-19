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
    }
}
