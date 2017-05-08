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
            StoneColor nextPlayer = toolService.Node.Move.WhoMoves.GetOpponentColor(toolService.Node, toolService.GameTree.GameTreeRoot);
            
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
                toolService.PlayStonePlacementSound(newNode.Move.Captures.Count > 0);
            }                
            
        }

        public IShadowItem GetShadowItem(IToolServices toolService)
        {
            if (toolService.Node.Equals(toolService.GameTree.GameTreeRoot))
            {
                int width = toolService.GameTree.BoardSize.Width;
                int height = toolService.GameTree.BoardSize.Height;
                MoveResult[,] moveResults = new MoveResult[width, height];
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        moveResults[x, y] = MoveResult.Legal;
                _currentNode = toolService.Node;
            }
            else if (!toolService.Node.Equals(_currentNode) || _currentNode == null)
            {
                _moveResults = toolService.Ruleset.GetMoveResult(toolService.Node);
                _currentNode = toolService.Node;
            }

            MoveResult result=_moveResults[toolService.PointerOverPosition.X,toolService.PointerOverPosition.Y];
            StoneColor nextPlayer = toolService.Node.Move.WhoMoves.GetOpponentColor(toolService.Node, toolService.GameTree.GameTreeRoot);

            if (result == MoveResult.Legal) 
                return new Stone(nextPlayer, toolService.PointerOverPosition);
            else
                return new None();
        }

        public void Set(IToolServices toolServices) { }
    }
}
