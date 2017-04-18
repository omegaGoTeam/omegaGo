using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class StonePlacementTool : ITool, IStoneTool
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

            //process pass or move
            if (toolService.PointerOverPosition == null) //it's a pass
            {
                GameTreeNode newNode = new GameTreeNode(Move.Pass(nextPlayer));
                newNode.BoardState = new GameBoard(toolService.Node.BoardState);
                newNode.GroupState = new GroupState(toolService.Node.GroupState);
                newNode.Parent = toolService.Node;
                toolService.Node.Branches.AddNode(newNode);
            }
            else
            {
                MoveProcessingResult moveResult = toolService.Ruleset.ProcessMove(
                    toolService.Node, 
                    Move.PlaceStone(nextPlayer, toolService.PointerOverPosition));
                if (moveResult.Result == MoveResult.Legal)
                {
                    GameTreeNode newNode = new GameTreeNode(Move.PlaceStone(nextPlayer, toolService.PointerOverPosition));
                    newNode.BoardState = moveResult.NewBoard;
                    newNode.GroupState = moveResult.NewGroupState;
                    newNode.Move.Captures.AddRange(moveResult.Captures);
                    newNode.Parent = toolService.Node;
                    toolService.Node.Branches.AddNode(newNode);
                }                
            }
        }

        public MoveResult[,] GetMoveResults(IToolServices toolService)
        {
            return toolService.Ruleset.GetMoveResultLite(toolService.Node);
        }
    }
}
