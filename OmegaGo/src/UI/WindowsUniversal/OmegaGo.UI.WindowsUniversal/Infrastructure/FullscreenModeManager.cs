﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using OmegaGo.UI.WindowsUniversal.Services.Cheats;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure
{
    /// <summary>
    /// Manages the full screen mode of the app
    /// </summary>
    internal static class FullScreenModeManager
    {
        /// <summary>
        /// Registers the keyboard shortcuts for full screen mode
        /// </summary>
        /// <param name="window">Window</param>
        public static void RegisterForWindow(Window window)
        {
            window.CoreWindow.Dispatcher.AcceleratorKeyActivated += HandleToggleKeyboardShortcut;
        }

        /// <summary>
        /// This method uses parts of http://stackoverflow.com/a/39929182/1580088.
        /// This event is called whenever a key is pressed down on a computer keyboard. We use this method
        /// to trigger application-wide hotkeys, most notably the Alt+Enter combination for
        /// toggling fullscreen mode.
        /// </summary>
        private static void HandleToggleKeyboardShortcut(Windows.UI.Core.CoreDispatcher sender, Windows.UI.Core.AcceleratorKeyEventArgs args)
        {
            if (args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown )
            {
                // "menu key" is Alt, in Microsoft-speak.
                if (args.VirtualKey == Windows.System.VirtualKey.Enter && args.KeyStatus.IsMenuKeyDown)
                {
                    SetFullScreenMode(!ApplicationView.GetForCurrentView().IsFullScreenMode);
                    args.Handled = true;
                }
            }
        }

        /// <summary>
        /// Sets the full screen mode
        /// </summary>
        public static void SetFullScreenMode(bool useFullScreen, [CallerMemberName] string caller = null )
        {
            if (useFullScreen && !ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                Debug.WriteLine($"Full screen entered {caller}");
            }
            else
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
                Debug.WriteLine($"Full screen exited {caller}");
            }
        }

        /// <summary>
        /// Checks if the window is full screen
        /// </summary>
        public static bool IsFullScreen => ApplicationView.GetForCurrentView().IsFullScreenMode;
    }
}