using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    /// <summary>
    /// A generic game controller phase factory, uses generic type parameters to specify game phases
    /// </summary>
    /// <typeparam name="TInitializationPhase">Initialization phase</typeparam>
    /// <typeparam name="THandicapPlacementPhase">Handicap placement phase</typeparam>
    /// <typeparam name="TMainPhase">Main phase</typeparam>
    /// <typeparam name="TLifeAndDeathPhase">Life and death phase</typeparam>
    /// <typeparam name="TFinishedPhase">Finished phase</typeparam>
    internal class GenericPhaseFactory<TInitializationPhase, THandicapPlacementPhase, TMainPhase, TLifeAndDeathPhase, TFinishedPhase> : IGameControllerPhaseFactory
        where TInitializationPhase : GamePhaseBase, IInitializationPhase
        where THandicapPlacementPhase : GamePhaseBase, IHandicapPlacementPhase
        where TMainPhase : GamePhaseBase, IMainPhase
        where TLifeAndDeathPhase : GamePhaseBase, ILifeAndDeathPhase
        where TFinishedPhase : GamePhaseBase, IFinishedPhase
    {
        public IGamePhase CreatePhase(GamePhaseType phaseType, IGameController controller)
        {
            Type phaseTypeToCreate;
            switch (phaseType)
            {
                case GamePhaseType.Initialization:
                    phaseTypeToCreate = typeof(TInitializationPhase);
                    break;
                case GamePhaseType.HandicapPlacement:
                    phaseTypeToCreate = typeof(THandicapPlacementPhase);
                    break;
                case GamePhaseType.Main:
                    phaseTypeToCreate = typeof(TMainPhase);
                    break;
                case GamePhaseType.LifeDeathDetermination:
                    phaseTypeToCreate = typeof(TLifeAndDeathPhase);
                    break;
                case GamePhaseType.Finished:
                    phaseTypeToCreate = typeof(TFinishedPhase);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phaseType), phaseType, null);
            }
            return (IGamePhase)Activator.CreateInstance(phaseTypeToCreate, controller);
        }
    }
}
