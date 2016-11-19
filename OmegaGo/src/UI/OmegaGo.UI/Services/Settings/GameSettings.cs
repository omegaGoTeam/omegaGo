using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.Services.Settings
{
    public class GameSettings : IGameSettings
    {
        private readonly ISettingsService _settings;

        public GameSettings( ISettingsService settings )
        {
            _settings = settings;
        }

        private const string LanguageSettingKey = "Language";

        public string Language
        {
            get { return _settings.GetSetting( LanguageSettingKey, () => GameLanguages.DefaultLanguage, SettingLocality.Roamed ); }
            set
            {
                if ( !GameLanguages.SupportedLanguages.Contains( value, StringComparer.OrdinalIgnoreCase ) )
                    throw new ArgumentException( nameof( value ) );
                _settings.SetSetting( LanguageSettingKey, value, SettingLocality.Roamed );
            }
        }
    }
}
