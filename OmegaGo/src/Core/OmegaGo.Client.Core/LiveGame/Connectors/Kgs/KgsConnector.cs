using System;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Connectors;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Kgs;

namespace OmegaGo.Core.LiveGame.Connectors.Kgs
{
    internal class KgsConnector : IRemoteConnector
    {
        private KgsGameController _gameController;
        private KgsConnection _connection;

        public KgsConnector(KgsGameController gameController, KgsConnection connection)
        {
            _gameController = gameController;
            _connection = connection;
        }

#pragma warning disable CS0067
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
#pragma warning restore CS0067
        public event EventHandler<GameEndInformation> GameEndedByServer;

        public event EventHandler<ChatMessage> NewChatMessageReceived;
        public event EventHandler<GamePlayer> ServerSaysAPlayerIsDone;


        /// <summary>
        /// New chat message received from server
        /// </summary>
        /// <param name="chatMessage"></param>
        public void ChatMessageFromServer(ChatMessage chatMessage)
        {
            OnNewChatMessageReceived(chatMessage);
        }

        public async void MovePerformed(Move move)
        {
            if (_gameController.Players[move.WhoMoves].Agent is KgsAgent) return;
            await _connection.Commands.MakeMove(_gameController.Info, move);
        }

        public void EndTheGame(GameEndInformation gameEndInfo)
        {
            GameEndedByServer?.Invoke(this, gameEndInfo);
        }
        
        /// <summary>
        /// Sends a chat message
        /// </summary>
        /// <param name="chatMessage">Chat message to send</param>        
        public async Task SendChatMessageAsync(string chatMessage)
        {
            await _connection.Commands.ChatAsync(_gameController.Info, chatMessage);
        }

        protected void OnNewChatMessageReceived(ChatMessage e)
        {
            NewChatMessageReceived?.Invoke(this, e);
        }

        public void ForceKillGroup(Position position)
        {
            LifeDeathKillGroupForced?.Invoke(this, position);
        }

        public void ForceRevivifyGroup(Position position)
        {
            LifeDeathRevivifyGroupForced?.Invoke(this, position);
        }

        public void RaiseDoneReceived(GamePlayer gamePlayer)
        {
            ServerSaysAPlayerIsDone?.Invoke(this, gamePlayer);
        }

        public void CauseUndo(int howManyUndos)
        {
            (_gameController.Phase as KgsMainPhase)?.Undo(howManyUndos);
            
        }
    }
}
