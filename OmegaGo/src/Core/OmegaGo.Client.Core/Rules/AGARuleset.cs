using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public class AGARuleset : Ruleset
    {
        public override void PutHandicapStone(Move moveToMake)
        {
            throw new NotImplementedException();
        }

        public override MoveResult ControlMove(Color[,] currentBoard, Move moveToMake, List<Color[,]> history)
        {
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

        public override int CountScore(Color[,] currentBoard)
        {
            throw new NotImplementedException();
        }
    }
}
