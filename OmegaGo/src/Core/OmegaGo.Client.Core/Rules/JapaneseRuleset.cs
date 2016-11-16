using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public class JapaneseRuleset : Ruleset
    {
        private bool _isPreviousMovePass = false;
        private int _komi;
        private int _whiteScore;
        private int _blackScore;
        private int _numberOfHandicapStone;

        public override void PutHandicapStone(Move moveToMake)
        {
            throw new NotImplementedException();
        }

        protected override MoveResult Pass()
        {
            if (_isPreviousMovePass)
            {
                //TODO check whether opponents score increases according to Japanese rules
                return MoveResult.LifeDeadConfirmationPhase;
            }
            else
            {
                //TODO check whether opponents score increases according to Japanese rules
                _isPreviousMovePass = true;
                return MoveResult.Legal;
            }

        }

        public override MoveResult IsLegalMove(StoneColor[,] currentBoard, Move moveToMake, List<StoneColor[,]> history)
        {
            _isPreviousMovePass = false;

            if (currentBoard[moveToMake.Coordinates.X, moveToMake.Coordinates.Y] != StoneColor.None)
            {
                return MoveResult.OccupiedPosition;
            }
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

        public override int CountScore(StoneColor[,] currentBoard)
        {
            throw new NotImplementedException();
        }
    }
}
