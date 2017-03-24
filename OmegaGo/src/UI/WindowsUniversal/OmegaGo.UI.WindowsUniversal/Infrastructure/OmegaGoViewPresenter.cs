using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using MvvmCross.Core.ViewModels;
using MvvmCross.WindowsUWP.Views;
using OmegaGo.UI.Infrastructure.PresentationHints;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure
{
    /// <summary>
    /// Main view presenter of the app
    /// </summary>
    class OmegaGoViewPresenter : MvxWindowsViewPresenter
    {
        private readonly Frame _rootFrame;

        public OmegaGoViewPresenter(IMvxWindowsFrame rootFrame) : base(rootFrame)
        {
            _rootFrame = ( Frame )rootFrame.UnderlyingControl;
        }

        /// <summary>
        /// Changes the presentation of the view presenter
        /// </summary>
        /// <param name="hint"></param>
        public override void ChangePresentation(MvxPresentationHint hint)
        {
            if (hint is RefreshDisplayPresentationHint)
            {
                HandleRefreshDisplayHint();
            }
            else if (hint is PopBackStackPresentationHint)
            {
                HandlePopBackStackHint();
            }
            base.ChangePresentation(hint);
        }

        /// <summary>
        /// Refreshes the display using AppShell
        /// </summary>
        private void HandleRefreshDisplayHint()
        {
            AppShell.GetForCurrentView().RefreshVisualSettings();
        }

        /// <summary>
        /// Pops the back stack
        /// </summary>
        private void HandlePopBackStackHint()
        {
            if (_rootFrame.BackStackDepth > 0)
            {
                _rootFrame.BackStack.RemoveAt(_rootFrame.BackStack.Count - 1);
            }
        }
    }
}
