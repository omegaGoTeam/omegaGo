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
            // "menu key" is Alt, in Microsoft-speak.
            if (args.VirtualKey == Windows.System.VirtualKey.Enter && args.KeyStatus.IsMenuKeyDown)
            {
                SetFullScreenMode(!ApplicationView.GetForCurrentView().IsFullScreenMode);
            }
        }

        /// <summary>
        /// Sets the full screen mode
        /// </summary>
        public static void SetFullScreenMode(bool useFullScreen)
        {
            if (useFullScreen && !ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
            }
        }

        /// <summary>
        /// Checks if the window is full screen
        /// </summary>
        public static bool IsFullScreen => ApplicationView.GetForCurrentView().IsFullScreenMode;
    }
}