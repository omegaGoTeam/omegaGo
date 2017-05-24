using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Main.Igs;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Events;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.Igs
{
    /// <summary>
    /// Connects the IgsConnection to a specific game
    /// </summary>
    internal class IgsConnector : IRemoteConnector, IIgsConnectorServerActions
    {
        private readonly IgsConnection _connnection;
        private readonly IgsGameController _gameController;
        private bool _handicapSet;

        public IgsConnector(IgsGameController igsGameController, IgsConnection connnection)
        {
            _connnection = connnection;
            _gameController = igsGameController;
        }

        // TODO  (future work) (Martin): This can be changed.
        // Here's some comments from Petr:
        /*They are inherited from IGameConnector. Those events are used by UiConnector.

 A cleaner alternative would be to get rid of IGameConnector and of GameController.Connectors
 and instead have the UiConnector and RemoteConnector instance variables inside GameController, 
 since only few events can be triggered by both the server and the client, and subscribe to them separately.

 But that’s not a one-minute refactoring, there’s a possibility of making mistakes during it, 
 and it changes code introduced during the second core refactoring,
 so I thought suppressing warnings would have the same result.*/
#pragma warning disable CS0067

        /// <summary>
        /// Indicates the handicap for the game
        /// </summary>
        public event EventHandler<int> GameHandicapSet;
        public event EventHandler LifeDeathReturnToMainForced;
        public event EventHandler LifeDeathUndoDeathMarksRequested;
        public event EventHandler LifeDeathUndoDeathMarksForced;
        public event EventHandler LifeDeathDoneRequested;
        public event EventHandler LifeDeathDoneForced;
        public event EventHandler<Position> LifeDeathKillGroupRequested;
        public event EventHandler<Position> LifeDeathKillGroupForced;
        public event EventHandler<Position> LifeDeathRevivifyGroupForced;
        public event EventHandler MainUndoRequested;
        public event EventHandler MainUndoForced;


        public event EventHandler<IgsTimeControlAdjustmentEventArgs> TimeControlShouldAdjust;
        public event EventHandler<GameScoreEventArgs> GameScoredAndCompleted;
#pragma warning restore CS0067
        public event EventHandler Disconnected;
        public event EventHandler<GameEndInformation> GameEndedByServer;

        public event EventHandler<ChatMessage> NewChatMessageReceived;
        public event EventHandler<GamePlayer> ServerSaysAPlayerIsDone;
        /// <summary>
        /// Unique identification of the game
        /// </summary>
        public int GameId => _gameController.Info.IgsIndex;

        /// <summary>
        /// Handles incoming move from the server
        /// </summary>
        /// <param name="moveIndex">Index of the move</param>
        /// <param name="move">Move</param>
        public void MoveFromServer(int moveIndex, Move move)
        {
            if (!_handicapSet)
            {
                //there is no handicap for this IGS game
                HandicapFromServer(0);
            }
            var targetPlayer = _gameController.Players[move.WhoMoves];
            var igsAgent = targetPlayer.Agent as IgsAgent;
            igsAgent?.MoveFromServer(moveIndex, move);
        }

        /// <summary>
        /// A undo operation is coming from the server
        /// </summary>
        public void UndoFromServer()
        {
            (_gameController.Phase as IgsMainPhase)?.Undo(1);
        }

        /// <summary>
        /// Sets the game's handicap
        /// </summary>
        /// <param name="stoneCount">Number of handicap stones</param>
        public void HandicapFromServer(int stoneCount)
        {
            // TODO  (future work) Petr: Can Handicap info arrive before HandicapPlacement starts?
            // Quite possibly. Sigh. I'll try to do something about it.
            GameHandicapSet?.Invoke(this, stoneCount);
            _handicapSet = true;
        }

        /// <summary>
        /// Informs the connection about a performed move
        /// </summary>
        /// <param name="move">Move that was performed</param>
        public void MovePerformed(Move move)
        {
            //ignore IGS-based moves
            if (_gameController.Players[move.WhoMoves].Agent is IgsAgent) return;
            //inform the connection
            _connnection.Commands.MakeMove(_gameController.Info, move);
        }

        /// <summary>
        /// Receives and handles resignation from server
        /// </summary>
        /// <param name="resigningPlayerColor">Color of the resigning player</param>
        public void ResignationFromServer(StoneColor resigningPlayerColor)
        {
            var player = _gameController.Players[resigningPlayerColor];
            var igsAgent = player.Agent as IgsAgent;
            igsAgent?.ResignationFromServer();
        }

        /// <summary>
        /// New chat message received from server
        /// </summary>
        /// <param name="chatMessage"></param>
        public void ChatMessageFromServer( ChatMessage chatMessage )
        {
            OnNewChatMessageReceived(chatMessage);
        }

        /// <summary>
        /// Sends a chat message
        /// </summary>
        /// <param name="chatMessage">Chat message to send</param>        
        public async Task SendChatMessageAsync( string chatMessage )
        {
            //IGS has two different chat message forms - say and kibitz, depending on whether you are a player or an obersver
            if (_gameController.Players.Local != null)
            {
                //say 
                if (await _connnection.Commands.SayAsync( _gameController.Info, chatMessage ))
                {
                    OnNewChatMessageReceived(new ChatMessage(_connnection.Username, chatMessage, DateTimeOffset.Now,
                        ChatMessageKind.Outgoing));
                }
                else
                {
                    // Not localized, but that's fine.
                    OnNewChatMessageReceived(new ChatMessage("SYSTEM", "Message failed to send: '" + chatMessage + "'. You cannot send messages when the game is over.", DateTimeOffset.Now,
                        ChatMessageKind.Outgoing));
                }
            }
            else
            {
                //kibitz
                if (await _connnection.Commands.KibitzAsync(_gameController.Info, chatMessage))
                {
                    OnNewChatMessageReceived(new ChatMessage(_connnection.Username, chatMessage, DateTimeOffset.Now,
                        ChatMessageKind.Outgoing));
                }
                else
                {
                    // Not localized, but that's fine.
                    OnNewChatMessageReceived(new ChatMessage("SYSTEM", "Message failed to send: '" + chatMessage + "'. You cannot send messages when the game is over.", DateTimeOffset.Now,
                        ChatMessageKind.Outgoing));
                }
            }
        }

        /// <summary>
        /// Server indicates that it wants to change the game phase
        /// </summary>
        /// <param name="gamePhase">Game phase type to start</param>
        public void SetPhaseFromServer(GamePhaseType gamePhase)
        {
            _gameController.SetPhase(gamePhase);
        }

        public void TimeControlAdjustment(IgsTimeControlAdjustmentEventArgs igsTimeControlAdjustmentEventArgs)
        {
            TimeControlShouldAdjust?.Invoke(this, igsTimeControlAdjustmentEventArgs);
        }

        public void ForceLifeDeathKillGroup(Position deadPosition)
        {
            LifeDeathKillGroupForced?.Invoke(this, deadPosition);
        }

        public void ForceLifeDeathUndoDeathMarks()
        {
            LifeDeathUndoDeathMarksForced?.Invoke(this, EventArgs.Empty);
        }

        public void ForceMainUndo()
        {
            MainUndoForced?.Invoke(this, EventArgs.Empty);
        }

        public void Disconnect()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
        public void EndTheGame(GameEndInformation gameEndInfo)
        {
            GameEndedByServer?.Invoke(this, gameEndInfo);
            _connnection.DestroyGame(_gameController.Info);
        }

        private void OnNewChatMessageReceived(ChatMessage e)
        {
            NewChatMessageReceived?.Invoke(this, e);
        }

        public void RaiseServerSaidDone(GamePlayer player)
        {
            ServerSaysAPlayerIsDone?.Invoke(this, player);
        }
    }
}
