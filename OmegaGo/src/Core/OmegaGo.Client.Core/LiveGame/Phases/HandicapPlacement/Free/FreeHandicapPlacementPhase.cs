using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Free
{
    /// <summary>
    /// Default local game implementation of the free handicap phase
    /// </summary>
    class FreeHandicapPlacementPhase : HandicapPlacementPhaseBase
    {
        /// <summary>
        /// Game board used for handicap placement
        /// </summary>
        private GameBoard _gameBoard = null;

        public FreeHandicapPlacementPhase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Free handicap placement
        /// </summary>
        public override HandicapPlacementType PlacementType => HandicapPlacementType.Free;

        /// <summary>
        /// Starts the phase
        /// </summary>
        public override void StartPhase()
        {
            ObservePlayerEvents();
            if (Controller.Info.NumberOfHandicapStones > 0)
            {
                _gameBoard = new GameBoard(Controller.Info.BoardSize);
                //Controller.GameTree.AddBoardToEnd(_gameBoard);
                //let the black player play
                Controller.TurnPlayer = Controller.Players.Black;
                Controller.TurnPlayer.Agent.PleaseMakeAMove();
            }
            else
            {
                //skip this phase and continue to main
                GoToPhase(GamePhaseType.Main);
            }
        }

        /// <summary>
        /// Ends the phase
        /// </summary>
        public override void EndPhase()
        {
            UnobservePlayerEvents();
        }

        /// <summary>
        /// Handles black player's attempt to place a stone
        /// </summary>
        /// <param name="agent">Agent that placed the stone</param>
        /// <param name="position">Position</param>
        private void BlackPlayerPlaceFreeHandicapStone( IAgent agent, Position position )
        {
            var moveResult = Controller.Ruleset.PlaceFreeHandicapStone(ref _gameBoard, position);
            if (moveResult == MoveResult.Legal)
            {
                //add the placed stone
                StonesPlaced++;
                if (StonesPlaced == Controller.Info.NumberOfHandicapStones)
                {
                    Controller.GameTree.AddBoardToEnd(_gameBoard, new GroupState(Controller.Ruleset.RulesetInfo.GroupState, Controller.Ruleset.RulesetInfo));
                    //start main phase                    
                    GoToPhase(GamePhaseType.Main);
                }
                else
                {
                    //place another handicap stone
                    Controller.TurnPlayer.Agent.PleaseMakeAMove();
                }
            }
            else
            {
                //inform the player
                if (agent.IllegalMoveHandling == IllegalMoveHandling.InformAgent)
                {
                    agent.MoveIllegal(moveResult);
                }
            }
        }

        /// <summary>
        /// Starts observing player events
        /// </summary>
        private void ObservePlayerEvents()
        {
            Controller.Players.Black.Agent.PlaceStone += BlackPlayerPlaceFreeHandicapStone;
        }

        /// <summary>
        /// Stops observing player events
        /// </summary>
        private void UnobservePlayerEvents()
        {
            Controller.Players.Black.Agent.PlaceStone -= BlackPlayerPlaceFreeHandicapStone;
        }
    }
}