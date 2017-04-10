using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Game.Styles
{
    /// <summary>
    /// Represents a background color
    /// </summary>
    public struct BackgroundColor : IEquatable<BackgroundColor>
    {
        public BackgroundColor(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public byte Red { get; set; }

        public byte Green { get; set; }

        public byte Blue { get; set; }

        public static bool operator ==(BackgroundColor first, BackgroundColor second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Default omegaGo color
        /// </summary>
        public static BackgroundColor Default => new BackgroundColor(253, 210, 112);

        public static bool operator !=(BackgroundColor first, BackgroundColor second)
        {
            return !(first == second);
        }

        public bool Equals(BackgroundColor other)
        {
            return Red == other.Red && Green == other.Green && Blue == other.Blue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BackgroundColor && Equals((BackgroundColor)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Red.GetHashCode();
                hashCode = (hashCode * 397) ^ Green.GetHashCode();
                hashCode = (hashCode * 397) ^ Blue.GetHashCode();
                return hashCode;
            }
        }
    }
}
