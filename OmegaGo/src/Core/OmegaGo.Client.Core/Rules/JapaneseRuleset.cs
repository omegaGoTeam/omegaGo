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
            scores = CountTerritory(currentBoard);

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

        public override void ModifyScoresAfterLDDeterminationPhase(int deadWhiteStoneCount, int deadBlackStoneCount)
        {
            _whiteScore -= deadWhiteStoneCount;
            _blackScore -= deadBlackStoneCount;
        }

        protected override void SetKomi(int handicapStoneCount)
        {
            if (handicapStoneCount == 0)
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
                return MoveResult.LifeDeathDeterminationPhase;
            }
            else
            {
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
