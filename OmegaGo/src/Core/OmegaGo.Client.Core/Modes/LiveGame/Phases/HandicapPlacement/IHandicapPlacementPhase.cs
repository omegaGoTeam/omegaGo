namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    public interface IHandicapPlacementPhase : IGamePhase
    {
        /// <summary>
        /// Handicap placement type
        /// </summary>
        HandicapPlacementType PlacementType { get; }

        /// <summary>
        /// Stones placed
        /// </summary>
        int StonesPlaced { get; }
    }
}
