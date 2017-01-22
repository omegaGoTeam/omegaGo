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
                int theSetting = GetSetting(nameof(BoardTheme), () => (int)BoardTheme.Simple);
                return (BoardTheme) theSetting;
            }
            set { SetSetting(nameof(BoardTheme), (int)value); }
        }
        public bool HighlightLastMove
        {
            get { return GetSetting(nameof(HighlightLastMove), () => true); }
            set { SetSetting(nameof(HighlightLastMove), value); }
        }
        public bool HighlightRecentCaptures
        {
            get { return GetSetting(nameof(HighlightRecentCaptures), () => false); }
            set { SetSetting(nameof(HighlightRecentCaptures), value); }
        }
        public bool HighlightIllegalKoMoves
        {
            get { return GetSetting(nameof(HighlightIllegalKoMoves), () => false); }
            set { SetSetting(nameof(HighlightIllegalKoMoves), value); }
        }
        public bool ShowCoordinates
        {
            get { return GetSetting(nameof(ShowCoordinates), () => true); }
            set { SetSetting(nameof(ShowCoordinates), value); }
        }
    }
    public enum BoardTheme
    {
        Simple,
        Oak,
        Kaya
    }
}