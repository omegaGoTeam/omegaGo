
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    /// <summary>
    /// Base interface for tools.
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// Performs an action based on the selected tool. This action is specified in developer documentation.
        /// </summary>
        /// <param name="toolServices">Useful information.</param>
        void Execute(IToolServices toolServices);

        void Set(IToolServices toolServices);
        
    }
}
