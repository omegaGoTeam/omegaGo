namespace OmegaGo.Core.Modes.LiveGame.Phases.Initialization
{
    class InitializationPhase : GamePhaseBase, IInitializationPhase
    {
        public InitializationPhase(GameController gameController) : base(gameController)
        {
        }

        public override GamePhaseType PhaseType => GamePhaseType.Initialization;

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

        public override void StartPhase()
        {
            BeginGame();
            GoToPhase(GamePhaseType.HandicapPlacement);
        }
    }
}
