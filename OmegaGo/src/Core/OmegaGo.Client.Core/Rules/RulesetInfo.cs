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
        
        internal static GameBoardSize BoardSize
        {
            get { return _gbSize; }
        }

        internal static GameBoard BoardState { get; set; }

        internal static GroupState GroupState { get; set; }

        internal RulesetInfo(GameBoardSize gbSize, GameBoard currentBoard, GroupState groupState)
        {
            _gbSize = gbSize;
            BoardState = currentBoard;
            GroupState = groupState;
        }
    }
}
