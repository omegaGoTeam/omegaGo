using System;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Infrasturcture.Bootstrap
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
        public async Task StartAsync( AppStartArgs startArgs = null )
        {            
            ShowViewModel<MainMenuViewModel>();
            OnAppStarted();            
        }

        public event EventHandler AppStarted;

        private void OnAppStarted()
        {
            AppStarted?.Invoke( this, EventArgs.Empty );
        }
    }
}
