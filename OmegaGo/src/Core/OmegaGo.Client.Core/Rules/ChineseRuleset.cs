using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// The ruleset contains the basics of Chinese Go rules. 
    /// </summary>
    public class ChineseRuleset : Ruleset
    {
        private bool _isPreviousMovePass;
        private float _komi;
        private float _whiteScore;
        private float _blackScore;

        /// <summary>
        /// Initializes the ruleset. For each game, a new ruleset must be created.
        /// </summary>
        /// <param name="gbSize">Size of the game board.</param>
        public ChineseRuleset(GameBoardSize gbSize) : base(gbSize)
        {
            _isPreviousMovePass = false;
            _komi = 0.0f;
            _whiteScore = 0.0f;
            _blackScore = 0.0f;
        }

        /// <summary>
        /// Calculates the default compensation (komi).
        /// </summary>
        /// <param name="rsType">Type of the ruleset</param>
        /// <param name="gbSize">Game board size</param>
        /// <param name="handicapStoneCount">Handicap stone count</param>
        /// <param name="cType">Counting type</param>
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
        /// <param name="currentBoard">The state of board after removing dead stones.</param>
        /// <returns>The score of players.</returns>
        public override Scores CountScore(GameBoard currentBoard)
        {
            var scores = CountArea();

            scores.WhiteScore += _komi + _whiteScore;
            scores.BlackScore += _blackScore;

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
            _isPreviousMovePass = false;

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
        /// <param name="playerColor">Color of player, who passes.</param>
        /// <returns>The legality of move or new game phase notification.</returns>
        protected override MoveResult Pass(StoneColor playerColor)
        {
            if (_isPreviousMovePass)
            {
                //TODO Aniko : check whether opponents score increases according to Chinese rules
                return MoveResult.StartLifeAndDeath;
            }
            else
            {
                //TODO Aniko : check whether opponents score increases according to Chinese rules
                _isPreviousMovePass = true;
                return MoveResult.Legal;
            }

        }


    }
}
