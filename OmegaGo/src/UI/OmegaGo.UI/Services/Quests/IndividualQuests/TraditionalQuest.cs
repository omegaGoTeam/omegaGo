using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class TraditionalQuest : Quest
    {
        public TraditionalQuest() : base("Traditional", "Finish any game on a 19x19 board.", Points.EASY_REWARD, 1)
        {
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            return
                info.IsPlayedByUs &&
                info.Game.Info.BoardSize.Width == 19 &&
                info.Game.Info.BoardSize.Height == 19
                ;
        }
    }
}
