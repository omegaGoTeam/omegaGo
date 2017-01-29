using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Online.Igs
{
    public static class IgsStoneColorExtensions
    {
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
