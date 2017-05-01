using OmegaGo.Core.Game.Markup;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    /// <summary>
    /// Places a stone of a color that's different from the current nodes's move color. 
    /// </summary>
    public sealed class StonePlacementTool : IPlacementTool
    {
        /// <summary>
        /// Contains the last processed node.
        /// </summary>
        private GameTreeNode _currentNode;
        
        /// <summary>
        /// Contains the results of possible moves based on the board state in _currentNode.
        /// </summary>
        private MoveResult[,] _moveResults;

        public StonePlacementTool(GameBoardSize boardSize)
        {
            _moveResults = new MoveResult[boardSize.Width, boardSize.Height];
        }

        public void Execute(IToolServices toolService)
        {
            // TODO if board this empty and we first make move with this tool, exception is thrown
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

        public IShadowItem GetShadowItem(IToolServices toolService)
        {
            if (!toolService.Node.Equals(_currentNode) || _currentNode==null)
            {
                _moveResults = toolService.Ruleset.GetMoveResult(toolService.Node);
                _currentNode = toolService.Node;
            }

            MoveResult result=_moveResults[toolService.PointerOverPosition.X,toolService.PointerOverPosition.Y];
            if (result == MoveResult.Legal) {
                if (_currentNode.Move.WhoMoves == StoneColor.Black)
                    return new Stone(StoneColor.White, toolService.PointerOverPosition);
                else if (_currentNode.Move.WhoMoves == StoneColor.White)
                    return new Stone(StoneColor.Black, toolService.PointerOverPosition); ;
            }

            return new None();
        }
    }
}
