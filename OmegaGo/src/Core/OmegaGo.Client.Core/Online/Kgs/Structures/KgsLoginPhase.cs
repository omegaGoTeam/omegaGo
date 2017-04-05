namespace OmegaGo.Core.Online.Kgs
{
    public enum KgsLoginPhase
    {
        StartingGetLoop,
        MakingLoginRequest,
        RequestingRoomNames,
        JoiningGlobalLists,
        Done
    }
}