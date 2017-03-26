using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// The ruleset contains the basics of Japanese Go rules. 
    /// </summary>
    public class JapaneseRuleset : Ruleset
    {
        private float _komi;
        
        /// <summary>
        /// Initializes the ruleset. For each game, a new ruleset must be created.
        /// </summary>
        /// <param name="gbSize">Size of the game board.</param>
        public JapaneseRuleset(GameBoardSize gbSize) : base(gbSize)
        {
            _komi = 0.0f;
        }

        /// <summary>
        /// Calculates the default compensation (komi).
        /// </summary>
        /// <param name="rsType">Type of the ruleset</param>
        /// <param name="gbSize">Game board size</param>
        /// <param name="handicapStoneCount">Handicap stone count</param>
        /// <param name="cType">Counting type</param>
        /// <returns></returns>
        public static float GetJapaneseCompensation(GameBoardSize gbSize, int handicapStoneCount)
        {
            float compensation = 0.0f;
            if (handicapStoneCount == 0)
                compensation = 6.5f;
            else
                compensation = 0.5f;
            RulesetInfo.Komi = compensation;
            return compensation;
        }

        /// <summary>
        /// There are two ways to score. One is based on territory, the other on area.
        /// This method uses the appropriate counting method according to the used ruleset and players' agreement.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="deadPositions">List of dead stones.</param>
        /// <returns>The score of players.</returns>
        public override Scores CountScore(GameTreeNode currentNode, IEnumerable<Position> deadPositions)
        {
            Scores scores;
            scores = CountTerritory(currentNode, deadPositions);
            scores.WhiteScore += _komi;
            
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
            if (currentNode != null && currentNode.Move.Kind == MoveKind.Pass)
                return MoveResult.StartLifeAndDeath;
            else
                return MoveResult.Legal;
           
        }

    }
}
