using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Two intersections are said to be adjacent if they are connected by a horizontal or vertical line with no other intersections between them.
    /// Two placed stones of the same color are said to be connected if it is possible to draw a path from one intersection to the other by passing through adjacent intersections of the same color.
    /// This class represents a group of stones which are connected.
    /// </summary>
    class Group
    {
        private readonly int _id;
        private readonly StoneColor _groupColor;
        private int _libertyCount;
        private List<Position> _members;
        private bool[,] _checkedInters;

        private IRulesetInfo _rulesetInfo;

        /// <summary>
        /// Initializes a new <see cref="Group"/>.
        /// </summary>
        /// <param name="id">Unique ID of new group.</param>
        /// <param name="color">The color of player who controlles the group.</param>
        /// <param name="info">Ruleset state.</param>
        public Group(int id, StoneColor color, IRulesetInfo info)
        {
            _id = id;
            _groupColor = color;
            _libertyCount = 0;
            _members = new List<Position>();
            _checkedInters = new bool[info.BoardSize.Width, info.BoardSize.Height];
            _rulesetInfo = info;

            _rulesetInfo.GroupState.Groups[id] = this;
        }

        /// <summary>
        /// Initializes a new <see cref="Group"/> as a copy of the given group.
        /// </summary>
        /// <param name="group">The group to copy.</param>
        /// <param name="info">Ruleset state.</param>
        public Group(Group group, IRulesetInfo info)
        {
            _id = group.ID;
            _groupColor = group.GroupColor;
            _libertyCount = group.LibertyCount;
            _members = new List<Position>();
            _members.AddRange(group.Members);
            _checkedInters = new bool[info.BoardSize.Width, info.BoardSize.Height];
            _rulesetInfo = info;

            for (int i = 0; i < info.BoardSize.Width; i++)
                for (int j = 0; j < info.BoardSize.Height; j++)
                    _checkedInters[i, j] = group._checkedInters[i, j];
        }
        
        /// <summary>
        /// Unique ID of group.
        /// </summary>
        public int ID
        {
            get { return _id; }
        }

        /// <summary>
        /// List of group members.
        /// </summary>
        public List<Position> Members
        {
            get { return _members;}

        }

        /// <summary>
        /// Count of empty intersections (liberties) around the group.
        /// </summary>
        public int LibertyCount
        {
            get { return _libertyCount; }
        }

        /// <summary>
        /// The color of player who controlles the group.
        /// </summary>
        public StoneColor GroupColor
        {
            get { return _groupColor; }
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
                _rulesetInfo.GroupState.GroupMap[position.X, position.Y] = _id;
                _checkedInters[position.X, position.Y] = true;
            }
            Position newp = new Position();

            //has same unchecked right neighbour
            if (position.X < _rulesetInfo.BoardSize.Width - 1 && _rulesetInfo.BoardState[position.X + 1, position.Y] == GroupColor && !_checkedInters[position.X + 1, position.Y])
            {
                newp.X = position.X + 1;
                newp.Y = position.Y;
                DiscoverGroup(newp);
            }
            //has same unchecked upper neighbour
            if (position.Y < _rulesetInfo.BoardSize.Height - 1 && _rulesetInfo.BoardState[position.X, position.Y + 1] == GroupColor && !_checkedInters[position.X, position.Y + 1])
            {
                newp.X = position.X;
                newp.Y = position.Y + 1;
                DiscoverGroup(newp);
            }
            //has same unchecked left neighbour
            if (position.X > 0 && _rulesetInfo.BoardState[position.X - 1, position.Y] == GroupColor && !_checkedInters[position.X - 1, position.Y])
            {
                newp.X = position.X - 1;
                newp.Y = position.Y;
                DiscoverGroup(newp);
            }
            //has same unchecked bottom neighbour
            if (position.Y > 0 && _rulesetInfo.BoardState[position.X, position.Y - 1] == GroupColor && !_checkedInters[position.X, position.Y - 1])
            {
                newp.X = position.X;
                newp.Y = position.Y - 1;
                DiscoverGroup(newp);
            }
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
            ChangeGroupMembersID(otherGroup.ID);
            otherGroup.AddMembersToGroupList(_members);
            _rulesetInfo.GroupState.Groups[ID] = null;
        }

        /// <summary>
        /// Changes the ID of group in group map.
        /// </summary>
        /// <param name="id">New group ID.</param>
        internal void ChangeGroupMembersID(int id)
        {
            foreach (Position member in _members)
            {
                _rulesetInfo.GroupState.GroupMap[member.X, member.Y] = id;
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
            _rulesetInfo.GroupState.GroupMap[position.X, position.Y] = _id;
            _libertyCount = GetLiberty(position);
            _checkedInters[position.X, position.Y] = true;
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
                List<int> neighbours = _rulesetInfo.GroupState.GetNeighbourGroups(member);
                foreach (int groupID in neighbours)
                {
                    _rulesetInfo.GroupState.Groups[groupID].IncreaseLibertyCount(1);
                }

                //delete from group map
                _rulesetInfo.GroupState.GroupMap[member.X, member.Y] = 0;
                //delete from board
                _rulesetInfo.BoardState[member.X, member.Y] = StoneColor.None;
                
            }
            else if (_members.Count > 1)
            {
                foreach (Position member in _members)
                {
                    //delete from group map
                    _rulesetInfo.GroupState.GroupMap[member.X, member.Y] = 0;
                    //delete from board
                    _rulesetInfo.BoardState[member.X, member.Y] = StoneColor.None;
                }
                //update liberties
                _rulesetInfo.GroupState.CountLiberties();
            }
            else
            {
                throw new Exception("The member list does not contain any member.");
            }

            //delete from group list
            _rulesetInfo.GroupState.Groups[_id] = null;
            
        }

        /// <summary>
        /// Increases the number of liberties.
        /// </summary>
        /// <param name="value">Liberty count</param>
        internal void IncreaseLibertyCount(int value)
        {
            _libertyCount += value;
        }

        /// <summary>
        /// Decreses the number of liberties.
        /// </summary>
        /// <param name="value">Liberty count</param>
        internal void DecreaseLibertyCount(int value)
        {
            if (_libertyCount - value >= 0)
                _libertyCount -= value;
            else
                throw new Exception("Liberty count cannot be lower than 0");
        }

        /// <summary>
        /// Calculates the number of empty intersection around the given position.
        /// </summary>
        /// <param name="position">Position on the board.</param>
        /// <returns>Returns the number of empty intersection around the given position.</returns>
        private int GetLiberty(Position position)
        {
            int liberty = 0;
            if (position.X > 0 && _rulesetInfo.BoardState[position.X - 1, position.Y] == StoneColor.None)
                liberty++;
            if (position.X < _rulesetInfo.BoardSize.Width - 1 && _rulesetInfo.BoardState[position.X + 1, position.Y] == StoneColor.None)
                liberty++;
            if (position.Y > 0 && _rulesetInfo.BoardState[position.X, position.Y - 1] == StoneColor.None)
                liberty++;
            if (position.Y < _rulesetInfo.BoardSize.Height - 1 && _rulesetInfo.BoardState[position.X, position.Y + 1] == StoneColor.None)
                liberty++;
            return liberty;
        }

    }
}
