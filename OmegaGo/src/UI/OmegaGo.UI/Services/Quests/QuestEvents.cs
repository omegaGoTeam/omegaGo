using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Common;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.Services.Quests
{
    public class QuestEvents
    {
        private QuestsSettings _questsSettings;

        public QuestEvents(QuestsSettings _questsSettings)
        {
            this._questsSettings = _questsSettings;
        }

        public void GameCompleted(IGame game, GameEndInformation end)
        {
            bool isOnlineGame = game is RemoteGame;
            bool isHotseatGame = game.Controller.Players.All(pl => pl.Agent.Type == AgentType.Human);
            GamePlayer human = game.Controller.Players.FirstOrDefault(pl => pl.Agent.Type == AgentType.Human);
            bool isPlayedByUs = human != null;
            bool isVictory = (end.HasWinnerAndLoser &&
                              end.Winner == human);
            var gcqi = new GameCompletedQuestInformation(isOnlineGame, isHotseatGame, isPlayedByUs,
                isVictory, human, game, end);

            // Add points per game
            if (isPlayedByUs)
            {
                int points = 0;
                if (isVictory)
                {
                    if (isOnlineGame)
                    {
                        points = Points.ONLINE_WIN;
                    }
                    else if (!isHotseatGame)
                    {
                        points = Points.LOCAL_WIN;
                    }
                }
                else
                {
                    if (isOnlineGame)
                    {
                        points = Points.ONLINE_LOSS;
                    }
                    else if (!isHotseatGame)
                    {
                        points = Points.LOCAL_LOSS;
                    }
                }

                _questsSettings.Points += points;
            }


            foreach (var quest in this._questsSettings.ActiveQuests.ToList())
            {
                if (quest.Quest.GameCompleted(gcqi))
                {
                    this._questsSettings.ProgressQuest(quest, 1);
                }
            }
        }
        

        public void NewTsumegoSolved()
        {
            foreach(var quest in this._questsSettings.ActiveQuests.ToList())
            {
                if (quest.Quest.NewTsumegoSolved())
                {
                    this._questsSettings.ProgressQuest(quest, 1);
                }
            }
        }
    }
}
