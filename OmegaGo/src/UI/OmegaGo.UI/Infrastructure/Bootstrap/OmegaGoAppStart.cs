using System;
using System.IO;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.Services.Settings;
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
        public async Task StartAsync(AppStartArgs startArgs = null)
        {
            await BeforeLaunchAsync();
            ShowViewModel<MainMenuViewModel>();
            OnAppStarted();
        }

        private async Task BeforeLaunchAsync()
        {
            var settingsService = Mvx.Resolve<IGameSettings>();
            if (settingsService.LaunchCount == 0)
            {
                //copy sample SGF file to library
                var appPackageService = Mvx.Resolve<IAppPackageFileService>();
                var appDataService = Mvx.Resolve<IAppDataFileService>();
                var fileName = "AlphaGo1.sgf";
                var content = await appPackageService.ReadFileFromRelativePathAsync(Path.Combine("SGF", fileName));
                await appDataService.WriteFileAsync(fileName, content, LibraryViewModel.SgfFolderName);
            }
            settingsService.LaunchCount++;
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
            AppStarted?.Invoke(this, EventArgs.Empty);
        }
    }
}
