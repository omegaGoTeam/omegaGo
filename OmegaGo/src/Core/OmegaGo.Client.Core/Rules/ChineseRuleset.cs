using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void ModifyScoresAfterLDDeterminationPhase(int deadWhiteStoneCount, int deadBlackStoneCount)
        {
            return; //Chinese ruleset uses area counting, we do not need number of dead stones
        }

        protected override void SetKomi(int handicapStoneCount)
        {
            if (handicapStoneCount == 0)
            {
                _komi = 7.5f;
            }
            else 
            {
                _komi = 0.5f + handicapStoneCount - 1;
            }

        }

        protected override void ModifyScoresAfterCapture(int capturedStoneCount, StoneColor removedStonesColor)
        {
            return; //Chinese ruleset uses area counting, we do not need number of captured stones
        }

        protected override MoveResult Pass(StoneColor playerColor)
        {
            if (_isPreviousMovePass)
            {
                //TODO check whether opponents score increases according to Chinese rules
                return MoveResult.LifeDeathDeterminationPhase;
            }
            else 
            {
                //TODO check whether opponents score increases according to Chinese rules
                _isPreviousMovePass = true;
                return MoveResult.Legal;
            }

        }

        protected override MoveResult CheckSelfCaptureKoSuperko(GameBoard currentBoard, Move moveToMake, List<GameBoard> history)
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
