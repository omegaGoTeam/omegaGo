using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public class AGARuleset : Ruleset
    {
        private bool _isPreviousMovePass;
        private float _komi;
        private float _whiteScore;
        private float _blackScore;
        private CountingType _countingType;
        
        public AGARuleset(Player white, Player black, GameBoardSize gbSize, CountingType countingType) : base(white, black, gbSize)
        {
            _isPreviousMovePass = false;
            _komi = 0.0f;
            _whiteScore = 0.0f;
            _blackScore = 0.0f;
            _countingType = countingType;
        }

        public override Scores CountScore(StoneColor[,] currentBoard)
        {
            Scores scores;
            if (_countingType == CountingType.Area)
                scores = CountArea(currentBoard);
            else
                scores = CountTerritory(currentBoard);

            scores.WhiteScore += _komi+_whiteScore;
            scores.BlackScore += _blackScore;
            return scores;
        }

        public override void ModifyScoresAfterLDConfirmationPhase(int deadWhiteStoneCount, int deadBlackStoneCount)
        {
            if (_countingType == CountingType.Territory)
            {
                _whiteScore -= deadWhiteStoneCount;
                _blackScore -= deadBlackStoneCount;
            }
        }

        protected override void SetKomi(int handicapStoneNumber)
        {
            if (handicapStoneNumber == 0)
            {
                _komi = 7.5f;
            }
            else if (handicapStoneNumber > 0 && _countingType == CountingType.Area)
            {
                _komi = 0.5f + handicapStoneNumber - 1;
            }
            else if (handicapStoneNumber > 0 && _countingType == CountingType.Territory)
            {
                _komi = 0.5f;
            }
        }
        
        protected override MoveResult Pass(StoneColor playerColor)
        {
            StoneColor opponentColor = (playerColor == StoneColor.Black) ? StoneColor.White : StoneColor.Black;

            //increase opponent's score
            if (opponentColor == StoneColor.Black)
                _blackScore++;
            else
                _whiteScore++;
            
            //check previous move
            if (_isPreviousMovePass)
            {
                return MoveResult.LifeDeathConfirmationPhase;
            }

            // Black player starts the passing
            if (playerColor == StoneColor.Black) 
            {
                _isPreviousMovePass = true;
            }

            return MoveResult.Legal;
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
            else if (IsSuperKo(currentBoard, moveToMake, history) == MoveResult.Ko)
            {
                return MoveResult.SuperKo;
            }
            else
            {
                return MoveResult.Legal;
            }
            
        }

        protected override void ModifyScoresAfterCapture(int capturedStoneCount, StoneColor removedStonesColor)
        {
            if (_countingType == CountingType.Territory)
            {
                if (removedStonesColor == StoneColor.Black)
                    _blackScore -= capturedStoneCount;
                else if (removedStonesColor == StoneColor.White)
                    _whiteScore -= capturedStoneCount;
            }
        }

    }
}
