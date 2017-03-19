using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    class Group
    {
        private readonly int _id;
        private List<Position> _members;
        private int _libertyCount;
        private readonly StoneColor _groupColor;
        private bool[,] _checkedInters;

        public Group(int id, StoneColor color)
        {
            _id = id;
            _groupColor = color;
            _libertyCount = 0;
            _members = new List<Position>();
            _checkedInters = new bool[RulesetInfo.BoardSize.Width, RulesetInfo.BoardSize.Height];
        }

        public int ID
        {
            get { return _id; }
        }

        public List<Position> Members
        {
            get { return _members;}

        }

        public int LibertyCount
        {
            get { return _libertyCount; }
        }

        public StoneColor GroupColor
        {
            get { return _groupColor; }
        }

        /// <summary>
        /// Adds a stone to the group and updates the liberty count of neighbours.
        /// </summary>
        /// <param name="position"></param>
        internal void AddStoneToGroup(Position position)
        {
            if (!_members.Contains(position))
            {
                _members.Add(position);
                List<int> neighbours= GetNeighbourGroups(position);
                foreach (int group in neighbours)
                {
                    RulesetInfo.GroupState.Groups[group].DecreaseLibertyCount(1);
                }
            }
        }

        /// <summary>
        /// Joins two groups.
        /// </summary>
        /// <param name="otherGroup"></param>
        internal void JoinGroupWith(Group otherGroup)
        {
            if (otherGroup.GroupColor != _groupColor)
                throw new Exception("The colors of groups do not equal");

            otherGroup.IncreaseLibertyCount(_libertyCount);
            otherGroup.ChangeGroupMembersID(otherGroup.ID, _members);
            otherGroup.AddMembersToGroupList(_members);

            RulesetInfo.GroupState.Groups[ID] = null;
        }

        internal void ChangeGroupMembersID(int id,List<Position> memberList)
        {
            foreach (Position member in memberList)
            {
                RulesetInfo.GroupState.GroupMap[member.X, member.Y] = id;
            }
        }

        internal void AddMembersToGroupList(List<Position> memberList)
        {
            foreach (Position p in memberList)
            {
                _members.Add(p);
            }
        }
        

        internal void DeleteGroup()
        {
            GameBoard prevBoard = new GameBoard(RulesetInfo.BoardState);
            int[,] prevGroupMap = RulesetInfo.GroupState.GroupMap; //!!!TODO Aniko: clone
            Group[] prevGroups = RulesetInfo.GroupState.Groups;
            foreach (Position member in _members)
            {
                prevBoard[member.X, member.Y] = StoneColor.None;
                prevGroupMap[member.X, member.Y] = 0;
            }

            prevGroups[_id] = null;


        }

        internal void IncreaseLibertyCount(int value)
        {
            _libertyCount += value;
        }

        internal void DecreaseLibertyCount(int value)
        {
            if (_libertyCount - value >= 0)
                _libertyCount -= value;
            else
                throw new Exception("Liberty count cannot be lower than 0");
        }

        public void DiscoverGroup(Position position)
        {
            if (!_checkedInters[position.X, position.Y])
            {
                _members.Add(position);
                _checkedInters[position.X, position.Y] = true;
            }
            Position newp = new Position();

            //has same unchecked right neighbour
            if (position.X < RulesetInfo.BoardSize.Width - 1 && RulesetInfo.BoardState[position.X + 1, position.Y] == GroupColor && !_checkedInters[position.X + 1, position.Y])
            {
                newp.X = position.X + 1;
                newp.Y = position.Y;
                DiscoverGroup(newp);
            }
            //has same unchecked upper neighbour
            if (position.Y < RulesetInfo.BoardSize.Height - 1 && RulesetInfo.BoardState[position.X, position.Y + 1] == GroupColor && !_checkedInters[position.X, position.Y + 1])
            {
                newp.X = position.X;
                newp.Y = position.Y + 1;
                DiscoverGroup( newp);
            }
            //has same unchecked left neighbour
            if (position.X > 0 && RulesetInfo.BoardState[position.X - 1, position.Y] == GroupColor && !_checkedInters[position.X - 1, position.Y])
            {
                newp.X = position.X - 1;
                newp.Y = position.Y;
                DiscoverGroup( newp);
            }
            //has same unchecked bottom neighbour
            if (position.Y > 0 && RulesetInfo.BoardState[position.X, position.Y - 1] == GroupColor && !_checkedInters[position.X, position.Y - 1])
            {
                newp.X = position.X;
                newp.Y = position.Y - 1;
                DiscoverGroup( newp);
            }
        }

        private List<int> GetNeighbourGroups(Position position)
        {
            List<int> neighbours = new List<int>();
            StoneColor opponent = StoneColorExtensions.GetOpponentColor(_groupColor);
            int left = (position.X == 0) ? 0 : RulesetInfo.GroupState.GroupMap[position.X - 1, position.Y];
            int right = (position.X == RulesetInfo.BoardSize.Width - 1) ? 0 : RulesetInfo.GroupState.GroupMap[position.X + 1, position.Y];
            int bottom = (position.Y == 0) ? 0 : RulesetInfo.GroupState.GroupMap[position.X, position.Y - 1];
            int upper = (position.Y == RulesetInfo.BoardSize.Height - 1) ? 0 : RulesetInfo.GroupState.GroupMap[position.X, position.Y + 1];
            if (left > 0)
                neighbours.Add(left);
            if (right > 0)
                neighbours.Add(right);
            if (upper > 0)
                neighbours.Add(upper);
            if (bottom > 0)
                neighbours.Add(bottom);

            return neighbours.Distinct().ToList();
        }
    }
}
