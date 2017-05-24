using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.LiveGame.Players.Agents;
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
        /// Requests that the latest move (and or moves in case of play against AI or internet) be undone. The returned task is considered
        /// completed as soon as the request is made, it doesn't need to actually take effect (or indeed, be accepted at all, in case of
        /// online play). In local play, this always eventually results in an undo. In online play, it results in an undo request.
        /// </summary>
        protected abstract Task MainRequestUndo();

        protected abstract void MainForceUndo();

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
            foreach(var connector in Controller.Connectors)
            {
                connector.MainUndoRequested += Connector_MainUndoRequested;
                connector.MainUndoForced += Connector_MainUndoForced;
            }
            AskFirstPlayerToMove();
        }

        /// <summary>
        /// Unsubscribes from player events
        /// </summary>
        public override void EndPhase()
        {
            foreach (var connector in Controller.Connectors)
            {
                connector.MainUndoRequested -= Connector_MainUndoRequested;
                connector.MainUndoForced -= Connector_MainUndoForced;
            }
            UnobservePlayerEvents();
        }
        
        /// <summary>
        /// Undoes the last move made, regardless of which player made it. This is called whenever the server commands
        /// us to undo, or whenever the user clicks to locally undo.
        /// </summary>
        public void Undo(int howManyMoves)
        {
            Controller.OnDebuggingMessage("Undoing " + howManyMoves + " moves:");
            //is there a move to undo?
            for (int i = 0; i < howManyMoves; i++)
            {
                if (!Controller.GameTree.LastNode.Equals(Controller.GameTree.GameTreeRoot))
                {
                    Controller.GameTree.LastNode = Controller.GameTree.LastNode.Parent;
                    foreach (var player in this.Controller.Players)
                    {
                        player.Agent.MoveUndone();
                    }
                    Controller.OnMoveUndone();
                    Controller.SwitchTurnPlayer();
                    Controller.OnDebuggingMessage("Move undone.");
                }
                else
                {
                    Controller.OnDebuggingMessage("Undo failed.");
                }
            }
            Controller.TurnPlayer.Agent.PleaseMakeAMove();
            
        }

        /// <summary>
        /// Alters the processing result before it is handled by the move logic
        /// </summary>
        /// <param name="move">Move processed</param>
        /// <param name="result">Result of processing</param>
        protected virtual void AlterMoveProcessingResult(Move move, MoveProcessingResult result)
        {
            //keep the rules logic inact
        }

        /// <summary>
        /// Handles the local clockout. Returns true if processing the current move should be aborted. 
        /// </summary>
        /// <param name="player">Player that clocked out</param>
        protected abstract bool HandleLocalClockOut(GamePlayer player);

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
        /// Asks the first player to make a move
        /// </summary>
        protected virtual void AskFirstPlayerToMove()
        {
            //decides who starts the game based on handicap information
            Controller.TurnPlayer = Controller.Info.NumberOfHandicapStones > 0 ?
                Controller.Players.White :
                Controller.Players.Black;

            Controller.OnDebuggingMessage(Controller.TurnPlayer + " begins!");

            //inform the agent that he is on turn
            Controller.TurnPlayer.Agent.PleaseMakeAMove();
        }

        private void Connector_MainUndoForced(object sender, EventArgs e)
        {
            MainForceUndo();
        }

        private async void Connector_MainUndoRequested(object sender, EventArgs e)
        {
            await MainRequestUndo();
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
            MoveProcessingResult processingResult = Controller.Ruleset.ProcessMove(Controller.GameTree.LastNode, move);

            //let the specific game controller alter the processing result to match game type
            AlterMoveProcessingResult(move, processingResult);
            

            //are we about to enter life and death phase?
            if (processingResult.Result == MoveResult.StartLifeAndDeath)
            {
                if (player.Clock.IsViolating())
                {
                    if (HandleLocalClockOut(player))
                    {
                        return;
                    }
                }

                //applies the legal move
                Controller.OnDebuggingMessage(Controller.TurnPlayer + " moves: " + move);
                ApplyMove(move, processingResult.NewBoard, processingResult.NewGroupState);

                //switches players
                Controller.SwitchTurnPlayer();

                // moves next
                GoToPhase(GamePhaseType.LifeDeathDetermination);
                return;
            }

            //is the move illegal?
            if (processingResult.Result != MoveResult.Legal)
            {
                Controller.OnDebuggingMessage("That move was illegal: " + processingResult.Result);

                //handle the illegal move, this may alter the 
                HandlePlayersIllegalMove(move, processingResult);
            }
            // These MUST NOT be an "else" here because HandlePlayersIllegalMove may change processingResult.
            if (processingResult.Result == MoveResult.Legal)
            {
                //we have a legal move
                if (move.Kind == MoveKind.PlaceStone)
                {
                    move.Captures.AddRange(processingResult.Captures);
                }

                if (player.Clock.IsViolating())
                {
                    if (HandleLocalClockOut(player))
                    {
                        return;
                    }
                }

                //applies the legal move
                Controller.OnDebuggingMessage(Controller.TurnPlayer + " moves: " + move);
                ApplyMove(move, processingResult.NewBoard, processingResult.NewGroupState);

                DetermineNextTurnPlayer();

                Controller.OnDebuggingMessage("Asking " + Controller.TurnPlayer + " to make a move.");
                Controller.TurnPlayer.Agent.PleaseMakeAMove();
            }
        }

        /// <summary>
        /// Passes the turn to the next player who's supposed to be on turn. This will usually be the other player 
        /// than the one who played last. In free handicap placement, this may be different.
        /// This method should only change <see cref="GameController.TurnPlayer"/> and do nothing else.
        /// </summary>
        protected virtual void DetermineNextTurnPlayer()
        {
            //switches players
            Controller.SwitchTurnPlayer();
        }

        /// <summary>
        /// Applies a move in the game state
        /// </summary>
        /// <param name="move">Move</param>
        /// <param name="newBoard">Game board state after the move</param>
        private void ApplyMove(Move move, GameBoard newBoard, GroupState newGroupState)
        {
            //add new move to game tree
            Controller.GameTree.AddMoveToEnd(move, newBoard, newGroupState);

            //inform the ui and the internet that a move occured
            foreach (var connector in Controller.Connectors)
            {
                connector.MovePerformed(move);
            }
            //inform the players that a move occured
            foreach (var informedPlayer in Controller.Players)
            {
                informedPlayer.Agent.MovePerformed(move);
            }
        }

        /// <summary>
        /// Handles an illegal move
        /// </summary>
        /// <param name="move">Move</param>
        /// <param name="processingResult">Why was the move illegal</param>
        private void HandlePlayersIllegalMove(Move move, MoveProcessingResult processingResult)
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
        }
    }
}
