using System.Collections.Generic;
using MvvmCross.Core.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.Plugins;
using MvvmCross.WindowsUWP.Platform;
using MvvmCross.WindowsUWP.Views;
using MvvmCross.WindowsUWP.Views.Suspension;
using OmegaGo.UI.WindowsUniversal.UserControls.Navigation;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    internal abstract class TabbedUISetup : MvxSetup
    {
        private readonly TabbedUIControl _tabbedUiControl;

        protected TabbedUISetup( TabbedUIControl tabbedUiControl )
        {
            _tabbedUiControl = tabbedUiControl;
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new MvxDebugTrace();
        }        

        protected override void InitializePlatformServices()
        {
            var suspensionManager = new MvxSuspensionManager();
            Mvx.RegisterSingleton(suspensionManager);
        }


        protected override IMvxViewsContainer CreateViewsContainer()
        {
            return new MvxWindowsViewsContainer();
        }

        protected override IMvxViewDispatcher CreateViewDispatcher()
        {
            var presenter = CreateViewPresenter();
            return new TabbedUIViewDispatcher(presenter, _tabbedUiControl);
        }

        protected override IMvxPluginManager CreatePluginManager()
        {
            return new MvxFilePluginManager(new List<string>() { ".WindowsUWP", ".WindowsCommon" });
        }

        protected override IMvxNameMapping CreateViewToViewModelNaming()
        {
            return new MvxPostfixAwareViewToViewModelNameMapping("View", "Page");
        }

        protected abstract IMvxWindowsViewPresenter CreateViewPresenter();
    }
}