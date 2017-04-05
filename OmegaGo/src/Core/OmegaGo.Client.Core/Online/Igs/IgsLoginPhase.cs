namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// Identifies a stage in the initial handshake and login process for connecting to IGS.
    /// </summary>
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