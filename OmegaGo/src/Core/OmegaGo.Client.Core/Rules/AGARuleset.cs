using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    public class AGARuleset : Ruleset
    {
        private bool _isPreviousMovePass;
        private float _komi;
        private float _whiteScore;
        private float _blackScore;
        private CountingType _countingType;
        
        public AGARuleset(GameBoardSize gbSize, CountingType countingType) : base(gbSize)
        {
            _isPreviousMovePass = false;
            _komi = 0.0f;
            _whiteScore = 0.0f;
            _blackScore = 0.0f;
            _countingType = countingType;
        }

        public override Scores CountScore(GameBoard currentBoard)
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

        public static float GetAGACompensation(GameBoardSize gbSize, int handicapStoneCount, CountingType cType)
        {
            float compensation = 0;
            if (handicapStoneCount == 0)
                compensation = 7.5f;
            else if (handicapStoneCount > 0 && cType == CountingType.Area)
                compensation = 0.5f + handicapStoneCount - 1;
            else if (handicapStoneCount > 0 && cType == CountingType.Territory)
                compensation = 0.5f;
            
            return compensation;
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
                return MoveResult.StartLifeAndDeath;
            }

            // Black player starts the passing
            if (playerColor == StoneColor.Black) 
            {
                _isPreviousMovePass = true;
            }

            return MoveResult.Legal;
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
