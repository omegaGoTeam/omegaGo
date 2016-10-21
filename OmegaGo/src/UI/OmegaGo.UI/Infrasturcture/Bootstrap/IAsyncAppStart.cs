using System;
using System.Threading.Tasks;

namespace OmegaGo.UI.Infrasturcture.Bootstrap
{
    /// <summary>
    /// Handles the start of the app asynchronously
    /// </summary>
    public interface IAsyncAppStart
    {
        /// <summary>
        /// Starts up and initializes the app
        /// Needs to fire the AppStarted event when done
        /// </summary>
        /// <param name="startArgs">Application start arguments</param>
        Task StartAsync( AppStartArgs startArgs = null );

        /// <summary>
        /// Application has finished initializing
        /// </summary>
        event EventHandler AppStarted;
    }
}
