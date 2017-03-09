using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.Services.Quests
{
    /// <summary>
    /// Manages quests
    /// </summary>
    internal class QuestsManager : IQuestsManager
    {
        private readonly IGameSettings _gameSettings;
        private readonly IAppNotificationService _appNotificationService;
        private readonly Localizer _localizer;

        /// <summary>
        /// Creates the quest points manager
        /// </summary>
        /// <param name="gameSettings">Game settings</param>
        /// <param name="appNotificationService">Notification service</param>
        /// <param name="localizationService">Localization service</param>       
        public QuestsManager(IGameSettings gameSettings, IAppNotificationService appNotificationService, ILocalizationService localizationService)
        {
            _gameSettings = gameSettings;
            _appNotificationService = appNotificationService;
            _localizer = (Localizer)localizationService;
        }

        /// <summary>
        /// Handles completion of a game
        /// </summary>
        /// <param name="game">Game that completed</param>
        /// <param name="end">Information about the game's end</param>
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

                AddPoints(points);
            }


            foreach (var quest in _gameSettings.Quests.ActiveQuests.ToList())
            {
                if (quest.Quest.GameCompleted(gcqi))
                {
                    ProgressQuest(quest, 1);
                }
            }
        }

        /// <summary>
        /// Handles solving a tsumego problem
        /// </summary>
        public void TsumegoSolved()
        {
            foreach (var quest in _gameSettings.Quests.ActiveQuests.ToList())
            {
                if (quest.Quest.NewTsumegoSolved())
                {
                    ProgressQuest(quest, 1);
                }
            }
        }

        /// <summary>
        /// Adds a quest
        /// </summary>
        /// <param name="quest">Quest</param>
        public void AddQuest(ActiveQuest quest)
        {
            _gameSettings.Quests.AddQuest(quest);
        }

        /// <summary>
        /// Loses a quest
        /// </summary>
        /// <param name="quest">Quest</param>
        public void LoseQuest(ActiveQuest quest)
        {
            _gameSettings.Quests.LoseQuest(quest);
        }

        /// <summary>
        /// Clears all quests
        /// </summary>
        public void ClearAllQuests()
        {
            _gameSettings.Quests.ClearAllQuests();
        }

        /// <summary>
        /// Makes progress on a quest
        /// </summary>
        /// <param name="quest">Quest</param>
        /// <param name="additionalProgress">Progress added</param>
        public void ProgressQuest(ActiveQuest quest, int additionalProgress)
        {
            quest.Progress += additionalProgress;
            if (quest.Progress >= quest.Quest.MaximumProgress)
            {
                _gameSettings.Quests.LoseQuest(quest);
                AddPoints(quest.Quest.PointReward);
                _gameSettings.Statistics.QuestsCompleted++;
            }
        }

        /// <summary>
        /// Adds quest points with possible notification to the user
        /// </summary>
        /// <param name="addedPoints">Points to add to the user</param>
        public void AddPoints(int addedPoints)
        {
            var newValue = _gameSettings.Quests.Points + addedPoints;
            int oldvalue = _gameSettings.Quests.Points;
            if (newValue > oldvalue)
            {
                _appNotificationService.TriggerNotification(
                    new BubbleNotification(
                        string.Format(
                            _localizer.YouHaveGainedXPointsNowYouHaveY,
                            addedPoints,
                            newValue)));
                if (Ranks.AdvancedInRank(oldvalue, newValue))
                {
                    _appNotificationService.TriggerNotification(
                        new BubbleNotification(
                            string.Format(
                                _localizer.YouHaveAdvancedToNewRankX,
                                Ranks.GetRankName(_localizer, newValue))));
                }
            }
            _gameSettings.Quests.Points = newValue;
        }
    }
}
