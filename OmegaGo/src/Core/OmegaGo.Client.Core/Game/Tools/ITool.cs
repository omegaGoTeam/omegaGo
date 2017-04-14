
namespace OmegaGo.Core.Game.Tools
{
    interface ITool
    {
        ToolKind Tool { get; }

        void Execute(IToolServices toolServices);
    }
}
