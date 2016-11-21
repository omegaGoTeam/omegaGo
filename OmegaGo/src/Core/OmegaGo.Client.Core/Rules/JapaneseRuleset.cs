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

        public override int CountScore(StoneColor[,] currentBoard)
        {
            throw new NotImplementedException();
        }

        protected override MoveResult Pass(StoneColor playerColor)
        {
            if (_isPreviousMovePass)
            {
                //TODO check whether opponents score increases according to Japanese rules
                return MoveResult.LifeDeathConfirmationPhase;
            }
            else
            {
                //TODO check whether opponents score increases according to Japanese rules
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

        
    }
}
