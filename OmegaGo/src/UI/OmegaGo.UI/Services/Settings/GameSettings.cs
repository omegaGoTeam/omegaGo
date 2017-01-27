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
            this.Audio = new AudioSettings(_settings);
            this.Display = new DisplaySettings(_settings);
            this.Assistant = new AssistantSettings(_settings);
            this.Quests = new QuestsGroup(_settings);
            this.Tsumego = new TsumegoSettings(_settings);
        }

        private const string LanguageSettingKey = "Language";

        public AudioSettings Audio { get; }
        public DisplaySettings Display { get; }
        public AssistantSettings Assistant { get; }
        public QuestsGroup Quests { get; }

        public TsumegoSettings Tsumego { get; }

        public bool InputConfirmationRequired
        {
            get { return _settings.GetSetting(nameof(InputConfirmationRequired), () => false); }
            set { _settings.SetSetting(nameof(InputConfirmationRequired), value); }
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
