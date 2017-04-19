using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    public interface IStoneTool : ITool
    {
        MoveResult[,] GetMoveResults(IToolServices toolService);
    }
}
