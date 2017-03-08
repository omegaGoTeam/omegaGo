using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsUWP.Platform;
using OmegaGo.UI.Services;
using OmegaGo.UI.WindowsUniversal.Services;
using Windows.UI.Xaml.Controls;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.WindowsUniversal.Services.Files;
using OmegaGo.UI.WindowsUniversal.Services.Settings;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.WindowsUniversal.Services.Audio;
using OmegaGo.UI.WindowsUniversal.Services.Dialogs;
using OmegaGo.UI.WindowsUniversal.Services.Notifications;

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
            Mvx.LazyConstructAndRegisterSingleton<IFilePickerService, FilePickerService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppNotificationService, AppNotificationService>();
            Mvx.LazyConstructAndRegisterSingleton<ISettingsService, SettingsService>();
            Mvx.LazyConstructAndRegisterSingleton<IDialogService, DialogService>();
            Mvx.LazyConstructAndRegisterSingleton<ISfxPlayerService, UwpSfxPlayerService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppNotificationService, AppNotificationService>();

            base.InitializeFirstChance();
        }
    }
}
