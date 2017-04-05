namespace OmegaGo.Core.Online.Igs
{
    public enum IgsLoginPhase
    {
        Connecting,
        LoggingIn,
        SendingInitialBurst,
        RefreshingGames,
        RefreshingUsers,
        Done
    }
}