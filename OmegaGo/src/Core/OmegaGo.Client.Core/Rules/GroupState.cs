using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Represents the current state of groups on the board: list of groups and group map.
    /// </summary>
    public class GroupState
    {
        /// <summary>
        /// Initializes a new <see cref="GroupState"/>.
        /// </summary>
        /// <param name="gbSize">The size of game board.</param>
        public GroupState(GameBoardSize gbSize)
        {
            Groups = new Group[gbSize.Height * gbSize.Width];
            GroupMap = new int[gbSize.Width, gbSize.Height];
        }

        /// <summary>
        /// Initializes a new <see cref="GroupState"/> as a copy of the given group state.
        /// </summary>
        /// <param name="gameState">The group state to copy.</param>
        public GroupState(GroupState groupState)
            : this(RulesetInfo.BoardSize)
        {
            for (int x = 0; x < RulesetInfo.BoardSize.Width; x++)
            {
                for (int y = 0; y < RulesetInfo.BoardSize.Height; y++)
                {
                    GroupMap[x, y] = groupState.GroupMap[x, y];
                }
            }

            for (int i = 0; i < groupState.Groups.Length; i++)
                if (groupState.Groups[i] != null)
                    Groups[i] = new Group(groupState.Groups[i]);
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
            Group newGroup = new Group(ID, color);
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
            List<int> neighbourGroups = newGroup.GetNeighbourGroups(position);
            foreach (int groupID in neighbourGroups)
            {
                Group group = RulesetInfo.GroupState.Groups[groupID];
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
                        group.JoinGroupWith(group);
                }
            }
            RulesetInfo.BoardState[position.X, position.Y] = color;
        }

        /// <summary>
        /// Counts the liberties of groups: For each empty intersection increases the liberty of neighbour groups.
        /// Call after discovering all groups (method FillGroupMap()).
        /// </summary>
        internal void CountLiberties()
        {
            //init liberties to 0
            foreach (Group g in Groups)
                g.DecreaseLibertyCount(g.LibertyCount);

            //count liberties
            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
            {
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                {
                    if (RulesetInfo.BoardState[i, j] == StoneColor.None)
                    {
                        int groupID = 0;
                        //left group
                        if (i > 0 && GroupMap[i - 1, j] != 0)
                        {
                            groupID = GroupMap[i - 1, j];
                            Groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (i < RulesetInfo.BoardSize.Width - 1 && GroupMap[i + 1, j] != 0) //right group
                        {
                            groupID = GroupMap[i + 1, j];
                            Groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (j > 0 && GroupMap[i, j - 1] != 0) //bottom group
                        {
                            groupID = GroupMap[i, j - 1];
                            Groups[groupID].IncreaseLibertyCount(1);
                        }
                        else if (j < RulesetInfo.BoardSize.Height - 1 && GroupMap[i, j + 1] != 0) //upper group
                        {
                            groupID = GroupMap[i, j + 1];
                            Groups[groupID].IncreaseLibertyCount(1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the groups on the board.
        /// </summary>
        private void FillGroupMap()
        {
            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                {
                    if (RulesetInfo.BoardState[i, j] != StoneColor.None && GroupMap[i, j] == 0)
                    {
                        Position position = new Position(i, j);
                        Group newGroup = CreateNewGroup(RulesetInfo.BoardState[i, j], position);
                        newGroup.DiscoverGroup(position);
                    }
                }
        }

    }
}
