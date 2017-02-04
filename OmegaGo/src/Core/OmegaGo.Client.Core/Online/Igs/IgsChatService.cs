using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Online.Chat;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsChatService : IChatService
    {
        private OnlineGame _onlineGame;

        public IgsChatService(OnlineGame onlineGame)
        {
            this._onlineGame = onlineGame;
            this._onlineGame.Metadata.Server.IncomingInGameChatMessage += Server_IncomingInGameChatMessage;
        }

        private void Server_IncomingInGameChatMessage(object sender, Tuple<OnlineGameInfo, ChatMessage> e)
        {
            if (e.Item1.IgsIndex == _onlineGame.Metadata.IgsIndex)
            {
                MessageReceived?.Invoke(this, e.Item2);
            }
        }

        public event EventHandler<ChatMessage> MessageReceived;
        public async void SendMessage(string message)
        {
            if (String.IsNullOrWhiteSpace(message)) return; // this step is mandatory or else we might crash
            await this._onlineGame.Metadata.Server.SayAsync(_onlineGame, message);
        }
    }
}
