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
    static class Points
    {
        public const int EASY_REWARD = 40;
        public const int MEDIUM_REWARD = 80;
        public const int HARD_REWARD = 120;
        public const int EXTREME_REWARD = 200;

        public const int LOCAL_WIN = 20;
        public const int LOCAL_LOSS = 10;

        public const int ONLINE_WIN = 30;
        public const int ONLINE_LOSS = 20;

    }
}
