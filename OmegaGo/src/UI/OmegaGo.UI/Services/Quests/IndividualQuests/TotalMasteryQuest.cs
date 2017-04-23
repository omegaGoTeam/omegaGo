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
    class TotalMasteryQuest : Quest
    {
        public TotalMasteryQuest() : base("Total Mastery", "Win a solo game against Fuego where Fuego is playing black and has a handicap of 3 stones or more.", RewardPoints.ExtremeReward + RewardPoints.MediumReward, 1)
        {
        }

        public override Type GetViewModelToTry()
        {
            Mvx.RegisterSingleton<GameCreation.GameCreationBundle>(new TotalMasteryBundle());
            return typeof(GameCreationViewModel);
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
        public override bool TryThisNowButtonVisible => true;
    }
}
