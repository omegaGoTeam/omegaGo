using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Extensions
{
    public static class GoExtensions
    {
        /// <summary>
        /// Gets the opposite color. For black, it returns white. For white, it returns black. For none, it fails
        /// with an exception.
        /// </summary>
        /// <param name="color">A Go stone color.</param>
        public static StoneColor GetOpponentColor(this StoneColor color)
        {
            switch (color)
            {
                case StoneColor.Black:
                    return StoneColor.White;
                case StoneColor.White:
                    return StoneColor.Black;
                default:
                    throw new ArgumentException("The argument must be black or white.");
            }
        }
        /// <summary>
        /// Turns the <see cref="StoneColor"/> into either the string "B" or "W". 
        /// </summary>
        /// <param name="color">The color to transform into a string.</param>
        public static string ToIgsCharacterString(this StoneColor color)
        {
            switch (color)
            {
                case StoneColor.Black:
                    return "B";
                case StoneColor.White:
                    return "W";
                default:
                    throw new ArgumentException("The IGS server does not accept colors other than Black or White.",
                        nameof(color));
            }
        } 
    }
}
