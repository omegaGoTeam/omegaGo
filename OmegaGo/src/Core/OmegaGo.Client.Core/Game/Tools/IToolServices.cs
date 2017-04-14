using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    interface IToolServices
    {
        IRuleset Ruleset { get; set; }
        GameTree GameTree { get; set; }
        GameTreeNode Node { get; set; }
        Position PointerOverPosition { get; set; }
    }
}
