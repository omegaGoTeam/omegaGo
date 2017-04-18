using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    public interface IStoneTool
    {
        MoveResult[,] GetMoveResults(IToolServices toolService);
    }
}
