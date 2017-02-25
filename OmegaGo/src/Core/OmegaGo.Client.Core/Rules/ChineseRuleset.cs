using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    public class ChineseRuleset : Ruleset
    {
        private bool _isPreviousMovePass;
        private float _komi;
        private float _whiteScore;
        private float _blackScore;
        
        public ChineseRuleset(GameBoardSize gbSize) : base(gbSize)
        {
            _isPreviousMovePass = false;
            _komi = 0.0f;
            _whiteScore = 0.0f;
            _blackScore = 0.0f;
        }

        public override Scores CountScore(GameBoard currentBoard)
        {
            var scores = CountArea(currentBoard);

            scores.WhiteScore += _komi + _whiteScore;
            scores.BlackScore += _blackScore;

            return scores;
        }

        public static float GetChineseCompensation(GameBoardSize gbSize, int handicapStoneCount)
        {
            float compensation = 0;
            if (handicapStoneCount == 0)
                compensation = 7.5f;
            else
                compensation = 0.5f + handicapStoneCount - 1;
            
            return compensation;
        }

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

        protected override MoveResult CheckSelfCaptureKoSuperko(GameBoard currentBoard, Move moveToMake, GameBoard[] history)
        {
            _isPreviousMovePass = false;

            if (IsSelfCapture(currentBoard, moveToMake) == MoveResult.SelfCapture)
            {
                return MoveResult.SelfCapture;
            }
            else if (IsKo(currentBoard, moveToMake, history) == MoveResult.Ko)
            {
                return MoveResult.Ko;
            }
            else if (IsSuperKo(currentBoard, moveToMake, history) == MoveResult.Ko)
            {
                return MoveResult.SuperKo;
            }
            else
            {
                return MoveResult.Legal;
            }
            
        }


    }
}
