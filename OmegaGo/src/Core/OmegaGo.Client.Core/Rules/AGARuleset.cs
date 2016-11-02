using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public class AGARuleset : Ruleset
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

        public override MoveResult ControlMove(Color[,] currentBoard, Move moveToMake, List<Color[,]> history)
        {
        //TODO what if white passes first, then black passes? 
        //Rules: "white must make the last move- if necessary, an additional pass, with a stone passed to the opponent as usual"
        //What should I do? Just give 1 stone (point) to Black and return MoveResult.LifeDeadConfirmationPhase?
            if (moveToMake.Kind == MoveKind.Pass && _isPreviousMovePass)
            {
                //TODO increase opponent score
                return MoveResult.LifeDeadConfirmationPhase;
            }
            else if (moveToMake.Kind == MoveKind.Pass)
            {
                //TODO increase opponent score
                _isPreviousMovePass = true;
                return MoveResult.Legal;
            }
            else {
                _isPreviousMovePass = false;

                if (IsPositionOccupied(currentBoard, moveToMake) == MoveResult.OccupiedPosition)
                {
                    return MoveResult.OccupiedPosition;
                }
                else if (IsKo(currentBoard, moveToMake, history) == MoveResult.Ko)
                {
                    return MoveResult.Ko;
                }
                else if (IsSuperKo(currentBoard, moveToMake, history) == MoveResult.Ko)
                {
                    return MoveResult.SuperKo;
                }
                else if (IsSelfCapture(currentBoard, moveToMake) == MoveResult.SelfCapture)
                {
                    return MoveResult.SelfCapture;
                }
                else
                {
                    return MoveResult.Legal;
                }
            }
        }

        public override int CountScore(Color[,] currentBoard)
        {
            throw new NotImplementedException();
        }
    }
}
