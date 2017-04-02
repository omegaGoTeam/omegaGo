using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.WindowsUWP.Views;
using OmegaGo.UI.WindowsUniversal.UserControls.Navigation;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    public class AppShellViewDispatcher
        : MvxWindowsMainThreadDispatcher
            , IMvxViewDispatcher
    {
        private readonly IMvxWindowsViewPresenter _presenter;


        public AppShellViewDispatcher(IMvxWindowsViewPresenter presenter, AppShell rootFrame)
            : base(rootFrame.Dispatcher)
        {
            _presenter = presenter;
        }
        
        public bool ShowViewModel(MvxViewModelRequest request)
        {
            return RequestMainThreadAction(() => _presenter.Show(request));
        }
        
        public bool ChangePresentation(MvxPresentationHint hint)
        {
            return RequestMainThreadAction(() => _presenter.ChangePresentation(hint));
        }
    }
}
