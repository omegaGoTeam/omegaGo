using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    interface IToolServices
    {
        //TODO Aniko: Game or just Ruleset?
        IRuleset Ruleset { get; set; }
        GameTree GameTree { get; set; }
        GameTreeNode Node { get; set; }
        Position PointerOverPosition { get; set; }
        
    }
}
