using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    public class JapaneseRuleset : Ruleset
    {
        private bool _isPreviousMovePass;
        private float _komi;
        private float _whiteScore;
        private float _blackScore;
        
        public JapaneseRuleset(GameBoardSize gbSize) : base(gbSize)
        {
            _isPreviousMovePass = false;
            _komi = 0.0f;
            _whiteScore = 0.0f;
            _blackScore = 0.0f;
        }

        public override Scores CountScore(GameBoard currentBoard)
        {
            Scores scores;
            scores = CountTerritory();

            scores.WhiteScore += _komi + _whiteScore;
            scores.BlackScore += _blackScore;

            return scores;
        }

        public static float GetJapaneseCompensation(GameBoardSize gbSize, int handicapStoneCount)
        {
            float compensation = 0.0f;
            if (handicapStoneCount == 0)
                compensation = 6.5f;
            else
                compensation = 0.5f;

            return compensation;
        }

        protected override MoveResult Pass(StoneColor playerColor)
        {
            if (_isPreviousMovePass)
            {
                return MoveResult.StartLifeAndDeath;
            }
            else
            {
                _isPreviousMovePass = true;
                return MoveResult.Legal;
            }

        }

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
            else
            {
                return MoveResult.Legal;
            }
            
        }

    }
}
