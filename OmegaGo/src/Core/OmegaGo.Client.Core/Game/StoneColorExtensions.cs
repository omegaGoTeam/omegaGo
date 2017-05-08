using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game
{
    public static class StoneColorExtensions
    {
        /// <summary>
        /// Returns the opponent's color for a given color
        /// </summary>
        /// <param name="color">Stone color</param>
        /// <returns>Opponent's color</returns>
        public static StoneColor GetOpponentColor(this StoneColor color)
        {
            if (color == StoneColor.Black) return StoneColor.White;
            if (color == StoneColor.White) return StoneColor.Black;        
            throw new ArgumentOutOfRangeException(nameof(color), "None color has no opponent");
        }

        public static StoneColor GetOpponentColor(this StoneColor color, GameTreeNode prevNode, GameTreeNode root)
        {
            if (color == StoneColor.White)
            {
                return StoneColor.Black;
            }
            else if (color == StoneColor.Black)
            {
                return StoneColor.White;
            }
            else
            {
                if (prevNode.Equals(root))
                    return StoneColor.Black;
                else
                    return StoneColor.White;
            }
        }
    }
}
