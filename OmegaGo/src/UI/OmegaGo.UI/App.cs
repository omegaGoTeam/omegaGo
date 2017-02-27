using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using OmegaGo.UI.Infrastructure;
using OmegaGo.UI.Infrastructure.Bootstrap;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith( "Service" )
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterServices();

            Mvx.RegisterType<IAsyncAppStart, OmegaGoAppStart>();
        }
        
        /// <summary>
        /// Registers game services
        /// </summary>
        private void RegisterServices()
        {
            RegisterSettings();
            RegisterLocalization();
            RegisterQuests();
        }

        /// <summary>
        /// Registers settings
        /// </summary>
        private void RegisterSettings()
        {
            Mvx.ConstructAndRegisterSingleton<IGameSettings, GameSettings>();
        }

        /// <summary>
        /// Registers localization
        /// </summary>
        private void RegisterLocalization()
        {
            Mvx.ConstructAndRegisterSingleton<ILocalizationService, Localizer>();            
        }

        /// <summary>
        /// Registers quest related services
        /// </summary>
        private void RegisterQuests()
        {
            Mvx.LazyConstructAndRegisterSingleton<IQuestsManager, QuestsManager>();
        }
    }
}
