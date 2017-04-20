namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Represents the type of couting method.
    /// There are two ways to score. One is based on territory, the other on area.
    /// </summary>
    public enum CountingType
    {
        /// <summary>
        /// The area of a player are all live stones of player left on the board together with any points of his territory. 
        /// This counting method adds up total area of players. In this case, prisoners are ignored.
        /// </summary>
        Area,
        /// <summary>
        /// The territory of a player are those empty points on the board which are entirely surrounded by his live stones. 
        /// This counting method adds up total territory of players. The scores include prisoners and dead stones. 
        /// </summary>
        Territory
    }
}
