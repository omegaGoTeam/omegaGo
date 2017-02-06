namespace OmegaGo.UI.Services.Settings
{
    public class DisplaySettings : SettingsGroup
    {
        public DisplaySettings(ISettingsService service) : base("Display", service)
        {
        }

        public BoardTheme BoardTheme
        {
            get {
                int theSetting = GetSetting(nameof(BoardTheme), () => (int)BoardTheme.SolidColor);
                return (BoardTheme) theSetting;
            }
            set { SetSetting(nameof(BoardTheme), (int)value); }
        }
        public StoneTheme StonesTheme
        {
            get
            {
                int theSetting = GetSetting(nameof(StonesTheme), () => (int)StoneTheme.SolidColor);
                return (StoneTheme)theSetting;
            }
            set { SetSetting(nameof(StonesTheme), (int)value); }
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
    public enum BoardTheme
    {
        SolidColor,
        OakWood,
        KayaWood,
        VirtualBoard,
        SabakiBoard
    }
    public enum StoneTheme
    {
        SolidColor,
        PolishedBitmap,
        Sabaki
    }
}