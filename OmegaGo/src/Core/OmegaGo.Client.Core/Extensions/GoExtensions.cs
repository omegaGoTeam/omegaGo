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
        /// Turns the <see cref="Color"/> into either the string "B" or "W". 
        /// </summary>
        /// <param name="color">The color to transform into a string.</param>
        public static string ToIgsCharacterString(this Color color)
        {
            switch (color)
            {
                case Color.Black:
                    return "B";
                case Color.White:
                    return "W";
                default:
                    throw new ArgumentException("The IGS server does not accept colors other than Black or White.",
                        nameof(color));
            }
        } 
    }
}
