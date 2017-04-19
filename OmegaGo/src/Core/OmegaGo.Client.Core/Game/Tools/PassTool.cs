using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class PassTool : ITool
    {
        public void Execute(IToolServices toolService)
        {
            StoneColor previousPlayer = toolService.Node.Move.WhoMoves;
            StoneColor nextPlayer = StoneColor.None;

            //set next player
            if (previousPlayer == StoneColor.White)
            {
                nextPlayer = StoneColor.Black;
            }
            else if (previousPlayer == StoneColor.Black)
            {
                nextPlayer = StoneColor.White;
            }
            else
            {
                if (toolService.Node.Equals(toolService.GameTree.GameTreeRoot))
                    nextPlayer = StoneColor.Black;
                else
                    nextPlayer = StoneColor.White;
            }

            GameTreeNode newNode = new GameTreeNode(Move.Pass(nextPlayer));

            newNode.BoardState = new GameBoard(toolService.Node.BoardState);
            newNode.GroupState = new GroupState(toolService.Node.GroupState);
            newNode.Parent = toolService.Node;
            toolService.Node.Branches.AddNode(newNode);

            toolService.SetNode(newNode);
        }
    }
}
