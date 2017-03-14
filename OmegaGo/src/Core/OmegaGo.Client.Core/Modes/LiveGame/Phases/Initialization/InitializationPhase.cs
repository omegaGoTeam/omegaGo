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
            BeginGame();
            GoToPhase(GamePhaseType.HandicapPlacement);
        }
        
        private void BeginGame()
        {
            Controller.OnDebuggingMessage("Game begins!");
            foreach (var player in Controller.Players)
            {
                player.Agent.GameInitialized();
            }
        }
    }
}
