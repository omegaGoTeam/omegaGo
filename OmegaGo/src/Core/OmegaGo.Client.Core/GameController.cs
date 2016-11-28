﻿using System;
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
        /// The game phase we are in. DO NOT set this directly, use <see cref="SetGamePhase(GamePhase)"/> instead. 
        /// </summary>
        private GamePhase _gamePhase = GamePhase.NotYetBegun;

        public GamePhase GamePhase => _gamePhase;
        private void SetGamePhase(GamePhase gamePhase)
        {
            this._gamePhase = gamePhase;
            OnEnterPhase(this._gamePhase);
        }
        /// <summary>
        /// Gets the player whose turn it is.
        /// </summary>
        public Player TurnPlayer => _turnPlayer;
        /// <summary>
        /// Gets or sets a value indicating whether the game controller should enforce rules. If true, then illegal moves by agents will be
        /// handled according to the agents' handling method. If false, then illegal moves will be accepted.
        /// </summary>
        public bool EnforceRules { get; set; } = true;
        private List<Position> _deadPositions = new List<Position>();
        public IEnumerable<Position> DeadPositions => _deadPositions;
        private List<Player> _playersDoneWithLifeDeath = new List<Player>();
        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class. This should only be called from within the Game class.
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
            if (this._game.Players.Count != 2)
                throw new InvalidOperationException("There must be 2 players in the game.");

            foreach(var player in _game.Players)
            {
                if (player.Agent == null)
                    throw new InvalidOperationException("Both players must have an Agent to make moves.");
                player.Agent.GameBegins(player, _game);
            }
            _game.NumberOfMovesPlayed = 0;
            SetGamePhase(GamePhase.MainPhase);
            MainPhase_AskPlayerToMove(_game.Black);
        }

        private void MainPhase_AskPlayerToMove(Player turnPlayer)
        {
            _turnPlayer = turnPlayer;
            OnTurnPlayerChanged(_turnPlayer);
            OnDebuggingMessage("Asking " + _turnPlayer + " to make a move...");
            _turnPlayer.Agent.PleaseMakeAMove();
        }

        public void MarkGroupDead(Position position)
        {
            var board = FastBoard.CreateBoardFromGame(_game);
            if (board[position.X, position.Y] == StoneColor.None)
            {
                return;
            }
            var group = _game.Ruleset.DiscoverGroup(position, board);
            foreach(var deadStone in group)
            {
                if (!this._deadPositions.Contains(deadStone))
                {
                    this._deadPositions.Add(deadStone);
                }
            }
            _playersDoneWithLifeDeath.Clear();
            OnBoardMustBeRefreshed();
        }
        public void LifeDeath_Done(Player player)
        {

            if (!_playersDoneWithLifeDeath.Contains(player))
            {
                _playersDoneWithLifeDeath.Add(player);
            }
            if (_playersDoneWithLifeDeath.Count == 2)
            {
                SetGamePhase(GamePhase.Completed);
            }
            OnBoardMustBeRefreshed();
        }
        public async void MakeMove(Player player, Move move)
        {
            if (_gamePhase != GamePhase.MainPhase)
                throw new InvalidOperationException("Moves can only be made during main phase.");
            if (player != TurnPlayer)
                throw new InvalidOperationException("It is not your turn.");
            OnDebuggingMessage(_turnPlayer + " moves: " + move);

            MoveProcessingResult result =
                   _game.Ruleset.ProcessMove(FastBoard.CreateBoardFromGame(_game), move, new List<StoneColor[,]>()); // TODO history

            if (result.Result == MoveResult.LifeDeathConfirmationPhase)
            {
                SetGamePhase(GamePhase.LifeDeathDetermination);
                _turnPlayer = null;
                return;
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
            _game.GameTree.AddMoveToEnd(move, FastBoard.CreateBoardFromGame(_game));
            if (_game.Server != null && !(_turnPlayer.Agent is OnlineAgent))
            {
                await _game.Server.MakeMove(_game, move);
            }
            OnBoardMustBeRefreshed();
            MainPhase_AskPlayerToMove(_game.OpponentOf(player));
        }

        private void HandleIllegalMove(Player player, ref MoveProcessingResult result)
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
                            FastBoard.CreateBoardFromGame(_game), new List<StoneColor[,]>()) ??
                                                       new List<Position>(); // TODO add history

                        if (possibleMoves.Count == 0)
                        {
                            MakeMove(player, Move.Pass(player.Color));
                            return;
                        }
                        else
                        {
                            Position randomTargetposition = possibleMoves[Randomness.Next(possibleMoves.Count)];
                            Move newMove = Move.PlaceStone(player.Color, randomTargetposition);
                            MakeMove(player, newMove);
                            return;
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

        public void Resign(Player player)
        {
            OnResignation(player);
            _turnPlayer = null;
            SetGamePhase(GamePhase.Completed);
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
        public event EventHandler<string> DebuggingMessage;
        private void OnDebuggingMessage(string logLine)
        {
            DebuggingMessage?.Invoke(this, logLine);
        }
        /// <summary>
        /// Occurs when the PLAYER resigns. The second argument is the RESIGNATION REASON.
        /// </summary>
        public event EventHandler<Player> Resignation;
        private void OnResignation(Player resigner)
        {
            Resignation?.Invoke(this, resigner);
        }
        /// <summary>
        /// Occurs when the game board should be redrawn by the user interface, probably because a move was made.
        /// </summary>
        public event EventHandler BoardMustBeRefreshed;
        private void OnBoardMustBeRefreshed()
        {
            BoardMustBeRefreshed?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Occurs wheneven the current game phase changes.
        /// </summary>
        public event EventHandler<GamePhase> EnterPhase;
        private void OnEnterPhase(GamePhase newPhase)
        {
            EnterPhase?.Invoke(this, newPhase);
        }
        /// <summary>
        /// This is the primary game loop.
        /// </summary>
        public void LifeDeath_UndoPhase()
        {
            this._deadPositions = new List<Position>();
            _playersDoneWithLifeDeath.Clear();
            OnBoardMustBeRefreshed();
        }

        public void LifeDeath_Resume()
        {
            this._deadPositions = new List<Position>();
            SetGamePhase(GamePhase.MainPhase);
            _playersDoneWithLifeDeath.Clear();
            OnBoardMustBeRefreshed();
            MainPhase_AskPlayerToMove(_game.Black);
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
