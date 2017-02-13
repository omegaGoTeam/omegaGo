﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class IgsChallengeQuest : Quest
    {
        public IgsChallengeQuest() : base("Pandanet Challenge", "Finish 3 games on the Pandanet server.", Points.HARD_REWARD, 3)
        {
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            return info.IsPlayedByUs &&
                   info.IsOnline &&
                   info.Game.Controller.Server.Name == ServerId.Igs;
        }
    }
}