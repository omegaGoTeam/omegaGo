using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class PureSkillQuest : Quest
    {
        public PureSkillQuest() : base("Pure Skill", "Win a solo game against Fuego without handicap.", RewardPoints.ExtremeReward, 1)
        {
        }

        public override Type GetViewModelToTry()
        {
            Mvx.RegisterSingleton<GameCreation.GameCreationBundle>(new SoloBundle());
            return typeof(GameCreationViewModel);
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            if (info.Human == null) return false;
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
        public override bool TryThisNowButtonVisible => true;
    }
}
