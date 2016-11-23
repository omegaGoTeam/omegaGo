using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public class JapaneseRuleset : Ruleset
    {
        private bool _isPreviousMovePass;
        private float _komi;
        private float _whiteScore;
        private float _blackScore;
        

        public JapaneseRuleset(Player white, Player black, GameBoardSize gbSize) : base(white, black, gbSize)
        {
            _isPreviousMovePass = false;
            _komi = 0.0f;
            _whiteScore = 0.0f;
            _blackScore = 0.0f;
        }

        public override Scores CountScore(StoneColor[,] currentBoard)
        {
            Scores scores;
            scores = CountTerritory(currentBoard);

            scores.WhiteScore += _komi + _whiteScore;
            scores.BlackScore += _blackScore;

            return scores;
        }

        public override void ModifyScoresAfterLDConfirmationPhase(int deadWhiteStoneCount, int deadBlackStoneCount)
        {
            _whiteScore -= deadWhiteStoneCount;
            _blackScore -= deadBlackStoneCount;
        }

        protected override void SetKomi(int handicapStoneNumber)
        {
            if (handicapStoneNumber == 0)
            {
                _komi = 6.5f;
            }
            else
            {
                _komi = 0.5f;
            }
        }

        protected override MoveResult Pass(StoneColor playerColor)
        {
            if (_isPreviousMovePass)
            {
                return MoveResult.LifeDeathConfirmationPhase;
            }
            else
            {
                _isPreviousMovePass = true;
                return MoveResult.Legal;
            }

        }

        protected override MoveResult CheckSelfCaptureKoSuperko(StoneColor[,] currentBoard, Move moveToMake, List<StoneColor[,]> history)
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
            else
            {
                return MoveResult.Legal;
            }
            
        }

        protected override void ModifyScoresAfterCapture(int capturedStoneCount, StoneColor removedStonesColor)
        {
            if (removedStonesColor == StoneColor.Black)
                _blackScore -= capturedStoneCount;
            else if (removedStonesColor == StoneColor.White)
                _whiteScore -= capturedStoneCount;
        }

    }
}
