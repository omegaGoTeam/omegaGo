using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public class GroupState
    {
        private Group[] _groups;
        private int[,] _groupMap;
        
        internal Group[] Groups
        {
            get { return _groups; }
        }

        internal int[,] GroupMap
        {
            get { return _groupMap; }
        }

        public GroupState(GameBoardSize gbSize)
        {
            _groups = new Group[gbSize.Height * gbSize.Width];
            _groupMap = new int[gbSize.Width, gbSize.Height];
        }
        internal void SetState(Group[] groups, int[,] groupMap)
        {
            _groups = groups;
            _groupMap = groupMap;
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
            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                {
                    if (RulesetInfo.BoardState[i, j] != StoneColor.None && _groupMap[i, j] == 0)
                        CreateNewGroup(RulesetInfo.BoardState[i, j], new Position(i, j));
                }
        }

        /// <summary>
        /// Call after discovering all groups.
        /// </summary>
        private void CountLiberties()
        {
            foreach (Group g in _groups)
                g.DecreaseLibertyCount(g.LibertyCount);

            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
            {
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                {
                    if (RulesetInfo.BoardState[i, j] == StoneColor.None)
                    {
                        int groupID = 0;
                        //left group
                        if (i > 0 && _groupMap[i - 1, j] != 0)
                        {
                            groupID = _groupMap[i - 1, j];
                            _groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (i < RulesetInfo.BoardSize.Width - 1 && _groupMap[i + 1, j] != 0) //right group
                        {
                            groupID = _groupMap[i + 1, j];
                            _groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (j > 0 && _groupMap[i, j - 1] != 0) //bottom group
                        {
                            groupID = _groupMap[i, j - 1];
                            _groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (j < RulesetInfo.BoardSize.Height - 1 && _groupMap[i, j + 1] != 0) //upper group
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
