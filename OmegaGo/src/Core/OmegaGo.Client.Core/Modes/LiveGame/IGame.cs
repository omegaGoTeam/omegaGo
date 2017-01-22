using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Chat;

namespace OmegaGo.Core.Modes.LiveGame
{
    public interface IGame
    {


        ObsoleteGameInfo Info { get; }

        IChatService ChatService { get; }

        IGameController Controller { get; }

        event EventHandler<GameTreeNode> BoardChanged;
    }
}
