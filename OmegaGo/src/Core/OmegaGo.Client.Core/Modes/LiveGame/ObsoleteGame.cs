using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Chat;

namespace OmegaGo.Core.Modes.LiveGame
{
    //public sealed class ObsoleteGame : IObsoleteGame
    //{
    //    private IChatService _chatService;
    //    private IGameController _controller;
    //    private ObsoleteGameInfo _info;

    //    public IChatService ChatService => _chatService;

    //    public IGameController Controller => _controller;

    //    public ObsoleteGameInfo Info => _info;

    //    public event EventHandler<GameTreeNode> BoardChanged;

    //    public ObsoleteGame(ObsoleteGameInfo info, IGameController controller, IChatService chatService)
    //    {
    //        _info = info;
    //        _controller = controller;
    //        _chatService = chatService;

    //        _controller.BoardMustBeRefreshed += (s, e) => BoardChanged?.Invoke(this, _info.GameTree.LastNode);
    //    }
    //}
}
