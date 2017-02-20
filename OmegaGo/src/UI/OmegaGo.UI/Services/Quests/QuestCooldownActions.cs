using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.Services.Quests
{
    static class QuestCooldownActions
    {
        public static bool IsExchangePossible(IGameSettings settings)
        {
            return settings.Quests.LastQuestExchangedWhen.Date
                   < DateTime.Today;
        }

        public static void CheckForNewQuests(IGameSettings settings)
        {
            int newQuestCount = (DateTime.Today - settings.Quests.LastQuestReceivedWhen.Date).Days;
            int slots = Quest.MAXIMUM_NUMBER_OF_QUESTS - settings.Quests.ActiveQuests.Count();
            if (newQuestCount > slots)
            {
                newQuestCount = slots;
            }
            for (int i = 0; i < newQuestCount; i++)
            {
                ActiveQuest newQuest = Quest.SpawnRandomQuest(settings.Quests.ActiveQuests.Select(q => q.QuestID));
                settings.Quests.AddQuest(newQuest);
            }
            settings.Quests.LastQuestReceivedWhen = DateTime.Now;
        }
    }
}
