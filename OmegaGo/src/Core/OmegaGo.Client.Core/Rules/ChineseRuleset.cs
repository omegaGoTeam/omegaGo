using System.Collections.Generic;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// The ruleset contains the basics of Chinese Go rules. 
    /// </summary>
    public sealed class ChineseRuleset : Ruleset
    {
        /// <summary>
        /// Initializes the ruleset. For each game, a new ruleset must be created.
        /// </summary>
        /// <param name="gbSize">Size of the game board.</param>
        public ChineseRuleset(GameBoardSize gbSize) : base(gbSize)
        {
        }

        /// <summary>
        /// Calculates the default compensation (komi).
        /// </summary>
        /// <param name="gbSize">Game board size</param>
        /// <param name="handicapStoneCount">Handicap stone count</param>
        /// <returns></returns>
        public static float GetChineseCompensation(GameBoardSize gbSize, int handicapStoneCount)
        {
            float compensation = 0;
            if (handicapStoneCount == 0)
                compensation = 7.5f;
            else
                compensation = 0.5f + handicapStoneCount - 1;
            return compensation;
        }

        /// <summary>
        /// There are two ways to score. One is based on territory, the other on area.
        /// This method uses the appropriate counting method according to the used ruleset and players' agreement.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="deadPositions">List of dead stones.</param>
        /// <param name="komi">Komi compensation.</param>
        /// <returns>The score of players.</returns>
        public override Scores CountScore(GameTreeNode currentNode, IEnumerable<Position> deadPositions, float komi)
        {
            var scores = CountArea(currentNode, deadPositions);
            scores.WhiteScore += komi;

            return scores;
        }

        /// <summary>
        /// Checks 3 illegal move types: self capture, ko, superko (Japanese ruleset permits superko).
        /// </summary>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>The result of legality check.</returns>
        protected override MoveResult CheckSelfCaptureKoSuperko(Move moveToMake, GameBoard[] history)
        {
            if (IsSelfCapture(moveToMake) == MoveResult.SelfCapture)
            {
                return MoveResult.SelfCapture;
            }
            else if (IsKo(moveToMake, history) == MoveResult.Ko)
            {
                return MoveResult.Ko;
            }
            else if (IsSuperKo(moveToMake, history) == MoveResult.SuperKo)
            {
                return MoveResult.SuperKo;
            }
            else
            {
                return MoveResult.Legal;
            }
            
        }

        /// <summary>
        /// Handles the pass of a player. Two consecutive passes signal the end of game.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <returns>The legality of move or new game phase notification.</returns>
        protected override MoveResult Pass(GameTreeNode currentNode)
        {
            //TODO (future work) Aniko : check whether opponents score increases according to Chinese rules
            if (currentNode != null && currentNode.Move.Kind == MoveKind.Pass)
                return MoveResult.StartLifeAndDeath;
            else
                return MoveResult.Legal;
        }
    }
}
