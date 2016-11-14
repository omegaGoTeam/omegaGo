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
                /*
                lblTurnPlayer.Text = _turnPlayer.Name;
                */
                OnDebuggingMessage("Asking " + _turnPlayer + " to make a move...");
                /*
                if (_turnPlayer.Agent is AIAgent)
                {
                    ((AIAgent)_turnPlayer.Agent).Strength = (int)this.nAiStrength.Value;
                }
                */
                AgentDecision decision = await _turnPlayer.Agent.RequestMove(_game);
                OnDebuggingMessage(_turnPlayer + " does: " + decision);

                if (decision.Kind == AgentDecisionKind.Resign)
                {
                    OnResignation(_turnPlayer, decision.Explanation);
                    /*
                    panelEnd.Visible = true;
                    lblEndCaption.Text = _turnPlayer + " resigned!";
                    lblGameEndReason.Text = "The player resignation reason: '" + decision.Explanation + "'";
                    */
                    OnDebuggingMessage("Game is over by resignation.");
                    break;
                }
                if (decision.Kind != AgentDecisionKind.Move)
                {
                    throw new Exception("There is no other possible decision.");
                }

                    Move moveToMake = decision.Move;
                    bool willWeAcceptTheMove = true;
                    if (moveToMake.Kind == MoveKind.PlaceStone)
                    {
                        if (moveToMake.Coordinates.X < 0 || moveToMake.Coordinates.Y < 0 ||
                            moveToMake.Coordinates.X >= _game.BoardSize.Width || moveToMake.Coordinates.Y >= _game.BoardSize.Height)
                        {
                            OnDebuggingMessage("Illegal Move - Outside the board");
                            willWeAcceptTheMove = false;
                        }
                    }
                    if (willWeAcceptTheMove)
                    {
                        // TODO So far, we're not providing Ko information
                        MoveResult canWeMakeIt =
                            _game.Ruleset?.IsLegalMove(
                                FastBoard.CreateBoardFromGame(_game), 
                                moveToMake,
                                new List<StoneColor[,]>()
                                ) ?? MoveResult.Legal;

                        // If there is no ruleset, moves are automatically legal.
                        if (canWeMakeIt != MoveResult.Legal && canWeMakeIt != MoveResult.LifeDeadConfirmationPhase)
                        {
                            willWeAcceptTheMove = false;
                            switch (canWeMakeIt)
                            {
                                case MoveResult.Ko:
                                OnDebuggingMessage("Illegal Move - Ko");
                                    break;
                                case MoveResult.OccupiedPosition:
                                OnDebuggingMessage("That intersection is already occupied!");
                                    break;
                                case MoveResult.SelfCapture:
                                OnDebuggingMessage("Illegal Move - Suicide");
                                    break;
                                case MoveResult.SuperKo:
                                OnDebuggingMessage("Illegal Move - Superko");
                                    break;
                            }
                        }
                    }
                    if (!willWeAcceptTheMove && _turnPlayer.Agent.HowToHandleIllegalMove == IllegalMoveHandling.PermitItAnyway)
                    {
                        OnDebuggingMessage("The agent asked us to make an ILLEGAL MOVE and we are DOING IT ANYWAY!");
                        willWeAcceptTheMove = true;
                    }
                    if (!willWeAcceptTheMove)
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
                                // Server overrides rules.
                            }
                        }
                        else
                        {
                            // Ok.
                        }
                    }
                    if (decision.Move.Kind == MoveKind.PlaceStone)
                    {
                        OnDebuggingMessage("Adding " + decision.Move + " to primary timeline.");
                        _game.GameTree.AddMoveToEnd(decision.Move);
                        if (_game.Server != null && !(_turnPlayer.Agent is OnlineAgent))
                        {
                            _game.Server.MakeMove(_game, decision.Move);
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
