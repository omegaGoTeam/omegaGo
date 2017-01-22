using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    class MainPhase : IMainPhase
    {
        public GamePhaseType PhaseType => GamePhaseType.Main;


        private void MainPhase_AskPlayerToMove(GamePlayer turnPlayer)
        {
            if (GamePhase == GamePhase.Completed) return;
            _turnPlayer = turnPlayer;
            OnTurnPlayerChanged(_turnPlayer);
            OnDebuggingMessage("Asking " + _turnPlayer + " to make a move...");
            _turnPlayer.Agent.PleaseMakeAMove();
        }

        public void MakeMove(GamePlayer player, Move move)
        {
            if (_gamePhase == GamePhase.Completed) return;
            if (_gamePhase != GamePhase.MainPhase)
                throw new InvalidOperationException("Moves can only be made during main phase.");
            if (player != TurnPlayer)
                throw new InvalidOperationException("It is not your turn.");
            OnDebuggingMessage(_turnPlayer + " moves: " + move);

            MoveProcessingResult result =
                   _game.Ruleset.ProcessMove(
                       _game.GameTree.LastNode?.BoardState ?? new GameBoard(_game.BoardSize),
                       move,
                       _game.GameTree.GameTreeRoot?.GetTimelineView.Select(node => node.BoardState).ToList() ?? new List<GameBoard>()); // TODO history

            if (result.Result == MoveResult.LifeDeathDeterminationPhase)
            {
                if (this._game.Server != null)
                {
                    result.Result = MoveResult.Legal;
                    // In server games, we let the server decide on life/death determination, not our own ruleset.
                }
                else
                {
                    SetGamePhase(GamePhase.LifeDeathDetermination);
                    _turnPlayer = null;
                    return;
                }
            }
            if (result.Result != MoveResult.Legal)
            {
                HandleIllegalMove(player, ref result);
                if (result.Result != MoveResult.Legal)
                {
                    // Still illegal.
                    return;
                }
            }
            if (move.Kind == MoveKind.PlaceStone)
            {
                OnDebuggingMessage("Adding " + move + " to primary timeline.");
                move.Captures.AddRange(result.Captures);
            }
            else if (move.Kind == MoveKind.Pass)
            {
                OnDebuggingMessage(_turnPlayer + " passed!");
            }
            else
            {
                throw new InvalidOperationException("An agent should not use any other move kinds except for placing stones and passing.");
            }
            // The move stands, let's make the other player move now.
            _game.NumberOfMovesPlayed++;
            _game.GameTree.AddMoveToEnd(move, new GameBoard(result.NewBoard));
            if (_game.Server != null && !(_turnPlayer.Agent is OnlineAgent))
            {
                _game.Server.MakeMove(_game, move);
            }
            OnBoardMustBeRefreshed();
            MainPhase_AskPlayerToMove(_game.OpponentOf(player));
        }

        private void HandleIllegalMove(GamePlayer player, ref MoveProcessingResult result)
        {
            if (player.Agent.HowToHandleIllegalMove == IllegalMoveHandling.PermitItAnyway)
            {
                OnDebuggingMessage("The agent asked us to make an ILLEGAL MOVE and we are DOING IT ANYWAY!");
                result.Result = MoveResult.Legal;
                return;
            }
            if (_game.Server == null) // In server games, we always permit all moves and leave the verification on the server.
            {
                if (this.EnforceRules)
                {
                    // Move is forbidden.
                    OnDebuggingMessage("Move is illegal because: " + result.Result);
                    if (_turnPlayer.Agent.HowToHandleIllegalMove == IllegalMoveHandling.Retry)
                    {
                        OnDebuggingMessage("Illegal move - retrying.");
                        _turnPlayer.Agent.PleaseMakeAMove();
                    }
                    else if (_turnPlayer.Agent.HowToHandleIllegalMove == IllegalMoveHandling.MakeRandomMove)
                    {

                        OnDebuggingMessage("Illegal move - making a random move instead.");
                        List<Position> possibleMoves = _game.Ruleset?.GetAllLegalMoves(player.Color,
                            FastBoard.CreateBoardFromGame(_game), new List<GameBoard>()) ??
                                                       new List<Position>(); // TODO add history

                        if (possibleMoves.Count == 0)
                        {
                            MakeMove(player, Move.Pass(player.Color));
                        }
                        else
                        {
                            Position randomTargetposition = possibleMoves[Randomness.Next(possibleMoves.Count)];
                            Move newMove = Move.PlaceStone(player.Color, randomTargetposition);
                            MakeMove(player, newMove);
                        }
                    }
                    else
                    {
                        throw new Exception("This agent does not provide information on how to handle its illegal move.");
                    }
                }
                else
                {
                    // Ok, we're not enforcing rules.
                    result.Result = MoveResult.Legal;
                }
            }
            else
            {
                // Ok, server will handle this.
                result.Result = MoveResult.Legal;
            }
        }

        /// <summary>
        /// Undoes the last move made, regardless of which player made it. This is called whenever the server commands
        /// us to undo, or whenever the user clicks to locally undo.
        /// </summary>
        public void MainPhase_Undo()
        {
            if (this.GamePhase != GamePhase.MainPhase)
                throw new InvalidOperationException("We are not in the main phase.");
            var latestMove = _game.GameTree.LastNode;
            if (latestMove == null)
            {
                throw new InvalidOperationException("There are no moves to undo.");
            }
            var previousMove = latestMove.Parent;
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
            _turnPlayer = _game.OpponentOf(_turnPlayer);
            OnTurnPlayerChanged(_turnPlayer);
            // Order here matters:
            (this._turnPlayer.Agent as OnlineAgent)?.Undo();
            _game.NumberOfMovesPlayed--;
            _turnPlayer.Agent.PleaseMakeAMove();
            OnBoardMustBeRefreshed();
        }

        public void Resign(GamePlayer player)
        {
            OnResignation(player);
            this._game.Server?.Resign(this._game);
            _turnPlayer = null;
            SetGamePhase(GamePhaseType.Finished);
        }
    }
}
