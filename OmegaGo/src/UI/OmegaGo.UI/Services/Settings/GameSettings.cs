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
            Tsumego = new TsumegoSettingsGroup(_settings);            
            this.Audio = new AudioSettings(_settings);
            this.Display = new DisplaySettings(_settings);
            this.Assistant = new AssistantSettings(_settings);
        }

        public TsumegoSettingsGroup Tsumego { get; }
        
        private const string LanguageSettingKey = "Language";

        public AudioSettings Audio { get; }
        public DisplaySettings Display { get; }
        public AssistantSettings Assistant { get; }

        public bool InputConfirmationRequired
        {
            get { return _settings.GetSetting(nameof(InputConfirmationRequired), () => false); }
            set { _settings.SetSetting(nameof(InputConfirmationRequired), value); }
        }

        public List<string> Tsumego_SolvedTsumego
        {
            get
            {
                if (_solvedTsumegos == null)
                {
                    _solvedTsumegos = _settings.GetComplexSetting(nameof(Tsumego_SolvedTsumego),
                        () => new List<string>());
                }
                return _solvedTsumegos;
            }
        }
        public void SaveChanges()
        {
            if (_solvedTsumegos == null)
            {
                _solvedTsumegos = _settings.GetComplexSetting(nameof(Tsumego_SolvedTsumego),
                      () => new List<string>());
            }
            _settings.SetComplexSetting(nameof(Tsumego_SolvedTsumego), _solvedTsumegos);
        }

        private List<string> _solvedTsumegos;
        public bool Tsumego_ShowPossibleMoves
        {
            get { return _settings.GetSetting(nameof(Tsumego_ShowPossibleMoves), () => true); }
            set { _settings.SetSetting(nameof(Tsumego_ShowPossibleMoves), value); }
        }
        

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
