namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    interface IHandicapPlacementPhase : IGamePhase
    {
        /// <summary>
        /// Handicap placement type
        /// </summary>
        HandicapPlacementType PlacementType { get; }
    }
}
