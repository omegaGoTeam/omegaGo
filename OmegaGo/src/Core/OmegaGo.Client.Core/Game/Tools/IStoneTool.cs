using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    interface IStoneTool
    {
        MoveResult[,] GetMoveResults(IToolServices toolService);
    }
}
