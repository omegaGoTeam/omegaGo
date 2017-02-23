using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests
{
    /// <summary>
    /// Static holder class that contains constants related to the gaining of points by
    /// the user for fulfilling quests.
    /// </summary>
    static class RewardPoints
    {
        public const int EasyReward = 40;
        public const int MediumReward = 80;
        public const int HardReward = 120;
        public const int ExtremeReward = 200;

        public const int LocalWin = 20;
        public const int LocalLoss = 10;

        public const int OnlineWin = 30;
        public const int OnlineLoss = 20;

    }
}
