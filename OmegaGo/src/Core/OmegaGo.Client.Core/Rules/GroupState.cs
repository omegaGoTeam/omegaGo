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
        internal Group[] Groups { get; set; }

        internal int[,] GroupMap { get; set; }

        public GroupState(GameBoardSize gbSize)
        {
            Groups = new Group[gbSize.Height * gbSize.Width];
            GroupMap = new int[gbSize.Width, gbSize.Height];
        }

        /// <summary>
        /// Creates new group intance.
        /// </summary>
        /// <param name="color">Color of stones in group.</param>
        /// <param name="position"></param>
        /// <returns></returns>
        internal Group CreateNewGroup(StoneColor color, Position position)
        {
            int ID = GetUniqueID();
            Group newGroup = new Group(ID, color);
            newGroup.AddStoneToEmptyGroup(position);
            return newGroup;
        }

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


        /// <summary>
        /// Counts the liberties of groups: For each empty intersection increases the liberty of neighbour groups.
        /// Call after discovering all groups.
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
    }
}
