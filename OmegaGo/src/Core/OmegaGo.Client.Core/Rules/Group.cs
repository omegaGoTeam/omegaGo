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
        private readonly StoneColor _groupColor;
        private int _libertyCount;
        private List<Position> _members;
        private bool[,] _checkedInters;

        public Group(int id, StoneColor color)
        {
            _id = id;
            _groupColor = color;
            _libertyCount = 0;
            _members = new List<Position>();
            _checkedInters = new bool[RulesetInfo.BoardSize.Width, RulesetInfo.BoardSize.Height];
            RulesetInfo.GroupState.Groups[id] = this;
        }

        /// <summary>
        /// Initializes a new <see cref="Group"/> as a copy of the given group.
        /// </summary>
        /// <param name="group">The group to copy.</param>
        public Group(Group group)
        {
            _id = group.ID;
            _groupColor = group.GroupColor;
            _libertyCount = group.LibertyCount;
            _members = new List<Position>();
            _members.AddRange(group.Members);
            _checkedInters = new bool[RulesetInfo.BoardSize.Width, RulesetInfo.BoardSize.Height];
            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                    _checkedInters[i, j] = group._checkedInters[i, j];
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
        /// Joins two groups and updates the liberties.
        /// </summary>
        /// <param name="otherGroup">Group, which will be joined with this group.</param>
        internal void JoinGroupWith(Group otherGroup)
        {
            if (otherGroup.GroupColor != _groupColor)
                throw new Exception("The colors of groups do not equal");

            otherGroup.IncreaseLibertyCount(_libertyCount);
            otherGroup.ChangeGroupMembersID(otherGroup.ID, _members);
            otherGroup.AddMembersToGroupList(_members);
            RulesetInfo.GroupState.Groups[ID] = null;
        }

        /// <summary>
        /// Changes the ID of group.
        /// </summary>
        /// <param name="id">New group ID.</param>
        /// <param name="memberList">The members of group.</param>
        internal void ChangeGroupMembersID(int id,List<Position> memberList)
        {
            foreach (Position member in memberList)
            {
                RulesetInfo.GroupState.GroupMap[member.X, member.Y] = id;
            }
        }

        /// <summary>
        /// Adds new members to the group's member list.
        /// </summary>
        /// <param name="memberList">List of members to add.</param>
        internal void AddMembersToGroupList(List<Position> memberList)
        {
            foreach (Position p in memberList)
            {
                _members.Add(p);
            }
        }

        /// <summary>
        /// Adds a position to group's member list and group map. Sets the liberty.
        /// </summary>
        /// <param name="position">Position for adding to member list.</param>
        internal void AddStoneToEmptyGroup(Position position)
        {
            if (_members.Count != 0)
                throw new Exception("Cannot add stone to non empty group. Use join.");

            _members.Add(position);
            RulesetInfo.GroupState.GroupMap[position.X, position.Y] = _id;
            _libertyCount = GetLiberty(position);
            _checkedInters[position.X, position.Y] = true;
        }

        /// <summary>
        /// Calculates the number of empty intersection around the given position.
        /// </summary>
        /// <param name="position">Position on the board.</param>
        /// <returns>Returns the number of empty intersection around the given position.</returns>
        private int GetLiberty(Position position)
        {
            int liberty = 0;
            if (position.X > 0 && RulesetInfo.BoardState[position.X - 1, position.Y] == StoneColor.None)
                liberty++;
            if (position.X < RulesetInfo.BoardSize.Width - 1 && RulesetInfo.BoardState[position.X + 1, position.Y] == StoneColor.None)
                liberty++;
            if (position.Y > 0 && RulesetInfo.BoardState[position.X, position.Y - 1] == StoneColor.None)
                liberty++;
            if (position.Y < RulesetInfo.BoardSize.Height - 1 && RulesetInfo.BoardState[position.X, position.Y + 1] == StoneColor.None)
                liberty++;
            return liberty;
        }
        
        /// <summary>
        /// Delete the group from group map, group list and board.
        /// Updates the liberties.
        /// </summary>
        internal void DeleteGroup()
        {
            if (_members.Count == 1)
            {
                // and update neighbour liberties
                Position member = _members.First();
                List<int> neighbours = GetNeighbourGroups(member);
                foreach (int groupID in neighbours)
                {
                    RulesetInfo.GroupState.Groups[groupID].IncreaseLibertyCount(1);
                }

                //delete from group map
                RulesetInfo.GroupState.GroupMap[member.X, member.Y] = 0;
                //delete from board
                RulesetInfo.BoardState[member.X, member.Y] = StoneColor.None;
                
            }
            else if (_members.Count > 1)
            {
                foreach (Position member in _members)
                {
                    //delete from group map
                    RulesetInfo.GroupState.GroupMap[member.X, member.Y] = 0;
                    //delete from board
                    RulesetInfo.BoardState[member.X, member.Y] = StoneColor.None;
                }
                //update liberties
                RulesetInfo.GroupState.CountLiberties();
            }
            else
            {
                throw new Exception("The member list does not contain any member.");
            }

            //delete from group list
            RulesetInfo.GroupState.Groups[_id] = null;
            
        }

        /// <summary>
        /// Increases the number of liberties.
        /// </summary>
        /// <param name="value">//TODO Aniko</param>
        internal void IncreaseLibertyCount(int value)
        {
            _libertyCount += value;
        }

        /// <summary>
        /// Decreses the number of liberties.
        /// </summary>
        /// <param name="value">//TODO Aniko</param>
        internal void DecreaseLibertyCount(int value)
        {
            if (_libertyCount - value >= 0)
                _libertyCount -= value;
            else
                throw new Exception("Liberty count cannot be lower than 0");
        }

        /// <summary>
        /// Finds the members of a group.
        /// </summary>
        /// <param name="position">Starting position.</param>
        public void DiscoverGroup(Position position)
        {
            if (!_checkedInters[position.X, position.Y])
            {
                _members.Add(position);
                RulesetInfo.GroupState.GroupMap[position.X, position.Y] = _id;
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

        /// <summary>
        /// Returns the IDs of groups around the intersection.
        /// </summary>
        /// <param name="position">Coordinates of intersection</param>
        /// <returns>IDs of groups around the intersection</returns>
        internal List<int> GetNeighbourGroups(Position position)
        {
            List<int> neighbours = new List<int>();
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
