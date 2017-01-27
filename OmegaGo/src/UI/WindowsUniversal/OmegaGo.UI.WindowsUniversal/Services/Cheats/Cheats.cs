﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.WindowsUniversal.Services.Cheats
{
    class Cheats
    {
        public static bool PermitCheats;
        private static IGameSettings _settings = Mvx.Resolve<IGameSettings>();

        public static void HandleKey(VirtualKey virtualKey)
        {
            switch (virtualKey)
            {
                case VirtualKey.F1:
                    _settings.Quests.Points = 20;
                    break;
                case VirtualKey.F2:
                    _settings.Quests.LastQuestReceivedWhen = DateTime.Now.AddDays(-1.5f);
                    _settings.Quests.LastQuestExchangedWhen = DateTime.Now.AddDays(-1.5f);
                    break;
                case VirtualKey.F3:
                    _settings.Quests.ClearAllQuests();
                    break;
            }
        }
    }
}
