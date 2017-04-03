using System;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Infrastructure.Bootstrap
{
    /// <summary>
    /// Handles the application's start
    /// </summary>
    public class OmegaGoAppStart : MvxNavigatingObject, IAsyncAppStart
    {
        /// <summary>
        /// Application has been started      
        /// <param name="startArgs">Startup arguments</param>
        /// </summary>        
        public Task StartAsync( AppStartArgs startArgs = null )
        {            
            ShowViewModel<MainMenuViewModel>();
            OnAppStarted();
            return Task.FromResult(false);
        }

        /// <summary>
        /// Indicates that the app has started
        /// </summary>
        public event EventHandler AppStarted;

        /// <summary>
        /// Invokes app started event
        /// </summary>
        private void OnAppStarted()
        {
            AppStarted?.Invoke( this, EventArgs.Empty );
        }
    }
}
