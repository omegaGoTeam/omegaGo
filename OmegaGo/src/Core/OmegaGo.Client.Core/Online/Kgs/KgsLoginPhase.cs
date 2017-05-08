namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// Represents a step in the KGS login process.
    /// </summary>
    public enum KgsLoginPhase
    {
        StartingGetLoop,
        MakingLoginRequest,
        RequestingRoomNames,
        JoiningGlobalLists,
        Done
    }
}