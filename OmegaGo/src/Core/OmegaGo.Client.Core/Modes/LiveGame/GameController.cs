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
using OmegaGo.Core.Modes.LiveGame.Phases;

namespace OmegaGo.Core
{
    /// <summary>
    /// The game controller contains the main loop of a Go game. After constructing it, use <see cref="BeginGame"/> to start the game on
    /// another thread. The game controller will ask both players' agents, in turn, what move do they wish to take.  
    /// </summary>
    public class GameController : IGameController
    {
        /// <summary>
        /// The game that this controller is running.
        /// </summary>
        private ObsoleteGameInfo _game;
        /// <summary>
        /// The player who is about to make a move.
        /// </summary>
        private Player _turnPlayer;
        /// <summary>
        /// The game phase we are in. DO NOT set this directly, use <see cref="SetGamePhase(Core.GamePhase)"/> instead. 
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
        // ReSharper disable once MemberCanBePrivate.Global
        public bool EnforceRules { get; set; } = true;
        private List<Position> _deadPositions = new List<Position>();
        public IEnumerable<Position> DeadPositions => _deadPositions;
        private List<Player> _playersDoneWithLifeDeath = new List<Player>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class. This should only be called from within the Game class.
        /// </summary>
        /// <param name="game">The game that this GameController instance will run.</param>
        public GameController(ObsoleteGameInfo game)
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
            if (GamePhase == GamePhase.Completed) return;
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
            OnDebuggingMessage(player + " has completed his part of the Life/Death determination phase.");
            if (!_playersDoneWithLifeDeath.Contains(player))
            {
                _playersDoneWithLifeDeath.Add(player);
            }
            // TODO maybe infinite recursion here?
            this._game.Server?.LifeDeath_Done(this._game);
            if (_playersDoneWithLifeDeath.Count == 2 && this._game.Server == null)
            {
                SetGamePhase(GamePhase.Completed);
            }
            OnBoardMustBeRefreshed();
        }
        public void MakeMove(Player player, Move move)
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

        public void Resign(Player player)
        {
            OnResignation(player);
            this._game.Server?.Resign(this._game);
            _turnPlayer = null;
            SetGamePhase(GamePhases.Finished);
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

        public void AbortGame()
        {
            _gamePhase = GamePhase.Completed;
        }


        public void RespondRequest()
        {
            throw new NotImplementedException();
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

        public void MainPhase_EnterLifeDeath()
        {
            SetGamePhase(GamePhase.LifeDeathDetermination);
        }

        public void EndGame()
        {
            SetGamePhase(GamePhase.Completed);
        }

        /// <summary>
        /// Called by the IGS connection, this method places fixed handicap stones on the board as a single node in the tree, and
        /// advances the timeline forward. In many respects, this acts as the <see cref="MakeMove(Player, Move)"/> method, except
        /// that it places multiple stones. 
        /// </summary>
        /// <param name="handicapStones">The number of handicap stones to place.</param>
        /// <exception cref="InvalidOperationException">Handicap stones can't be placed in the middle of a game.</exception>
        public void HandicapPhase_PlaceIgsHandicap(int handicapStones)
        {
            if (_game.NumberOfMovesPlayed != 0)
                throw new InvalidOperationException("Handicap stones can't be placed in the middle of a game.");

            OnDebuggingMessage("Placing " + handicapStones + " handicap stones...");

            _game.NumberOfHandicapStones = handicapStones;
            GameBoard gameBoard = new GameBoard(_game.BoardSize);
            _game.Ruleset.StartHandicapPlacementPhase(ref gameBoard, handicapStones, HandicapPositions.Type.Fixed);
            _game.GameTree.AddMoveToEnd(Move.NoneMove, gameBoard);
            _turnPlayer = _game.White;
            OnTurnPlayerChanged(_turnPlayer);
            OnDebuggingMessage("Asking " + _turnPlayer + " to make a move after handicap placement.");
            _game.NumberOfMovesPlayed++;
            _turnPlayer.Agent.PleaseMakeAMove();
        }
    }

}
