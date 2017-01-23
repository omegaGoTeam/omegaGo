using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Contains scores for individual players
    /// </summary>
    public class Scores
    {
        /// <summary>
        /// Score of the black player
        /// </summary>
        public float BlackScore { get; set; }
        
        /// <summary>
        /// Score of the white player
        /// </summary>
        public float WhiteScore { get; set; }

        /// <summary>
        /// Gets or sets score for a given stone color
        /// </summary>
        /// <param name="color">Stone color</param>
        /// <returns>Score</returns>
        public float this[StoneColor color ]
        {
            get
            {
                switch (color)
                {
                    case StoneColor.Black:
                        return BlackScore;                        
                    case StoneColor.White:
                        return WhiteScore;                        
                    default:
                        throw new ArgumentOutOfRangeException(nameof(color), color, null);
                }
            }
            set
            {
                switch (color)
                {                    
                    case StoneColor.Black:
                        BlackScore = value;
                        break;
                    case StoneColor.White:
                        WhiteScore = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(color), color, null);
                }
            }
        }
    }
}
