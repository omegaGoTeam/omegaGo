using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players.AI;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class TotalMasteryQuest : Quest
    {
        public TotalMasteryQuest() : base("Total Mastery", "Win a solo game against Fuego where Fuego is playing black and has a handicap of 3 stones or more.", Points.EXTREME_REWARD + Points.MEDIUM_REWARD, 1)
        {
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            var opponent =
         info.Game.Controller.Players.GetOpponentOf(info.Human);
            return
                info.IsPlayedByUs &&
                info.IsVictory &&
                opponent.Agent is AiAgent &&
                opponent.Info.Name.Contains("Fuego") &&
                info.Game.Info.NumberOfHandicapStones == 3 &&
                info.Human.Info.Color == Core.Game.StoneColor.White
                ;
        }
    }
}
