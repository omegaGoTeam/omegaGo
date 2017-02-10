namespace OmegaGo.Core.Modes.LiveGame.Phases.Initialization
{
    /// <summary>
    /// Game initialization phase
    /// </summary>
    class InitializationPhase : GamePhaseBase, IInitializationPhase
    {
        /// <summary>
        /// Creates initializaiton phase
        /// </summary>
        /// <param name="gameController">Game controller</param>
        public InitializationPhase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Initialization phase
        /// </summary>
        public override GamePhaseType PhaseType => GamePhaseType.Initialization;

        /// <summary>
        /// Starts the initialization phase
        /// </summary>
        public override void StartPhase()
        {
            BeginGame();
            GoToPhase(GamePhaseType.HandicapPlacement);
        }
        
        /// <summary>
        /// Begins the main game loop by asking the first player (who plays black) to make a move, and then the second player, then the first,
        /// and so on until the game concludes. This method will return immediately but it will launch this loop in a Task on another thread.
        /// </summary>
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
