using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    class DeleteBranchTool : ITool
    {
        public void Execute(IToolServices toolService)
        {
            if (!toolService.GameTree.LastNode.GetNodeHistory().Contains(toolService.Node)) 
            {
                GameTreeNode parent = toolService.Node.Parent;
                parent.RemoveChild(toolService.Node);
                toolService.Node = parent;
            }
        }
    }
}
