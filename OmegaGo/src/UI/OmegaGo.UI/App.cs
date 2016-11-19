using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using OmegaGo.UI.Infrastructure;
using OmegaGo.UI.Infrastructure.Bootstrap;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
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
            RegisterLocalization();
        }

        /// <summary>
        /// Registers localization
        /// </summary>
        private void RegisterLocalization()
        {
            Mvx.ConstructAndRegisterSingleton<IGameSettings, GameSettings>();
        }
    }
}
