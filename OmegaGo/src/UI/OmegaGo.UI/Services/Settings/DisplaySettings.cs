using OmegaGo.UI.Board.Styles;
using OmegaGo.UI.Controls.Styles;
using OmegaGo.UI.Game.Styles;

namespace OmegaGo.UI.Services.Settings
{
    public class DisplaySettings : SettingsGroup
    {
        public DisplaySettings(ISettingsService service) : base("Display", service)
        {
        }

        public ControlStyle ControlStyle
        {
            get
            {
                int theSetting = GetSetting(nameof(ControlStyle), () => (int)ControlStyle.Wood, SettingLocality.Roamed);
                return (ControlStyle)theSetting;
            }
            set { SetSetting(nameof(ControlStyle), (int)value, SettingLocality.Roamed); }
        }
        
        public BoardTheme BoardTheme
        {
            get {
                int theSetting = GetSetting(nameof(BoardTheme), () => (int)BoardTheme.SolidColor, SettingLocality.Roamed);
                return (BoardTheme) theSetting;
            }
            set { SetSetting(nameof(BoardTheme), (int)value, SettingLocality.Roamed); }
        }

        public BackgroundImage BackgroundImage
        {
            get
            {
                int theSetting = GetSetting(nameof(BackgroundImage), () => (int)BackgroundImage.Go, SettingLocality.Roamed);
                return (BackgroundImage)theSetting;
            }
            set { SetSetting(nameof(BackgroundImage), (int)value, SettingLocality.Roamed); }
        }

        public BackgroundColor BackgroundColor
        {
            get
            {
                int theSetting = GetSetting(nameof(BackgroundColor), () => (int)BackgroundColor.Basic, SettingLocality.Roamed);
                return (BackgroundColor)theSetting;
            }
            set { SetSetting(nameof(BackgroundColor), (int)value, SettingLocality.Roamed); }
        }

        public StoneTheme StonesTheme
        {
            get
            {
                int theSetting = GetSetting(nameof(StonesTheme), () => (int)StoneTheme.SolidColor, SettingLocality.Roamed);
                return (StoneTheme)theSetting;
            }
            set { SetSetting(nameof(StonesTheme), (int)value, SettingLocality.Roamed); }
        }

        public bool HighlightLastMove
        {
            get { return GetSetting(nameof(HighlightLastMove), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(HighlightLastMove), value, SettingLocality.Roamed); }
        }

        public bool HighlightRecentCaptures
        {
            get { return GetSetting(nameof(HighlightRecentCaptures), () => false, SettingLocality.Roamed); }
            set { SetSetting(nameof(HighlightRecentCaptures), value, SettingLocality.Roamed); }
        }

        public bool HighlightIllegalKoMoves
        {
            get { return GetSetting(nameof(HighlightIllegalKoMoves), () => false, SettingLocality.Roamed); }
            set { SetSetting(nameof(HighlightIllegalKoMoves), value, SettingLocality.Roamed); }
        }

        public bool ShowCoordinates
        {
            get { return GetSetting(nameof(ShowCoordinates), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(ShowCoordinates), value, SettingLocality.Roamed); }
        }
        public bool ShowTutorialInMainMenu
        {
            get { return GetSetting(nameof(ShowTutorialInMainMenu), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(ShowTutorialInMainMenu), value, SettingLocality.Roamed); }
        }
    }   
}