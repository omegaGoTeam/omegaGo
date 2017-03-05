using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    class GroupState
    {
        private static Group[] _groups;
        private static int[,] _groupMap;
        private static GameBoardSize _gbSize;

        public static Group[] Groups
        {
            get { return _groups; }
        }

        public static int[,] GroupMap
        {
            get { return _groupMap; }
        }

        public static GameBoardSize BoardSize
        {
            get { return _gbSize; }
        }

        public GroupState(GameBoardSize gbSize)
        {
            _groups = new Group[gbSize.Height * gbSize.Width];
            _gbSize = gbSize;
        }

        public void RefreshState()
        {

        }

        internal void CreateNewGroup()
        {
        }

        
        internal int GetUniqueID()
        {
            return 0;
        }
    }
}
