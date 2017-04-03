using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsUWP.Platform;
using Windows.UI.Xaml.Controls;
using MvvmCross.WindowsUWP.Views;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Feedback;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.WindowsUniversal.Services.Files;
using OmegaGo.UI.WindowsUniversal.Services.Settings;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Timer;
using OmegaGo.UI.Services.PasswordVault;
using OmegaGo.UI.WindowsUniversal.Infrastructure;
using OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed;
using OmegaGo.UI.WindowsUniversal.Services.Audio;
using OmegaGo.UI.WindowsUniversal.Services.Dialogs;
using OmegaGo.UI.WindowsUniversal.Services.Feedback;
using OmegaGo.UI.WindowsUniversal.Services.Notifications;
using OmegaGo.UI.WindowsUniversal.Services.PasswordVault;
using OmegaGo.UI.WindowsUniversal.Services.Timer;

namespace OmegaGo.UI.WindowsUniversal
{
    public class Setup : AppShellSetup
    {
        public Setup(AppShell appShell) : base(appShell)
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
            Mvx.LazyConstructAndRegisterSingleton<ITabProvider, TabProvider>();
            Mvx.LazyConstructAndRegisterSingleton<IFilePickerService, FilePickerService>();
            Mvx.LazyConstructAndRegisterSingleton<ITimerService, TimerService>();
            Mvx.LazyConstructAndRegisterSingleton<IFeedbackService, FeedbackService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppNotificationService, AppNotificationService>();
            Mvx.LazyConstructAndRegisterSingleton<IPasswordVaultService, PasswordVaultService>();
            Mvx.LazyConstructAndRegisterSingleton<ISettingsService, SettingsService>();
            Mvx.LazyConstructAndRegisterSingleton<IDialogService, DialogService>();
            Mvx.LazyConstructAndRegisterSingleton<ISfxPlayerService, UwpSfxPlayerService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppNotificationService, AppNotificationService>();           
            base.InitializeFirstChance();
        }

        /// <summary>
        /// Creates the omegaGo view presenter
        /// </summary>
        /// <param name="appShell">App shell</param>
        /// <returns>View presenter</returns>
        protected override IMvxWindowsViewPresenter CreateViewPresenter(AppShell appShell )
        {
            return new AppShellViewPresenter(appShell);
        }
    }
}
