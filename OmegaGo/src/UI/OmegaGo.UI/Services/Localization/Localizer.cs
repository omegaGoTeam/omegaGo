using OmegaGo.UI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Localization
{
    /// <summary>
    /// Default localizer for the LocalizedStrings resources
    /// </summary>
    public class Localizer : LocalizationService
    {
        /// <summary>
        /// Initializes localizer
        /// </summary>
        public Localizer() : base( LocalizedStrings.ResourceManager )
        {

        }

        public string OmegaGo => LocalizeCaller();

        public string BoardSize => LocalizeCaller();

        public string Settings => LocalizeCaller();

        public string Library => LocalizeCaller();

        public string Load => LocalizeCaller();

        public string Loading => LocalizeCaller();

        public string Back => LocalizeCaller();

        public string Open => LocalizeCaller();

        public string Delete => LocalizeCaller();

        public string Select => LocalizeCaller();

        public string Rename => LocalizeCaller();

        public string UserInterface => LocalizeCaller();

        public string Sounds => LocalizeCaller();

        public string Language => LocalizeCaller();

        public string Gameplay => LocalizeCaller();

        public string Assistant => LocalizeCaller();

        public string FilterBySource => LocalizeCaller();

        public string LoadFolder => LocalizeCaller();

        public string DeleteSelection => LocalizeCaller();

        public string Tutorial => LocalizeCaller();

        public string Singleplayer => LocalizeCaller();

        public string LocalGame => LocalizeCaller();

        public string OnlineGame => LocalizeCaller();

        public string GameLibrary => LocalizeCaller();

        public string Statistics => LocalizeCaller();

        public string Help => LocalizeCaller();

        public string Credits => LocalizeCaller();

        public string Tips => LocalizeCaller();

        public string TutorialToolTip => LocalizeCaller();

        public string SingleplayerToolTip => LocalizeCaller();

        public string LocalGameToolTip => LocalizeCaller();

        public string OnlineGameToolTip => LocalizeCaller();

        public string GameLibraryToolTip => LocalizeCaller();

        public string StatisticsToolTip => LocalizeCaller();

        public string HelpToolTip => LocalizeCaller();

        public string SettingsToolTip => LocalizeCaller();

        public string ToggleFullscreenTooltip => LocalizeCaller();

        public string CreditsToolTip => LocalizeCaller();

        public string TipsToolTip => LocalizeCaller();

        public string LanguageChangeInfo => LocalizeCaller();

        public string Rules => LocalizeCaller();

        public string Difficulty => LocalizeCaller();

        public string WhiteHandicap => LocalizeCaller();

        public string Play => LocalizeCaller();

        public string Player => LocalizeCaller();

        public string Easy => LocalizeCaller();

        public string Medium => LocalizeCaller();

        public string Hard => LocalizeCaller();

        public string Black => LocalizeCaller();

        public string White => LocalizeCaller();

        public string Chinese => LocalizeCaller();

        public string Japonese => LocalizeCaller();
    }
}
