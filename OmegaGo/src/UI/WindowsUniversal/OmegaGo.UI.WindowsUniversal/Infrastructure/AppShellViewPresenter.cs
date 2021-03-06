﻿using System;
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
using OmegaGo.UI.Infrastructure.Tabbed;

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
                TabNavigationType tabNavigationType = TabNavigationType.ActiveTab;
                if (request.PresentationValues != null)
                {
                    if (request.PresentationValues.ContainsKey(nameof(TabNavigationType)))
                    {
                        Enum.TryParse(request.PresentationValues[nameof(TabNavigationType)], true,
                            out tabNavigationType);
                    }
                }
                _appShell.TabManager.ProcessViewModelRequest(request, tabNavigationType);
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

            MvxTrace.Warning("Hint ignored {0}", hint.GetType().Name);
        }

        /// <summary>
        /// Registers presentation hint handlers
        /// </summary>
        private void RegisterPresentationHintHandlers()
        {
            AddPresentationHintHandler<MvxClosePresentationHint>(HandleClose);
            AddPresentationHintHandler<RefreshDisplayPresentationHint>(HandleRefreshDisplay);
            AddPresentationHintHandler<GoBackPresentationHint>(GoBack);
        }

        private bool GoBack(GoBackPresentationHint hint)
        {
            var tab = _appShell.TabManager.Tabs.FirstOrDefault(t => t.Id == hint.Tab.Id);
            if (tab?.Frame?.CanGoBack == true)
            {
                tab?.Frame?.GoBack();
            }
            return true;
        }

        private bool HandleRefreshDisplay(RefreshDisplayPresentationHint arg)
        {
            AppShell.GetForCurrentView().RefreshVisualSettings();
            return true;
        }

        private bool HandleClose(MvxClosePresentationHint hint)
        {
            if (_appShell.TabManager.ActiveTab?.Frame.CanGoBack == true)
            {
                _appShell.TabManager.ActiveTab?.Frame.GoBack();
            }
            return true;
        }
    }
}
