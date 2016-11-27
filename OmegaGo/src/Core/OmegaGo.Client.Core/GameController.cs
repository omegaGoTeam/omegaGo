using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.Common;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core
{
    /// <summary>
    /// The game controller contains the main loop of a Go game. After constructing it, use <see cref="BeginGame"/> to start the game on
    /// another thread. The game controller will ask both players' agents, in turn, what move do they wish to take.  
    /// </summary>
    public class GameController
    {
        /// <summary>
        /// The game that this controller is running.
        /// </summary>
        private Game _game;
        /// <summary>
        /// The player who is about to make a move.
        /// </summary>
        private Player _turnPlayer;
        /// <summary>
        /// The game phase we are in.
        /// </summary>
        private GamePhase _gamePhase = GamePhase.NotYetBegun;
        /// <summary>
        /// Gets the player whose turn it is.
        /// </summary>
        public Player TurnPlayer => _turnPlayer;
        /// <summary>
        /// Gets or sets a value indicating whether the game controller should enforce rules. If true, then illegal moves by agents will be
        /// handled according to the agents' handling method. If false, then illegal moves will be accepted.
        /// </summary>
        public bool EnforceRules { get; set; } = true;
        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        /// <param name="game">The game that this GameController instance will run.</param>
        public GameController(Game game)
        {
            this._game = game;
        }

        /// <summary>
        /// Begins the main game loop by asking the first player (who plays black) to make a move, and then the second player, then the first,
        /// and so on until the game concludes. This method will return immediately but it will launch this loop in a Task on another thread.
        /// </summary>
        public void BeginGame()
        {
            SanityCheck();
            LoopDecisionRequest();
        }

        private void SanityCheck()
        {
            if (this._game.Players.Count != 2)
                throw new InvalidOperationException("There must be 2 players in the game.");
            if (this._game.Players.Any(pl => pl.Agent == null))
                throw new InvalidOperationException("Both players must have an Agent to make moves.");
        }

        /// <summary>
        /// Occurs when a PLAYER is about to take their turn.
        /// </summary>
        public event EventHandler<Player> TurnPlayerChanged;
        private void OnTurnPlayerChanged(Player newTurnPlayer)
        {
            TurnPlayerChanged?.Invoke(this, newTurnPlayer);
        }
        /// <summary>
        /// Occurs when a DEBUGGING MESSAGE should be printed out to the user in debug mode.
        /// </summary>
        public event Action<string> DebuggingMessage;
        private void OnDebuggingMessage(string logLine)
        {
            DebuggingMessage?.Invoke(logLine);
        }
        /// <summary>
        /// Occurs when the PLAYER resigns. The second argument is the RESIGNATION REASON.
        /// </summary>
        public event Action<Player, string> Resignation;
        private void OnResignation(Player resigner, string reason)
        {
            Resignation?.Invoke(resigner, reason);
        }
        /// <summary>
        /// Occurs when the game board should be redrawn by the user interface, probably because a move was made.
        /// </summary>
        public event Action BoardMustBeRefreshed;
        private void OnBoardMustBeRefreshed()
        {
            BoardMustBeRefreshed?.Invoke();
        }

        private List<Position> _deadPositions = new List<Position>();
        public event EventHandler<GamePhase> EnterPhase;
        private void OnEnterPhase(GamePhase newPhase)
        {
            EnterPhase?.Invoke(this, newPhase);
        }
        /// <summary>
        /// This is the primary game loop.
        /// </summary>
        private async void LoopDecisionRequest()
        {
            // Begin
            _turnPlayer = _game.Players[0];
            _game.NumberOfMovesPlayed = 0;

            // Main phase
            _gamePhase = GamePhase.MainPhase;
            while (_gamePhase == GamePhase.MainPhase)
            {
                OnTurnPlayerChanged(TurnPlayer);
                OnDebuggingMessage("Asking " + _turnPlayer + " to make a move...");
                AgentDecision decision = await _turnPlayer.Agent.RequestMoveAsync(_game);
                OnDebuggingMessage(_turnPlayer + " does: " + decision);

                if (decision.Kind == AgentDecisionKind.Resign)
                {
                    _gamePhase = GamePhase.Completed;
                    OnResignation(_turnPlayer, decision.Explanation);
                    OnDebuggingMessage("Game is over by resignation.");
                    break;
                }
                Debug.Assert(decision.Kind == AgentDecisionKind.Move);

                MoveProcessingResult result =
                    _game.Ruleset.ProcessMove(FastBoard.CreateBoardFromGame(_game),
                        decision.Move,
                        new List<StoneColor[,]>());

                bool isTheMoveLegal = result.Result == MoveResult.Legal ||
                                      result.Result == MoveResult.LifeDeathConfirmationPhase;
                if (result.Result == MoveResult.LifeDeathConfirmationPhase)
                {
                    _gamePhase = GamePhase.LifeDeathDetermination;
                    break;
                }
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
                            OnDebuggingMessage("Move is illegal because: " + result.Result);
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
                                    decision = AgentDecision.MakeMove(Move.PlaceStone(actorColor, randomTargetposition),
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
                    decision.Move.Captures.AddRange(result.Captures);

                    _game.GameTree.AddMoveToEnd(decision.Move);

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
                // The move stands, let's make the other player move now.
                _game.NumberOfMovesPlayed++;
                _game.GameTree.GameTreeRoot.GetTimelineView.Last().BoardState
                    = FastBoard.CreateBoardFromGame(_game);
                OnBoardMustBeRefreshed();
                _turnPlayer = _game.OpponentOf(_turnPlayer);
            }
            if (_gamePhase == GamePhase.LifeDeathDetermination)
            {
                OnEnterPhase(_gamePhase);

            }
        }


        public bool MarkGroupDead(Position position)
        {
            OnBoardMustBeRefreshed();
            // TODO IGS
            return true;
        }
        public void DoneWithLifeDeathDetermination(Player player)
        {
            OnBoardMustBeRefreshed();
        }
        public void MakeMove(Player player, Move move)
        {

        }
        public void Resign(Player player)
        {

        }
    }
    /// <summary>
    /// Indicates at which stage of the game the game currently is. Most of the time during gameplay, the game will be in the <see cref="MainPhase"/>. 
    /// </summary>
    public enum GamePhase
    {
        /// <summary>
        /// The game has not yet been started.
        /// </summary>
        NotYetBegun,
        /// <summary>
        /// Black is placing handicap stones on the board.
        /// </summary>
        HandicapPlacement,
        /// <summary>
        /// The main phase: In this phase, players alternately make moves until both players pass.
        /// </summary>
        MainPhase,
        /// <summary>
        /// The Life/Death Determination Phase: 
        /// In this phase, players agree on which stones should be marked dead and which should be marked alive.
        /// </summary>
        LifeDeathDetermination,
        /// <summary>
        /// The game has ended and its score has been calculated.
        /// </summary>
        Completed
    }
}
