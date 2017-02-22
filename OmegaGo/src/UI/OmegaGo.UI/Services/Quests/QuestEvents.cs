using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Remote;
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
            bool isOnlineGame = game is IRemoteGame;
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
                        points = RewardPoints.OnlineWin;
                    }
                    else if (!isHotseatGame)
                    {
                        points = RewardPoints.LocalWin;
                    }
                }
                else
                {
                    if (isOnlineGame)
                    {
                        points = RewardPoints.OnlineLoss;
                    }
                    else if (!isHotseatGame)
                    {
                        points = RewardPoints.LocalLoss;
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
