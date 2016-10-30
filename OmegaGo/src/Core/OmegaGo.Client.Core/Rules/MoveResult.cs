namespace OmegaGo.Core.Rules
{
    public enum MoveResult
    {
        Legal,
        OccupiedPosition,
        Ko,
        SuperKo,
        SelfCapture
    }
}