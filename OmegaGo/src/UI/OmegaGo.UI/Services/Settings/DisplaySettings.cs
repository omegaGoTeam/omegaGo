using MvvmCross.Platform.UI;
using OmegaGo.UI.Board.Styles;
using OmegaGo.UI.Controls.Styles;
using OmegaGo.UI.Controls.Themes;
using OmegaGo.UI.Game.Styles;

namespace OmegaGo.UI.Services.Settings
{
    public class DisplaySettings : SettingsGroup
    {
        private const string BackgroundColorOpacityKey = "BackgroundColorOpacity";

        public DisplaySettings(ISettingsService service) : base("Display", service)
        {
        }

        public ControlStyle ControlStyle
        {
            get
            {
                int theSetting = GetSetting(nameof(ControlStyle), () => (int)ControlStyle.OperatingSystem, SettingLocality.Roamed);
                return (ControlStyle)theSetting;
            }
            set { SetSetting(nameof(ControlStyle), (int)value, SettingLocality.Roamed); }
        }

        public BoardTheme BoardTheme
        {
            get
            {
                int theSetting = GetSetting(nameof(BoardTheme), () => (int)BoardTheme.KayaWood, SettingLocality.Roamed);
                BoardTheme theme= (BoardTheme)theSetting;
                if (theSetting < 0 || theSetting > 3) theme = BoardTheme.KayaWood;
                return theme;
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

        public float BackgroundColorOpacity
        {
            get { return GetSetting(BackgroundColorOpacityKey, () => 0.1f, SettingLocality.Roamed); }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    SetSetting(BackgroundColorOpacityKey, value, SettingLocality.Roamed);
                }
            }
        }

        public BackgroundColor BackgroundColor
        {
            get
            {
                return GetComplexSetting(nameof(BackgroundColor), () => BackgroundColor.Default, SettingLocality.Roamed);
            }
            set { SetComplexSetting(nameof(BackgroundColor), value, SettingLocality.Roamed); }
        }

        public StoneTheme StonesTheme
        {
            get
            {
                int theSetting = GetSetting(nameof(StonesTheme), () => (int)StoneTheme.PolishedBitmap, SettingLocality.Roamed);
                return (StoneTheme)theSetting;
            }
            set { SetSetting(nameof(StonesTheme), (int)value, SettingLocality.Roamed); }
        }

        public AppTheme AppTheme
        {
            get
            {
                int setting = GetSetting(nameof(AppTheme), () => (int)AppTheme.Light, SettingLocality.Roamed);
                return (AppTheme)setting;
            }
            set
            {
                SetSetting(nameof(AppTheme), (int)value, SettingLocality.Roamed);
            }
        }

        public bool AddGraceSecond
        {
            get { return GetSetting(nameof(AddGraceSecond), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(AddGraceSecond), value, SettingLocality.Roamed); }
        }

        public bool AddTouchInputOffset
        {
            get { return GetSetting(nameof(AddTouchInputOffset), () => false, SettingLocality.Roamed); }
            set { SetSetting(nameof(AddTouchInputOffset), value, SettingLocality.Roamed); }
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