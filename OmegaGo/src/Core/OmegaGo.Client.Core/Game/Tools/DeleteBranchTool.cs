using System.Linq;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    /// <summary>
    /// Removes the selected node and its children. Nodes in the primary timeline cannot be deleted.
    /// </summary>
    public sealed class DeleteBranchTool : ITool
    {
        public async void Execute(IToolServices toolService)
        {
            if (!toolService.GameTree.PrimaryTimelineWithRoot.Contains(toolService.Node))
            {
                var result = await toolService.ShowMessage(ToolMessage.BranchDeletionConfirmation);

                if (result == ToolConfirmationResult.Cancel)
                    return;

                GameTreeNode parent = toolService.Node.Parent;
                parent.RemoveChild(toolService.Node);
                toolService.SetNode(parent);
            }
            else
            {
                var task = toolService.ShowMessage(ToolMessage.BranchDeletionError);
            }
        }

        public void Set(IToolServices toolServices) { }
        
    }
}
