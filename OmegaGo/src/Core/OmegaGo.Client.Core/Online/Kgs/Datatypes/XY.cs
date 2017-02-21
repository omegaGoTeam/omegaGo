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
    }
}