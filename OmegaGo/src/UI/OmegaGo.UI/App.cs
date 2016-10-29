using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using OmegaGo.UI.Infrastructure;
using OmegaGo.UI.Infrastructure.Bootstrap;
using OmegaGo.UI.Services.Localization;

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
         
            Mvx.RegisterType<IAsyncAppStart, OmegaGoAppStart>();
        }
    }
}
