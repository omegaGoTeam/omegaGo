using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public class ChineseRuleset : Ruleset
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

        public override MoveResult ControlMove(StoneColor[,] currentBoard, Move moveToMake, List<StoneColor[,]> history)
        {
            if (moveToMake.Kind == MoveKind.Pass && _isPreviousMovePass)
            {
                //TODO check whether opponents score increases according to Chinese rules
                return MoveResult.LifeDeadConfirmationPhase;
            }
            else if (moveToMake.Kind == MoveKind.Pass)
            {
                //TODO check whether opponents score increases according to Chinese rules
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

        public override int CountScore(StoneColor[,] currentBoard)
        {
            throw new NotImplementedException();
        }

    }
}
