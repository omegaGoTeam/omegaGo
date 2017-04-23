using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Represents the current state of groups on the board: list of groups and group map.
    /// </summary>
    public class GroupState
    {
        private IRulesetInfo _rulesetInfo;

        /// <summary>
        /// Initializes a new <see cref="GroupState"/>.
        /// </summary>
        /// <param name="info">Ruleset state.</param>
        public GroupState(IRulesetInfo info)
        {
            _rulesetInfo = info;
            Groups = new Group[info.BoardSize.Height * info.BoardSize.Width];
            GroupMap = new int[info.BoardSize.Width, info.BoardSize.Height];
        }

        /// <summary>
        /// Initializes a new <see cref="GroupState"/> as a copy of the given group state.
        /// </summary>
        /// <param name="groupState">The group state to copy.</param>
        /// <param name="info">Ruleset state.</param>
        public GroupState(GroupState groupState, IRulesetInfo info)
            : this(info)
        {
            for (int x = 0; x < info.BoardSize.Width; x++)
            {
                for (int y = 0; y < info.BoardSize.Height; y++)
                {
                    GroupMap[x, y] = groupState.GroupMap[x, y];
                }
            }

            for (int i = 0; i < groupState.Groups.Length; i++)
                if (groupState.Groups[i] != null)
                    Groups[i] = new Group(groupState.Groups[i], info);
        }

        /// <summary>
        /// List of groups on the table.
        /// </summary>
        internal Group[] Groups { get; set; }

        /// <summary>
        /// Table (map) of intersections containing the ID of group to which the intersection belongs.
        /// </summary>
        internal int[,] GroupMap { get; set; }

        /// <summary>
        /// Finds the smallest unused group ID.
        /// </summary>
        /// <returns>The smallest unused ID.</returns>
        internal int GetUniqueID()
        {
            //ID from 1
            for (int i = 1; i < Groups.Length; i++)
            {
                if (Groups[i] == null)
                    return i;
            }

            throw new Exception("more group than intersection");
        }

        /// <summary>
        /// Creates new group intance with one member.
        /// </summary>
        /// <param name="color">Color of stones in group.</param>
        /// <param name="position">The position of group member.</param>
        /// <returns>Created group with one member.</returns>
        internal Group CreateNewGroup(StoneColor color, Position position)
        {
            int ID = GetUniqueID();
            Group newGroup = new Group(ID, color, _rulesetInfo);
            newGroup.AddStoneToEmptyGroup(position);
            return newGroup;
        }
        
        /// <summary>
        /// Adds stone to the group map, group list and board.
        /// </summary>
        /// <param name="position">Position on the board.</param>
        /// <param name="color">Color of stone.</param>
        internal void AddStoneToBoard(Position position, StoneColor color)
        {
            Group newGroup= CreateNewGroup(color,position);
            List<int> neighbourGroups = GetNeighbourGroups(position);

            foreach (int groupID in neighbourGroups)
            {
                Group group = Groups[groupID];
                group.DecreaseLibertyCount(1);
                //join
                if (group.GroupColor == newGroup.GroupColor)
                {
                    //choose group with smaller ID
                    if (group.ID < newGroup.ID)
                    {
                        newGroup.JoinGroupWith(group);
                        newGroup = group;
                    }
                    else
                        group.JoinGroupWith(newGroup);
                }
            }

            _rulesetInfo.BoardState[position.X, position.Y] = color;
        }

        /// <summary>
        /// Adds stone to the group map, group list and board, but the possible group join is ignored.
        /// </summary>
        /// <param name="position">Position on the board.</param>
        /// <param name="color">Color of stone.</param>
        internal void AddTempStoneToBoard(Position position, StoneColor color)
        {
            Group newGroup = CreateNewGroup(color, position);
            List<int> neighbourGroups = GetNeighbourGroups(position);

            foreach (int groupID in neighbourGroups)
            {
                Group group = Groups[groupID];
                group.DecreaseLibertyCount(1);
                if (group.GroupColor == color && group.LibertyCount > 0)
                    newGroup.IncreaseLibertyCount(1);
            }

            _rulesetInfo.BoardState[position.X, position.Y] = color;
        }

        /// <summary>
        /// Removes temporarily added stone from group map, group list and board.
        /// </summary>
        /// <param name="position">Position of stone.</param>
        internal void RemoveTempStoneFromPosition(Position position)
        {
            List<int> neighbourGroups = GetNeighbourGroups(position);

            foreach (int groupID in neighbourGroups)
            {
                Group group = Groups[groupID];
                group.IncreaseLibertyCount(1);
            }

            int ID = GroupMap[position.X, position.Y];
            _rulesetInfo.BoardState[position.X, position.Y] = StoneColor.None;
            Groups[ID] = null;
            GroupMap[position.X, position.Y] = 0;
        }

        /// <summary>
        /// Counts the liberties of groups: For each empty intersection increases the liberty of neighbour groups.
        /// Call after discovering all groups (method FillGroupMap()).
        /// </summary>
        internal void CountLiberties()
        {
            //init liberties to 0
            foreach (Group g in Groups)
                if (g != null)
                    g.DecreaseLibertyCount(g.LibertyCount);

            //count liberties
            for (int i = 0; i < _rulesetInfo.BoardSize.Width; i++)
            {
                for (int j = 0; j < _rulesetInfo.BoardSize.Height; j++)
                {
                    if (_rulesetInfo.BoardState[i, j] == StoneColor.None)
                    {
                        List<int> neighbourGroups = GetNeighbourGroups(new Position(i, j));
                        foreach (int groupID in neighbourGroups)
                        {
                            Groups[groupID].IncreaseLibertyCount(1);
                        }
                    }
                }
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
            int left = (position.X == 0) ? 0 : GroupMap[position.X - 1, position.Y];
            int right = (position.X == _rulesetInfo.BoardSize.Width - 1) ? 0 : GroupMap[position.X + 1, position.Y];
            int bottom = (position.Y == 0) ? 0 : GroupMap[position.X, position.Y - 1];
            int upper = (position.Y == _rulesetInfo.BoardSize.Height - 1) ? 0 : GroupMap[position.X, position.Y + 1];

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

        /// <summary>
        /// Finds the groups on the board.
        /// </summary>
        /// <param name="currentBoard">State of game board.</param>
        internal void FillGroupMap(GameBoard currentBoard)
        {
            for (int i = 0; i < _rulesetInfo.BoardSize.Width; i++)
                for (int j = 0; j < _rulesetInfo.BoardSize.Height; j++)
                {
                    if (_rulesetInfo.BoardState[i, j] != StoneColor.None && GroupMap[i, j] == 0)
                    {
                        Position position = new Position(i, j);
                        Group newGroup = CreateNewGroup(_rulesetInfo.BoardState[i, j], position);
                        newGroup.DiscoverGroup(position);
                    }
                }
        }

    }
}
