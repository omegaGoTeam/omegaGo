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
        public Localizer() : base(LocalizedStrings.ResourceManager)
        {

        }

        public string ThemesPanel => LocalizeCaller();

        public string BoardTheme => LocalizeCaller();

        public string StonesTheme => LocalizeCaller();

        public string BackgroundImage => LocalizeCaller();

        public string BackgroundColor => LocalizeCaller();

        public string FullscreenModeCheckbox => LocalizeCaller();

        public string HighlightLastMove => LocalizeCaller();

        public string HighlightRecentCaptures => LocalizeCaller();

        public string HighlightRecentCapturesTooltip => LocalizeCaller();

        public string HighlightIllegalKoMoves => LocalizeCaller();

        public string HighlightIllegalKoMovesTooltip => LocalizeCaller();

        public string ShowCoordinates => LocalizeCaller();

        public string MakingAMoveRequiresAConfirmationClick => LocalizeCaller();

        public string EnableHints => LocalizeCaller();

        public string EnableEvenInOnlineGames => LocalizeCaller();

        public string EnableEvenInOnlineGamesTooltip => LocalizeCaller();

        public string Strength => LocalizeCaller();

        public string AssistantAIProgram => LocalizeCaller();

        public string ASoundEffectShouldPlay => LocalizeCaller();

        public string WhenIPlaceAStone => LocalizeCaller();

        public string WhenAnotherPlayerPlacesAStone => LocalizeCaller();

        public string WhenIReceiveANotification => LocalizeCaller();

        public string SfxVolume => LocalizeCaller();

        public string MusicVolume => LocalizeCaller();

        public string MuteAll => LocalizeCaller();

        public string MasterVolume => LocalizeCaller();

        public string ShowTutorialButton => LocalizeCaller();

        public string InputPanel => LocalizeCaller();

        public string AudioPanel => LocalizeCaller();

        public string AssistPanel => LocalizeCaller();

        public string OmegaGo => LocalizeCaller();

        public string Game => LocalizeCaller();

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
        public string TotalGamesPlayed => LocalizeCaller();

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

        public string HotseatGamesPlayed => LocalizeCaller();
        public string SoloGamesPlayed => LocalizeCaller();
        public string OnlineGamesPlayed => LocalizeCaller();
        public string TotalGamesWon => LocalizeCaller();
        public string SoloGamesWon => LocalizeCaller();
        public string OnlineGamesWon => LocalizeCaller();
        public string QuestsCompleted => LocalizeCaller();
        public string TsumegoProblemsSolved => LocalizeCaller();
        public string PointsColon => LocalizeCaller();
        public string OmegaGoRankColon => LocalizeCaller();

        // Board settings control strings
        public string BoardSettings => LocalizeCaller();

        public string CellSize => LocalizeCaller();
        
        public string BoardBorderThickness => LocalizeCaller();
    }
}
