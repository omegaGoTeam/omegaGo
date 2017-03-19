using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    internal class RulesetInfo
    {
        private static GameBoardSize _gbSize;
        private static GameBoard _boardState;
        private static GroupState _groupState;

        internal static GameBoardSize BoardSize
        {
            get { return _gbSize; }
        }

        internal static GameBoard BoardState
        {
            get { return _boardState; }
        }

        internal static GroupState GroupState
        {
            get { return _groupState; }
        }

        internal RulesetInfo(GameBoardSize gbSize, GameBoard currentBoard, GroupState groupState)
        {
            _gbSize = gbSize;
            _boardState = currentBoard;
            _groupState = groupState;
        }

        internal void SetRulesetInfo(GameBoard currentBoard, GroupState groupState)
        {
            _boardState = currentBoard;
            _groupState = groupState;
        }
    }
}
