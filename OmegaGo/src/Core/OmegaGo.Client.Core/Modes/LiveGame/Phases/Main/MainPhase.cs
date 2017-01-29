using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.Main
{
    internal class MainPhase : GamePhaseBase, IMainPhase
    {
        public MainPhase(GameController gameController) : base(gameController)
        {
        }

        public override void StartPhase()
        {
            ObservePlayerEvents();
            AskFirstPlayerToMove();
        }

        public override void EndPhase()
        {
            UnobservePlayerEvents();
        }

        public override GamePhaseType PhaseType => GamePhaseType.Main;


        private void AskFirstPlayerToMove()
        {
            //handicapped game?
            if (Controller.Info.NumberOfHandicapStones > 0)
            {
                Controller.TurnPlayer = Controller.Players.White;
            }
            else
            {
                Controller.TurnPlayer = Controller.Players.Black;
            }
            Controller.OnDebuggingMessage(Controller.TurnPlayer + " begins!");
            Controller.TurnPlayer.Agent.PleaseMakeAMove();
        }

        private void ObservePlayerEvents()
        {
            foreach (var player in Controller.Players)
            {
                player.Agent.PlaceStone += HandleStonePlacement;
                player.Agent.Pass += Agent_Pass;
            }
        }

        private void Agent_Pass(object sender, EventArgs e)
        {
            var agent = (sender as IAgent);
            if (agent != null)
            {
                Move attemptedMove = Move.Pass(agent.Color);
                TryToMakeMove(attemptedMove);
            }
        }

        private void UnobservePlayerEvents()
        {
            foreach (var player in Controller.Players)
            {
                player.Agent.PlaceStone -= HandleStonePlacement;
                player.Agent.Pass -= Agent_Pass;
            }
        }

        private void HandleStonePlacement(object sender, Position e)
        {
            var agent = (sender as IAgent);
            if (agent != null)
            {
                Move attemptedMove = Move.PlaceStone(agent.Color, e);
                TryToMakeMove(attemptedMove);
            }
        }

        private void TryToMakeMove(Move move)
        {
            var player = Controller.Players[move.WhoMoves];
            if (player != Controller.TurnPlayer)
                throw new InvalidOperationException("It is not your turn.");

            MoveProcessingResult result =
                   Controller.Ruleset.ProcessMove(
                       Controller.GameTree.LastNode?.BoardState ?? new GameBoard(Controller.Info.BoardSize),
                       move,
                       Controller.GameTree.GameTreeRoot?.GetTimelineView.Select(node => node.BoardState).ToList() ?? new List<GameBoard>()); 
            
            if (result.Result == MoveResult.StartLifeAndDeath)
            {
                GoToPhase(GamePhaseType.LifeDeathDetermination);
                return;
            }
            else if (result.Result != MoveResult.Legal)
            {
                switch (player.Agent.IllegalMoveHandling)
                {
                    case IllegalMoveHandling.InformAgent:
                        player.Agent.MoveIllegal(result.Result);
                        break;
                    case IllegalMoveHandling.PassInstead:
                        TryToMakeMove(Move.Pass(move.WhoMoves));
                        break;
                    case IllegalMoveHandling.PermitItAnyway:
                        result.Result = MoveResult.Legal;
                        break;
                }
                //TODO: Extend to include server based illegal move validation
                //HandleIllegalMove(player, ref result);
                if (result.Result != MoveResult.Legal)
                {
                    // Still illegal.
                    return;
                }
            }

            if (move.Kind == MoveKind.PlaceStone)
            {
                move.Captures.AddRange(result.Captures);
            }

            // The move stands, let's make the other player move now.
            Controller.NumberOfMoves++;
            var newNode = Controller.GameTree.AddMoveToEnd(move, new GameBoard(result.NewBoard));            
            Controller.CurrentNode = newNode;
            Controller.SwitchTurnPlayer();
            Controller.TurnPlayer.Agent.PleaseMakeAMove();
            Controller.OnDebuggingMessage("Asking " + Controller.TurnPlayer + " to make a move.");
        }

        //private void HandleIllegalMove(GamePlayer player, ref MoveProcessingResult result)
        //{
        //    if (player.Agent.HowToHandleIllegalMove == IllegalMoveHandling.PermitItAnyway)
        //    {
        //        OnDebuggingMessage("The agent asked us to make an ILLEGAL MOVE and we are DOING IT ANYWAY!");
        //        result.Result = MoveResult.Legal;
        //        return;
        //    }
        //    if (_game.Server == null) // In server games, we always permit all moves and leave the verification on the server.
        //    {
        //        if (this.EnforceRules)
        //        {
        //            // Move is forbidden.
        //            OnDebuggingMessage("Move is illegal because: " + result.Result);
        //            if (_turnPlayer.Agent.HowToHandleIllegalMove == IllegalMoveHandling.Retry)
        //            {
        //                OnDebuggingMessage("Illegal move - retrying.");
        //                _turnPlayer.Agent.PleaseMakeAMove();
        //            }
        //            else if (_turnPlayer.Agent.HowToHandleIllegalMove == IllegalMoveHandling.MakeRandomMove)
        //            {

        //                OnDebuggingMessage("Illegal move - making a random move instead.");
        //                List<Position> possibleMoves = _game.Ruleset?.GetAllLegalMoves(player.Color,
        //                    ObsoleteFastBoard.CreateBoardFromGame(_game), new List<GameBoard>()) ??
        //                                               new List<Position>(); // TODO add history

        //                if (possibleMoves.Count == 0)
        //                {
        //                    MakeMove(player, Move.Pass(player.Color));
        //                }
        //                else
        //                {
        //                    Position randomTargetposition = possibleMoves[Randomizer.Next(possibleMoves.Count)];
        //                    Move newMove = Move.PlaceStone(player.Color, randomTargetposition);
        //                    MakeMove(player, newMove);
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("This agent does not provide information on how to handle its illegal move.");
        //            }
        //        }
        //        else
        //        {
        //            // Ok, we're not enforcing rules.
        //            result.Result = MoveResult.Legal;
        //        }
        //    }
        //    else
        //    {
        //        // Ok, server will handle this.
        //        result.Result = MoveResult.Legal;
        //    }
        //}

        ///// <summary>
        ///// Undoes the last move made, regardless of which player made it. This is called whenever the server commands
        ///// us to undo, or whenever the user clicks to locally undo.
        ///// </summary>
        public void Undo()
        {
           
            var latestMove = Controller.GameTree.LastNode;
            if (latestMove == null)
            {
                throw new InvalidOperationException("There are no moves to undo.");
            }
            var previousMove = latestMove.Parent;
            var _game = Controller;
            if (previousMove == null)
            {
                _game.GameTree.GameTreeRoot = null;
                _game.GameTree.LastNode = null;
            }
            else
            {
                previousMove.Branches.RemoveNode(latestMove);
                _game.GameTree.LastNode = previousMove;
            }
            Controller.SwitchTurnPlayer();
            // Order here matters:
            //(this._turnPlayer.Agent as OnlineAgent)?.Undo();
            //_game.NumberOfMovesPlayed--;
            Controller.TurnPlayer.Agent.PleaseMakeAMove();
            // TODO
            Controller.OnBoardMustBeRefreshed();
        }

      
    }
}
