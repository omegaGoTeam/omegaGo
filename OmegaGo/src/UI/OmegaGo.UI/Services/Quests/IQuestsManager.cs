using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.State;

namespace OmegaGo.UI.Services.Quests
{
    /// <summary>
    /// Manages the quests
    /// </summary>
    public interface IQuestsManager
    {
        /// <summary>
        /// Handles completion of a game
        /// </summary>
        /// <param name="game">Game that completed</param>
        /// <param name="end">Information about the game's end</param>
        void GameCompleted(IGame game, GameEndInformation end);

        /// <summary>
        /// Handles solving a tsumego problem
        /// </summary>
        void TsumegoSolved();

        /// <summary>
        /// Adds a quest
        /// </summary>
        /// <param name="quest">Quest</param>
        void AddQuest(ActiveQuest quest);

        /// <summary>
        /// Loses a quest
        /// </summary>
        /// <param name="quest">Quest</param>
        void LoseQuest(ActiveQuest quest);

        /// <summary>
        /// Clears all quests
        /// </summary>
        void ClearAllQuests();

        /// <summary>
        /// Makes progress on a quest
        /// </summary>
        /// <param name="quest">Quest</param>
        /// <param name="additionalProgress">Progress added</param>
        void ProgressQuest(ActiveQuest quest, int additionalProgress);

        // TODO Petr: Consider removing public modifier if cheats won't be in the final version of the game
        /// <summary>
        /// Adds quest points with possible notification to the user
        /// </summary>
        /// <param name="addedPoints">Points to add to the user</param>
        void AddPoints(int addedPoints);
    }
}
