using System;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Contains scores for individual players.
    /// </summary>
    public class Scores
    {
        /// <summary>
        /// Creates empty scores.
        /// </summary>
        public Scores()
        {
        }

        /// <summary>
        /// Creates scores
        /// </summary>
        /// <param name="blackScore">Black's score</param>
        /// <param name="whiteScore">White's score</param>
        public Scores(float blackScore, float whiteScore)
        {
            BlackScore = blackScore;
            WhiteScore = whiteScore;
        }

        /// <summary>
        /// Score of the black player.
        /// </summary>
        public float BlackScore { get; set; }
        
        /// <summary>
        /// Score of the white player.
        /// </summary>
        public float WhiteScore { get; set; }

        /// <summary>
        /// Absolute score difference.
        /// </summary>
        public float AbsoluteScoreDifference => Math.Abs(this.WhiteScore - this.BlackScore);

        /// <summary>
        /// Gets or sets score for a given stone color.
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
