using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.Main
{
    public abstract class MainPhaseBase : GamePhaseBase, IMainPhase
    {
        protected MainPhaseBase(GameController gameController) : base(gameController)
        {
        }


        /// <summary>
        /// Main phase
        /// </summary>
        public override GamePhaseType Type => GamePhaseType.Main;

        /// <summary>
        /// Starts main phase. Observes player events and asks the first player to move.
        /// </summary>
        public override void StartPhase()
        {
            ObservePlayerEvents();
            AskFirstPlayerToMove();
        }

        /// <summary>
        /// Unsubscribes from player events
        /// </summary>
        public override void EndPhase()
        {
            UnobservePlayerEvents();
        }

        /// <summary>
        /// Asks the first player to make a move
        /// </summary>
        private void AskFirstPlayerToMove()
        {
            //decides who starts the game based on handicap information
            Controller.TurnPlayer = Controller.Info.NumberOfHandicapStones > 0 ?
                Controller.Players.White :
                Controller.Players.Black;

            Controller.OnDebuggingMessage(Controller.TurnPlayer + " begins!");

            //inform the agent that he is on turn
            Controller.TurnPlayer.Agent.PleaseMakeAMove();
        }

        /// <summary>
        /// Attaches player events
        /// </summary>
        private void ObservePlayerEvents()
        {
            foreach (var player in Controller.Players)
            {
                player.Agent.PlaceStone += Agent_PlaceStone;
                player.Agent.Pass += Agent_Pass;
            }
        }

        /// <summary>
        /// Detaches player events
        /// </summary>
        private void UnobservePlayerEvents()
        {
            foreach (var player in Controller.Players)
            {
                player.Agent.PlaceStone -= Agent_PlaceStone;
                player.Agent.Pass -= Agent_Pass;
            }
        }

        /// <summary>
        /// Handles the agent's place stone event
        /// </summary>
        /// <param name="agent">Agent who placed the stone</param>
        /// <param name="position">Position played</param>
        private void Agent_PlaceStone(IAgent agent, Position position)
        {
            Move attemptedMove = Move.PlaceStone(agent.Color, position);
            TryToMakeMove(attemptedMove);
        }

        /// <summary>
        /// Handles the agent's pass event
        /// </summary>
        /// <param name="agent">Agent</param>
        private void Agent_Pass(IAgent agent)
        {
            Move attemptedMove = Move.Pass(agent.Color);
            TryToMakeMove(attemptedMove);
        }

        private void TryToMakeMove(Move move)
        {
            var player = Controller.Players[move.WhoMoves];
            if (player != Controller.TurnPlayer)
                throw new InvalidOperationException("It is not your turn.");

            //ask the ruleset to validate the move
            MoveProcessingResult processingResult =
                   Controller.Ruleset.ProcessMove(
                       Controller.GameTree.LastNode?.BoardState ?? new GameBoard(Controller.Info.BoardSize),
                       move,
                       Controller.GameTree.GameTreeRoot?.GetTimelineView.Select(node => node.BoardState).ToArray() ?? new GameBoard[0]);

            //are we about to enter life and death phase?
            if (processingResult.Result == MoveResult.StartLifeAndDeath)
            {
                //TODO: IS THIS REALLY NECESSARY?
                // (Petr) yes (because we want to keep the server in control)
                //if (this.Controller.IsOnlineGame)
                //{
                //    processingResult.Result = MoveResult.Legal;
                //}
                GoToPhase(GamePhaseType.LifeDeathDetermination);
                return;
            }

            //is the move illegal?
            if (processingResult.Result != MoveResult.Legal)
            {
                Controller.OnDebuggingMessage("That move was illegal: " + processingResult.Result);

                //handle the illegal move
                if (HandlePlayersIllegalMove(move, processingResult) != IllegalMoveHandling.PermitItAnyway)
                {
                    //stop processing, this stone will not be placed
                    return;
                }
            }

            //we have a legal move
            if (move.Kind == MoveKind.PlaceStone)
            {
                move.Captures.AddRange(processingResult.Captures);
            }

            //TODO: WHY NOT IN ONLINE GAME?
            // (Petr) because we want to keep the server in control (it has more authoritative information)
            // if ( !Controller.IsOnlineGame && ... )
            if (player.Clock.IsViolating())
            {
                ClockOut(player);
                return;
            }

            //applies the legal move
            Controller.OnDebuggingMessage(Controller.TurnPlayer + " moves: " + move);
            ApplyMove(move, processingResult.NewBoard);

            //switches players
            Controller.SwitchTurnPlayer();
            Controller.OnDebuggingMessage("Asking " + Controller.TurnPlayer + " to make a move.");
            Controller.TurnPlayer.Agent.PleaseMakeAMove();
        }

        /// <summary>
        /// Applies a move in the game state
        /// </summary>
        /// <param name="move">Move</param>
        /// <param name="newBoard">Game board state after the move</param>
        private void ApplyMove(Move move, GameBoard newBoard)
        {
            //add new move to game tree
            Controller.GameTree.AddMoveToEnd(move, newBoard);

            //inform the players that a move occured
            foreach (var connector in Controller.Connectors)
            {
                connector.MovePerformed(move);
            }
        }

        /// <summary>
        /// Handles an illegal move
        /// </summary>
        /// <param name="move">Move</param>
        /// <param name="processingResult">Why was the move illegal</param>
        private IllegalMoveHandling HandlePlayersIllegalMove(Move move, MoveProcessingResult processingResult)
        {
            var player = Controller.Players[move.WhoMoves];
            switch (player.Agent.IllegalMoveHandling)
            {
                case IllegalMoveHandling.InformAgent:
                    player.Agent.MoveIllegal(processingResult.Result);
                    break;
                case IllegalMoveHandling.PassInstead:
                    Controller.OnDebuggingMessage("Passing instead.");
                    TryToMakeMove(Move.Pass(move.WhoMoves));
                    break;
                case IllegalMoveHandling.PermitItAnyway:
                    Controller.OnDebuggingMessage("Permitting it anyway.");
                    processingResult.Result = MoveResult.Legal;
                    break;
            }
            return player.Agent.IllegalMoveHandling;
        }

        //TODO: Extract last node deletion to a GameTree method and ensure the undo works with IGS
        /// <summary>
        /// Undoes the last move made, regardless of which player made it. This is called whenever the server commands
        /// us to undo, or whenever the user clicks to locally undo.
        /// </summary>
        public void Undo()
        {
            //is there a move to undo?
            if (Controller.GameTree.LastNode != null)
            {
                Controller.GameTree.RemoveLastNode();
                Controller.SwitchTurnPlayer();
                // Order here matters:
                //(this._turnPlayer.Agent as OnlineAgent)?.Undo();
                //_game.NumberOfMovesPlayed--;
                Controller.TurnPlayer.Agent.PleaseMakeAMove();
                // TODO
                Controller.OnBoardMustBeRefreshed();
            }
        }

        /// <summary>
        /// Called when a player clocks out. Ends the game as a timeout.
        /// </summary>
        /// <param name="player">Player that clocked out</param>
        private void ClockOut(GamePlayer player)
        {
            var endGameInformation = GameEndInformation.CreateTimeout(player, Controller.Players);
            Controller.EndGame(endGameInformation);
        }
    }
}
