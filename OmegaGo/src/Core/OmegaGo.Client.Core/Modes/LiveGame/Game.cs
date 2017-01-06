using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Chat;

namespace OmegaGo.Core
{
    public sealed class ObsoleteGame : IGame
    {
        private IChatService _chatService;
        private IGameController _controller;
        private ObsoleteGameInfo _info;

        public IChatService ChatService => _chatService;

        public IGameController Controller => _controller;

        public ObsoleteGameInfo Info => _info;

        public event EventHandler<GameTreeNode> BoardChanged;

        public ObsoleteGame(ObsoleteGameInfo info, IGameController controller, IChatService chatService)
        {
            _info = info;
            _controller = controller;
            _chatService = chatService;

            _controller.BoardMustBeRefreshed += (s, e) => BoardChanged?.Invoke(this, _info.GameTree.LastNode);
        }
    }
}
