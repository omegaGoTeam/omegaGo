using System.Linq;

namespace OmegaGo.Core.Game.Tools
{
    /// <summary>
    /// Removes the selected node and its children. Nodes in the primary timeline cannot be deleted.
    /// </summary>
    public sealed class DeleteBranchTool : ITool
    {
        public void Execute(IToolServices toolService)
        {
            if (!toolService.GameTree.LastNode.GetNodeHistory().Contains(toolService.Node))
            {
                GameTreeNode parent = toolService.Node.Parent;
                parent.RemoveChild(toolService.Node);
                toolService.SetNode(parent);
            }
        }
    }
}
