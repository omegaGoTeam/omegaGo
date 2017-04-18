
namespace OmegaGo.Core.Game.Tools
{
    public interface ITool
    {
        ToolKind Tool { get; }

        void Execute(IToolServices toolServices);
    }
}
