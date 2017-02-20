using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Chat;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsChatService : IChatService
    {
        private IgsGame _onlineGame;

        public IgsChatService(IgsGame onlineGame)
        {
            this._onlineGame = onlineGame;
            //TODO Martin : Move handling to IGS Connector
            //this._onlineGame.Controller.Server.IncomingInGameChatMessage += Server_IncomingInGameChatMessage;
        }

        private void Server_IncomingInGameChatMessage(object sender, Tuple<IgsGameInfo, ChatMessage> e)
        {
            if (e.Item1.IgsIndex == _onlineGame.Info.IgsIndex)
            {
                MessageReceived?.Invoke(this, e.Item2);
            }
        }

        public event EventHandler<ChatMessage> MessageReceived;
        public async void SendMessage(string message)
        {
            if (String.IsNullOrWhiteSpace(message)) return; // this step is mandatory or else we might crash
            //TODO Martin: move handling to IGSConnector
            //await this._onlineGame.Info.Server.SayAsync(_onlineGame, message);
        }
    }
}
