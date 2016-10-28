using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    public class Ruleset
    {
        public int Score;

        public void startGame() {
            throw new NotImplementedException();
        }

        public void PutHandicapStone(Move moveToMake) {
            throw new NotImplementedException();
        }

        public bool controlMove(GameBoard currentBoard, Move moveToMake, GameBoard[] history) {
            throw new NotImplementedException();
            //return true;
        }

        public GameBoard ControlCaptureAndRemoveStones(GameBoard currentBoard) {
            throw new NotImplementedException();
        }

        public int CountScore(GameBoard currentBoard) {
            throw new NotImplementedException();
        }

        private bool IsLegal(Move moveToMake) {
            throw new NotImplementedException();
        }

        private bool IsKo(GameBoard currentBoard, Move moveToMake, GameBoard[] history) {
            //TODO: return code/type of illegal move
            throw new NotImplementedException();
        }

        private bool IsOccupiedPosition(GameBoard currentBoard, Move moveToMake) {
            //TODO: return code/type of illegal move
            throw new NotImplementedException();
        }

        private bool IsSelfCapture(GameBoard currentBoard, Move ) {
            //TODO: return code/type of illegal move
            throw new NotImplementedException();
        }
    }
}

