using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.Common;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core
{
    public class GameController
    {
        private Game _game;
        private Player _turnPlayer;
        /// <summary>
        /// Gets the player whose turn it is.
        /// </summary>
        public Player TurnPlayer => _turnPlayer;
        public bool EnforceRules { get; set; } = true;

        public GameController(Game game)
        {
            this._game = game;
        }

        public void BeginGame()
        {
            _game.NumberOfMovesPlayed = 0;
            LoopDecisionRequest();
        }

        public event Action<string> TurnPlayerChanged;
        private void OnTurnPlayerChanged(string newTurnPlayer)
        {
            TurnPlayerChanged?.Invoke(newTurnPlayer);
        }
        public event Action<string> DebuggingMessage;
        private void OnDebuggingMessage(string logLine)
        {
            DebuggingMessage?.Invoke(logLine);
        }
        public event Action<Player, string> Resignation;
        private void OnResignation(Player resigner, string reason)
        {
            Resignation?.Invoke(resigner, reason);
        }
        public event Action BoardMustBeRefreshed;
        private void OnBoardMustBeRefreshed()
        {
            BoardMustBeRefreshed?.Invoke();
        }
        private async void LoopDecisionRequest()
        {
            _turnPlayer = _game.Players[0];
            while (true)
            {
                OnTurnPlayerChanged(TurnPlayer.Name);
                OnDebuggingMessage("Asking " + _turnPlayer + " to make a move...");
                AgentDecision decision = await _turnPlayer.Agent.RequestMove(_game);
                OnDebuggingMessage(_turnPlayer + " does: " + decision);

                if (decision.Kind == AgentDecisionKind.Resign)
                {
                    OnResignation(_turnPlayer, decision.Explanation);
                    OnDebuggingMessage("Game is over by resignation.");
                    break;
                }
                if (decision.Kind != AgentDecisionKind.Move)
                {
                    throw new Exception("There is no other possible decision.");
                }

                Move moveToMake = decision.Move;
                MoveProcessingResult result =
                        _game.Ruleset.ProcessMove(FastBoard.CreateBoardFromGame(_game),
                        decision.Move,
                        new List<StoneColor[,]>()); // TODO history

                bool isTheMoveLegal = result.Result == MoveResult.Legal ||
                                      result.Result == MoveResult.LifeDeadConfirmationPhase;
                if (!isTheMoveLegal && _turnPlayer.Agent.HowToHandleIllegalMove == IllegalMoveHandling.PermitItAnyway)
                {
                    OnDebuggingMessage("The agent asked us to make an ILLEGAL MOVE and we are DOING IT ANYWAY!");
                    isTheMoveLegal = true;

                }
                if (!isTheMoveLegal)
                {
                    if (_game.Server == null) // In server games, we always permit all moves and leave the verification on the server.
                    {
                        if (this.EnforceRules)
                        {
                            // Move is forbidden.
                            if (_turnPlayer.Agent.HowToHandleIllegalMove == IllegalMoveHandling.Retry)
                            {
                                OnDebuggingMessage("Illegal move - retrying.");
                                continue; // retry
                            }
                            else if (_turnPlayer.Agent.HowToHandleIllegalMove == IllegalMoveHandling.MakeRandomMove)
                            {

                                OnDebuggingMessage("Illegal move - making a random move instead.");
                                StoneColor actorColor = (_turnPlayer == _game.Players[0]) ? StoneColor.Black : StoneColor.White;
                                List<Position> possibleMoves = _game.Ruleset?.GetAllLegalMoves(actorColor,
                                    FastBoard.CreateBoardFromGame(_game), new List<StoneColor[,]>()) ??
                                                               new List<Position>();
                                // TODO add history
                                if (possibleMoves.Count == 0)
                                {
                                    OnDebuggingMessage("NO MORE MOVES!");
                                    // TODO
                                    break;
                                }
                                else
                                {
                                    Position randomTargetposition = possibleMoves[Randomness.Next(possibleMoves.Count)];
                                    decision = AgentDecision.MakeMove(Move.Create(actorColor, randomTargetposition),
                                        "A random move was made because the AI supplied an illegal move.");
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
                        }
                    }
                    else
                    {
                        // Ok, server will handle this.
                    }
                }
                if (decision.Move.Kind == MoveKind.PlaceStone)
                {
                    OnDebuggingMessage("Adding " + decision.Move + " to primary timeline.");
                    _game.GameTree.AddMoveToEnd(decision.Move);

                    decision.Move.Captures.AddRange(result.Captures);

                    if (_game.Server != null && !(_turnPlayer.Agent is OnlineAgent))
                    {
                        await _game.Server.MakeMove(_game, decision.Move);
                    }
                }
                else if (decision.Move.Kind == MoveKind.Pass)
                {
                    OnDebuggingMessage(_turnPlayer + " passed!");
                }
                else
                {
                    throw new InvalidOperationException("An agent should not use any other move kinds except for placing stones and passing.");
                }
                // THE MOVE STANDS
                _game.NumberOfMovesPlayed++;
                OnBoardMustBeRefreshed();
                _turnPlayer = _game.OpponentOf(_turnPlayer);
            }

        }
    }
}
