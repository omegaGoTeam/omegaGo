using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Provides access to game related settings
    /// </summary>
    public class GameSettings : IGameSettings
    {
        private readonly ISettingsService _settings;

        public GameSettings(ISettingsService settings)
        {
            _settings = settings;
        }

        private const string TsumegoPossibleMovesSettingKey = "Tsumego_ShowPossibleMoves";

        public bool Tsumego_ShowPossibleMoves
        {
            get { return _settings.GetSetting(TsumegoPossibleMovesSettingKey, () => true); }
            set { _settings.SetSetting(TsumegoPossibleMovesSettingKey, value); }
        }

        private const string LanguageSettingKey = "Language";

        public string Language
        {
            get { return _settings.GetSetting(LanguageSettingKey, () => GameLanguages.DefaultLanguage.CultureTag, SettingLocality.Roamed); }
            set
            {
                if (!GameLanguages.SupportedLanguages.ContainsKey(value)) throw new ArgumentException(nameof(value));
                _settings.SetSetting(LanguageSettingKey, value, SettingLocality.Roamed);
            }
        }
    }
}
