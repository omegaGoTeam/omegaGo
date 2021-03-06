using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsUWP.Platform;
using Windows.UI.Xaml.Controls;
using MvvmCross.WindowsUWP.Views;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.AppPackage;
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
using OmegaGo.UI.WindowsUniversal.Services.AppPackage;
using OmegaGo.UI.WindowsUniversal.Services.Audio;
using OmegaGo.UI.WindowsUniversal.Services.Dialogs;
using OmegaGo.UI.WindowsUniversal.Services.Feedback;
using OmegaGo.UI.WindowsUniversal.Services.Notifications;
using OmegaGo.UI.WindowsUniversal.Services.PasswordVault;
using OmegaGo.UI.WindowsUniversal.Services.Timer;
using OmegaGo.UI.Services.Memory;
using OmegaGo.UI.WindowsUniversal.Services.Memory;
using OmegaGo.UI.Services.DataMigration;
using OmegaGo.UI.WindowsUniversal.Services.DataMigration;
using System.Threading.Tasks;

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
            Mvx.LazyConstructAndRegisterSingleton<IAppDataFileService, AppDataFileService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppPackageService, AppPackageService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppPackageFileService, AppPackageFileService>();                    
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
            Mvx.LazyConstructAndRegisterSingleton<IMemoryService, MemoryService>();
            Mvx.LazyConstructAndRegisterSingleton<IDataMigrationProvider, DataMigrationProvider>();
            EnsureLargeSettingsFolderCreated();
            base.InitializeFirstChance();
        }
        
        /// <summary>
        /// Creates the omegaGo view presenter
        /// </summary>
        /// <param name="appShell">App shell</param>
        /// <returns>View presenter</returns>
        protected override IMvxWindowsViewPresenter CreateViewPresenter(AppShell appShell)
        {
            appShell.InitVisuals();
            return new AppShellViewPresenter(appShell);
        }

        /// <summary>
        /// Ensures that the Settings folder, used by SettingsService is created.
        /// </summary>
        private void EnsureLargeSettingsFolderCreated()
        {
            var appDataFileService = Mvx.Resolve<IAppDataFileService>();
            Task.Run(() => SettingsService.EnsureSettingsFolderCreated()).Wait();
        }
    }
}
