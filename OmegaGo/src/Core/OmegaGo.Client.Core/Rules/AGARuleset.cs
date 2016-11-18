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
        
        public AGARuleset(Player white, Player black, GameBoardSize gbSize) : base(white, black, gbSize)
        {
            _isPreviousMovePass = false;
            _komi = 0.0f;
            _whiteScore = 0.0f;
            _blackScore = 0.0f;
        }

        protected override void SetKomi(int handicapStoneNumber)
        {
            if (handicapStoneNumber == 0)
            {
                _komi = 7.5f;
                _whiteScore = 7.5f;
            }
            else
            {
                _komi = 0.5f + handicapStoneNumber - 1;
            }
        }

        public override int CountScore(StoneColor[,] currentBoard)
        {
            throw new NotImplementedException();
        }

        protected override MoveResult Pass()
        {
            //TODO what if white passes first, then black passes? 
            //Rules: "white must make the last move- if necessary, an additional pass, with a stone passed to the opponent as usual"
            //What should I do? Just give 1 stone (point) to Black and return MoveResult.LifeDeadConfirmationPhase?
            if (_isPreviousMovePass)
            {
                //TODO increase opponent score
                return MoveResult.LifeDeathConfirmationPhase;
            }
            else
            {
                //TODO increase opponent score
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
