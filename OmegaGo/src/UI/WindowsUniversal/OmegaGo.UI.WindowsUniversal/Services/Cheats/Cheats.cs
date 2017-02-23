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
        private static IGameSettings settings = Mvx.Resolve<IGameSettings>();

        public static void HandleKeyPress(AcceleratorKeyEventArgs keyPressEventArgs)
        {
            switch (keyPressEventArgs.VirtualKey)
            {
                case VirtualKey.F1:
                    Cheats.settings.Quests.Points = 20;
                    AppShell.GetForCurrentView().DEBUG_TriggerNotification(new BubbleNotification("CHEAT: Points set to 20."));
                    break;
                case VirtualKey.F2:
                    Cheats.settings.Quests.LastQuestReceivedWhen = DateTime.Today.GetNoon().GetPreviousDay();
                    Cheats.settings.Quests.LastQuestExchangedWhen = DateTime.Today.GetNoon().GetPreviousDay();

                    AppShell.GetForCurrentView().DEBUG_TriggerNotification(new BubbleNotification("CHEAT: Cooldowns refreshed to 1 day."));
                    break;
                case VirtualKey.F3:
                    Cheats.settings.Quests.ClearAllQuests();
                    AppShell.GetForCurrentView().DEBUG_TriggerNotification(new BubbleNotification("CHEAT: All quests cleared."));
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
