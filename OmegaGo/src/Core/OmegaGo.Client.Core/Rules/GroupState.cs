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
        private static GameBoard _boardState;

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

        public static GameBoard BoardState
        {
            get { return _boardState; }
        }

        public GroupState(GameBoardSize gbSize)
        {
            _groups = new Group[gbSize.Height * gbSize.Width];
            _gbSize = gbSize;
        }

        internal void SetState(Group[] groups, int[,] groupMap, GameBoard boardState)
        {
            _groups = groups;
            _groupMap = groupMap;
            _boardState = boardState;
        }

        internal void CreateNewGroup(StoneColor color, Position position)
        {
            int ID = GetUniqueID();
            Group newGroup = new Group(ID, color);
            newGroup.DiscoverGroup(position);
        }

        internal int GetUniqueID()
        {
            //ID from 1
            for (int i = 1; i < _groups.Length; i++)
            {
                if (_groups[i] == null)
                    return i;
            }

            throw new Exception("more group than intersection");
        }

        private void FillGroupMap()
        {
            for (int i = 0; i < _gbSize.Width; i++)
                for (int j = 0; j < _gbSize.Height; j++)
                {
                    if (_boardState[i, j] != StoneColor.None && _groupMap[i, j] == 0)
                        CreateNewGroup(_boardState[i, j], new Position(i, j));
                }
        }

        /// <summary>
        /// Call after discovering all groups.
        /// </summary>
        private void CountLiberties()
        {
            foreach (Group g in _groups)
                g.DecreaseLibertyCount(g.LibertyCount);

            for (int i = 0; i < _gbSize.Width; i++)
            {
                for (int j = 0; j < _gbSize.Height; j++)
                {
                    if (_boardState[i, j] == StoneColor.None)
                    {
                        int groupID = 0;
                        //left group
                        if (i > 0 && _groupMap[i - 1, j] != 0)
                        {
                            groupID = _groupMap[i - 1, j];
                            _groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (i < _gbSize.Width - 1 && _groupMap[i + 1, j] != 0) //right group
                        {
                            groupID = _groupMap[i + 1, j];
                            _groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (j > 0 && _groupMap[i, j - 1] != 0) //bottom group
                        {
                            groupID = _groupMap[i, j - 1];
                            _groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (j < _gbSize.Height - 1 && _groupMap[i, j + 1] != 0) //upper group
                        {
                            groupID = _groupMap[i, j + 1];
                            _groups[groupID].IncreaseLibertyCount(1);
                        }
                    }
                }
            }
        }
    }
}
