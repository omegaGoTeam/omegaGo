using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class EpicPlayQuest : Quest
    {
        public EpicPlayQuest() : base("Epic", "Finish any game on a 25x25 or larger board.", Points.EXTREME_REWARD, 1)
        {
        }
        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            return
                info.IsPlayedByUs &&
                info.Game.Info.BoardSize.Width >= 25 &&
                info.Game.Info.BoardSize.Height >= 25;
        }
    }
}
