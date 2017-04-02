using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Exceptions;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsUWP.Views;
using OmegaGo.UI.Infrastructure.PresentationHints;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure
{
    /// <summary>
    /// View presenter using App Shell
    /// </summary>
    class AppShellViewPresenter
        : MvxViewPresenter, IMvxWindowsViewPresenter
    {
        private readonly AppShell _appShell;

        public AppShellViewPresenter(AppShell appShell)
        {
            _appShell = appShell;
            RegisterPresentationHintHandlers();
        }

        public override void Show(MvxViewModelRequest request)
        {
            try
            {
                var requestTranslator = Mvx.Resolve<IMvxViewsContainer>();
                var viewType = requestTranslator.GetViewType(request.ViewModelType);

                var converter = Mvx.Resolve<IMvxNavigationSerializer>();
                var requestText = converter.Serializer.SerializeObject(request);

                //this._rootFrame.Navigate(viewType, requestText); //Frame won't allow serialization of it's nav-state if it gets a non-simple type as a nav param
                //throw new NotImplementedException();
            }
            catch (Exception exception)
            {
                MvxTrace.Trace("Error seen during navigation request to {0} - error {1}", request.ViewModelType.Name,
                    exception.ToLongString());
            }
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            if (HandlePresentationChange(hint)) return;

            if (hint is MvxClosePresentationHint)
            {
                
            }

            MvxTrace.Warning("Hint ignored {0}", hint.GetType().Name);
        }
        
        /// <summary>
        /// Registers presentation hint handlers
        /// </summary>
        private void RegisterPresentationHintHandlers()
        {
            AddPresentationHintHandler<MvxClosePresentationHint>( HandleClose );
            AddPresentationHintHandler<RefreshDisplayPresentationHint>( HandleRefreshDisplay );
            AddPresentationHintHandler<PopBackStackPresentationHint>(HandlePopBackStack);
        }

        private bool HandleRefreshDisplay(RefreshDisplayPresentationHint arg)
        {
            AppShell.GetForCurrentView().RefreshVisualSettings();
            return true;
        }

        private bool HandlePopBackStack(PopBackStackPresentationHint arg)
        {
            throw new NotImplementedException();
        }

        private bool HandleClose(MvxClosePresentationHint hint)
        {
            throw new NotImplementedException();
        }
    }   
}
