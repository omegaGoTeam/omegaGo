using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    /// The game controller contains the main loop of a Go game. After constructing it, use <see cref="BeginGame"/> to start the game on
    /// another thread. The game controller will ask both players' agents, in turn, what move do they wish to take.  
    /// </summary>
    //public class ObsoleteGameController : IGameController
    //{
    //    /// <summary>
    //    /// The game that this controller is running.
    //    /// </summary>
    //    private ObsoleteGameInfo _game;
    //    /// <summary>
    //    /// The player who is about to make a move.
    //    /// </summary>
    //    private GamePlayer _turnPlayer;
    //    /// <summary>
    //    /// The game phase we are in. DO NOT set this directly, use <see cref="SetGamePhase(Core.GamePhase)"/> instead. 
    //    /// </summary>
    //    private GamePhase _gamePhase = GamePhase.NotYetBegun;

    //    public GamePhase GamePhase => _gamePhase;
    //    private void SetGamePhase(GamePhase gamePhase)
    //    {
    //        this._gamePhase = gamePhase;
    //        OnEnterPhase(this._gamePhase);
    //    }
    //    /// <summary>
    //    /// Gets the player whose turn it is.
    //    /// </summary>
    //    public GamePlayer TurnPlayer => _turnPlayer;
    //    /// <summary>
    //    /// Gets or sets a value indicating whether the game controller should enforce rules. If true, then illegal moves by agents will be
    //    /// handled according to the agents' handling method. If false, then illegal moves will be accepted.
    //    /// </summary>
    //    // ReSharper disable once MemberCanBePrivate.Global
    //    public bool EnforceRules { get; set; } = true;
    //    private List<Position> _deadPositions = new List<Position>();
    //    public IEnumerable<Position> DeadPositions => _deadPositions;
    //    private List<GamePlayer> _playersDoneWithLifeDeath = new List<GamePlayer>();

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ObsoleteGameController"/> class. This should only be called from within the Game class.
    //    /// </summary>
    //    /// <param name="game">The game that this GameController instance will run.</param>
    //    public ObsoleteGameController(ObsoleteGameInfo game)
    //    {
    //        this._game = game;
    //    }

    //    /// <summary>
    //    /// Occurs when a PLAYER is about to take their turn.
    //    /// </summary>
    //    public event EventHandler<GamePlayer> TurnPlayerChanged;
    //    private void OnTurnPlayerChanged(GamePlayer newTurnPlayer)
    //    {
    //        TurnPlayerChanged?.Invoke(this, newTurnPlayer);
    //    }
    //    /// <summary>
    //    /// Occurs when a DEBUGGING MESSAGE should be printed out to the user in debug mode.
    //    /// </summary>
    //    public event EventHandler<string> DebuggingMessage;
    //    private void OnDebuggingMessage(string logLine)
    //    {
    //        DebuggingMessage?.Invoke(this, logLine);
    //    }
    //    /// <summary>
    //    /// Occurs when the PLAYER resigns. The second argument is the RESIGNATION REASON.
    //    /// </summary>
    //    public event EventHandler<GamePlayer> Resignation;
    //    private void OnResignation(GamePlayer resigner)
    //    {
    //        Resignation?.Invoke(this, resigner);
    //    }
    //    /// <summary>
    //    /// Occurs when the game board should be redrawn by the user interface, probably because a move was made.
    //    /// </summary>
    //    public event EventHandler BoardMustBeRefreshed;
    //    private void OnBoardMustBeRefreshed()
    //    {
    //        BoardMustBeRefreshed?.Invoke(this, EventArgs.Empty);
    //    }
    //    /// <summary>
    //    /// Occurs wheneven the current game phase changes.
    //    /// </summary>
    //    public event EventHandler<GamePhase> EnterPhase;

    //    private void OnEnterPhase(GamePhase newPhase)
    //    {
    //        EnterPhase?.Invoke(this, newPhase);
    //    }
       
    //    public void AbortGame()
    //    {
    //        _gamePhase = GamePhase.Completed;
    //    }


    //    public void RespondRequest()
    //    {
    //        throw new NotImplementedException();
    //    }

    //}

}
