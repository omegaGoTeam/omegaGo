using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsUWP.Platform;
using OmegaGo.UI.Services;
using OmegaGo.UI.WindowsUniversal.Services;
using Windows.UI.Xaml.Controls;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.WindowsUniversal.Services.Files;
using OmegaGo.UI.WindowsUniversal.Services.Settings;

namespace OmegaGo.UI.WindowsUniversal
{
    public class Setup : MvxWindowsSetup
    {
        public Setup(Frame rootFrame) : base(rootFrame)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new UI.App();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override void InitializeFirstChance()
        {            
            Mvx.LazyConstructAndRegisterSingleton<IFileService, FileService>();            
            Mvx.LazyConstructAndRegisterSingleton<ISettingsService, SettingsService>();

            base.InitializeFirstChance();
        }
    }
}
