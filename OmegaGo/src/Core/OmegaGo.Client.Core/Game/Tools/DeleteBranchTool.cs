﻿using System.Linq;

namespace OmegaGo.Core.Game.Tools
{
    public class DeleteBranchTool : ITool
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
