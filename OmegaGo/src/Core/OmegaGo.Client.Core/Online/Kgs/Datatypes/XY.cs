using OmegaGo.Core.Game;

namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// In a KGS SGF property, represents a location on the game board, or a pass.
    /// </summary>
    public class XY
    {
        public int X { get; set; }
        public int Y { get; set; }
        /// <summary>
        /// If this is true, then the values of <see cref="X"/> and <see cref="Y"/> are meaningless.  
        /// </summary>
        public bool IsPass { get; set; }

        public bool SameAs(XY xy)
        {
            return this.X == xy.X && this.Y == xy.Y && this.IsPass == xy.IsPass;
        }

        public override string ToString()
        {
            return new Position(X, Y).ToIgsCoordinates() + "(KGS!)";
        }
    }
}