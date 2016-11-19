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
        public ObservableCollection<string> Languages { get; } =
            new ObservableCollection<string>( GameLanguages.SupportedLanguages );

        /// <summary>
        /// Selected language
        /// </summary>
        public string SelectedLanguage
        {
            get { return _gameSettings.Language; }
            set
            {
                _gameSettings.Language = value;
                RaisePropertyChanged();
            }
        }
    }
}
