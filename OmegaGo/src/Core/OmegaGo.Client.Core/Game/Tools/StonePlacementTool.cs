using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class StonePlacementTool : IStoneTool
    {
        private GameTreeNode _currentNode;
        private MoveResult[,] _moveResults;

        public StonePlacementTool(GameBoardSize boardSize)
        {
            _moveResults = new MoveResult[boardSize.Width, boardSize.Height];
        }

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

            //process move
            MoveProcessingResult moveResult = toolService.Ruleset.ProcessMove(
                toolService.Node, 
                Move.PlaceStone(nextPlayer, toolService.PointerOverPosition));
            if (moveResult.Result == MoveResult.Legal)
            {
                GameTreeNode newNode = new GameTreeNode(Move.PlaceStone(nextPlayer, toolService.PointerOverPosition));

                newNode.BoardState = moveResult.NewBoard;
                newNode.GroupState = moveResult.NewGroupState;
                newNode.Move.Captures.AddRange(moveResult.Captures);
                toolService.Node.Branches.AddNode(newNode);

                toolService.SetNode(newNode);
            }                
            
        }

        public MoveResult[,] GetMoveResults(IToolServices toolService)
        {
            if (toolService.Node.Equals(_currentNode))
            {
                _moveResults = toolService.Ruleset.GetMoveResult(toolService.Node);
                _currentNode = toolService.Node;
            }

            return _moveResults;
        }
    }
}
