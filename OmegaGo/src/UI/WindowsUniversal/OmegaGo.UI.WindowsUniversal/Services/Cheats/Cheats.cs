using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using MvvmCross.Platform;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.WindowsUniversal.Infrastructure;

namespace OmegaGo.UI.WindowsUniversal.Services.Cheats
{
    /// <summary>
    /// The class enables some debugging cheat-like hotkeys for testing the app.
    /// </summary>
    static class Cheats
    {
        public static bool PermitCheats;
        private static readonly IGameSettings GameSettings = Mvx.Resolve<IGameSettings>();           

        /// <summary>
        /// Initializes cheat handling for the current app window
        /// </summary>
        public static void Initialize()
        {
#if DEBUG
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CheckForCheatKeys;
#endif
        }


        /// <summary>
        /// Checks and handles cheat keyboard combinations
        /// </summary>
        /// <param name="sender">Core dispatcher</param>
        /// <param name="args">Accelerator key event args</param>    
        private static void CheckForCheatKeys(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            // activate cheat mode -- disabling is not needed, cheats are used in short
            // debug sessions anyway
            if (args.VirtualKey == Windows.System.VirtualKey.C && args.KeyStatus.IsMenuKeyDown)
            {
                if (!Cheats.PermitCheats)
                {
                    AppShell.GetForCurrentView().TriggerBubbleNotification(new BubbleNotification("Cheats enabled."));
                }
                Cheats.PermitCheats = true;
                args.Handled = true;
            }
            //handle cheat key combinations
            if (Cheats.PermitCheats)
            {
                Cheats.HandleKeyPress(args);
            }
        }

        public static void HandleKeyPress(AcceleratorKeyEventArgs keyPressEventArgs)
        {
            if (keyPressEventArgs.EventType != CoreAcceleratorKeyEventType.KeyDown)
            {
                return;
            }
            switch (keyPressEventArgs.VirtualKey)
            {
                case VirtualKey.F1:
                    Cheats.GameSettings.Quests.Points = 20;
                    AppShell.GetForCurrentView().TriggerBubbleNotification(new BubbleNotification("CHEAT: Points set to 20."));
                    break;
                case VirtualKey.F2:
                    Cheats.GameSettings.Quests.LastQuestReceivedWhen = DateTime.Today.GetNoon().GetPreviousDay();
                    Cheats.GameSettings.Quests.LastQuestExchangedWhen = DateTime.Today.GetNoon().GetPreviousDay();

                    AppShell.GetForCurrentView().TriggerBubbleNotification(new BubbleNotification("CHEAT: Cooldowns refreshed to 1 day."));
                    break;
                case VirtualKey.F3:
                    Cheats.GameSettings.Quests.ClearAllQuests();
                    AppShell.GetForCurrentView().TriggerBubbleNotification(new BubbleNotification("CHEAT: All quests cleared."));
                    break;
                case VirtualKey.F4:
                    Cheats.GameSettings.Quests.Points += 100;
                    AppShell.GetForCurrentView().TriggerBubbleNotification(new BubbleNotification("CHEAT: +100 points"));
                    break;
                default:
                    //not handled, just return
                    return;
            }
            //handle the key press
            keyPressEventArgs.Handled = true;
        }
    }
}
