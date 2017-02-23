using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class PureSkillQuest : Quest
    {
        public PureSkillQuest() : base("Pure Skill", "Win a solo game against Fuego without handicap.", RewardPoints.ExtremeReward, 1)
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
                info.Game.Info.NumberOfHandicapStones == 0
                ;
        }
    }
}
