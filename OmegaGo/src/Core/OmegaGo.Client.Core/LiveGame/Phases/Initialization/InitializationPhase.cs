using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.Core.Modes.LiveGame.State;

namespace OmegaGo.Core.Modes.LiveGame.Phases.Initialization
{
    /// <summary>
    /// Game initialization phase
    /// </summary>
    class InitializationPhase : GamePhaseBase, IInitializationPhase
    {
        /// <summary>
        /// Creates initialization phase
        /// </summary>
        /// <param name="gameController">Game controller</param>
        public InitializationPhase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Initialization phase
        /// </summary>
        public override GamePhaseType Type => GamePhaseType.Initialization;

        /// <summary>
        /// Starts the initialization phase
        /// </summary>
        public override void StartPhase()
        {
            Controller.OnDebuggingMessage("Game begins!");
            bool thisIsAGoodFuegoGame = false;
            foreach (var player in Controller.Players)
            {
                if (player.Agent is AiAgent && (player.Agent as AiAgent).AI is Fuego)
                {
                    if (FuegoSingleton.Instance.CurrentGame != null && !thisIsAGoodFuegoGame)
                    {
                        // Fuego can't be in two games at once.
                        Controller.EndGame(GameEndInformation.CreateCancellation(Controller.Players));
                        return;
                    }
                    if (!thisIsAGoodFuegoGame)
                    {
                        FuegoSingleton.Instance.CurrentGame = Controller;
                        thisIsAGoodFuegoGame = true;
                    }
                }
                player.Agent.GameInitialized();
            }
            GoToPhase(GamePhaseType.HandicapPlacement);
        }
    }
}
