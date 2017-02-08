using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
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
                player.Agent.PlaceHandicapStones += Agent_PlaceHandicapStones;
                player.Agent.Pass += Agent_Pass;
            }
        }

        private void Agent_PlaceHandicapStones(object sender, int count)
        {
            var agent = (sender as IAgent);
            if (agent != null)
            {

                Controller.OnDebuggingMessage("Placing " + count + " handicap stones...");

                Controller.Info.NumberOfHandicapStones = count;

                GameBoard gameBoard = new GameBoard(Controller.Info.BoardSize);

                Controller.Ruleset.StartHandicapPlacementPhase(ref gameBoard, count, HandicapPlacementType.Fixed);

                Controller.NumberOfMoves++;
                Controller.CurrentNode = Controller.GameTree.AddMoveToEnd(Move.NoneMove, gameBoard);
                Controller.TurnPlayer = Controller.Players.White;
                Controller.TurnPlayer.Agent.PleaseMakeAMove();
                Controller.OnDebuggingMessage("Asking " + Controller.TurnPlayer + " to make a move.");
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
                if (this.Controller.IsOnlineGame)
                {
                    result.Result = MoveResult.Legal;
                }
                else
                {
                    GoToPhase(GamePhaseType.LifeDeathDetermination);
                    return;
                }
            }
            else if (result.Result != MoveResult.Legal)
            {
                Controller.OnDebuggingMessage("That move was illegal: " + result.Result);
                switch (player.Agent.IllegalMoveHandling)
                {
                    case IllegalMoveHandling.InformAgent:
                        player.Agent.MoveIllegal(result.Result);
                        break;
                    case IllegalMoveHandling.PassInstead:
                        Controller.OnDebuggingMessage("Passing instead.");
                        TryToMakeMove(Move.Pass(move.WhoMoves));
                        break;
                    case IllegalMoveHandling.PermitItAnyway:
                        Controller.OnDebuggingMessage("Permitting it anyway.");
                        result.Result = MoveResult.Legal;
                        break;
                }
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
            if (!Controller.IsOnlineGame && player.Clock.IsViolating())
            {
                ClockOut(player);
                return;
            }

            // The move stands, let's make the other player move now.
            if (Controller.TurnPlayer.IsHuman)
            {
                Controller.Server?.Commands.MakeMove(Controller.RemoteInfo, move);
            }
            Controller.OnDebuggingMessage(Controller.TurnPlayer + " moves: " + move);
            Controller.NumberOfMoves++;
            var newNode = Controller.GameTree.AddMoveToEnd(move, new GameBoard(result.NewBoard));            
            Controller.CurrentNode = newNode;
            Controller.SwitchTurnPlayer();
            Controller.TurnPlayer.Agent.PleaseMakeAMove();
            Controller.OnDebuggingMessage("Asking " + Controller.TurnPlayer + " to make a move.");
        }

        private void ClockOut(GamePlayer player)
        {
            Controller.GoToEnd(GameEndInformation.Timeout(player, this.Controller));
        }

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
