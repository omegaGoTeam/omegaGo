using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    class StonePlacementTool : ITool
    {
        public ToolKind Tool { get; } = ToolKind.StonePlacement;
        public void Execute(IToolServices toolService)
        {
            StoneColor previousPlayer = toolService.Node.Move.WhoMoves;
            StoneColor nextPlayer=StoneColor.None;
            if (toolService.Ruleset == null)
                toolService.Ruleset = Ruleset.Create(RulesetType.Chinese, toolService.GameTree.BoardSize);


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
    }
}
