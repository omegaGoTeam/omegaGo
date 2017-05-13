using OmegaGo.Core.Rules;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    /// <summary>
    /// Using this tool the player can make a pass. 
    /// In analyze mode, the pass move creates a new game tree node and doesn't use the ruleset.
    /// </summary>
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
            newNode.GroupState = new GroupState(toolService.Node.GroupState,toolService.Ruleset.RulesetInfo);
            toolService.Node.Branches.AddNode(newNode);

            toolService.SetNode(newNode);
            toolService.PlayPassSound();
        }

        public void Set(IToolServices toolServices) { }
        
    }
}
