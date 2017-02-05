using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.WindowsUniversal.Services.Cheats
{
    /// <summary>
    /// The class enables some debugging cheat-like hotkeys for testing the app.
    /// </summary>
    static class Cheats
    {
        public static bool PermitCheats;
        private static IGameSettings settings = Mvx.Resolve<IGameSettings>();

        public static void HandleKeyPress(KeyEventArgs keyPressEventArgs)
        {
            switch (keyPressEventArgs.VirtualKey)
            {
                case VirtualKey.F1:
                    Cheats.settings.Quests.Points = 20;
                    break;
                case VirtualKey.F2:
                    Cheats.settings.Quests.LastQuestReceivedWhen = DateTime.Now.AddDays(-1.5f);
                    Cheats.settings.Quests.LastQuestExchangedWhen = DateTime.Now.AddDays(-1.5f);
                    break;
                case VirtualKey.F3:
                    Cheats.settings.Quests.ClearAllQuests();
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
