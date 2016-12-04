using OmegaGo.Core.Online.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    public interface IGame
    {
        GameInfo Info { get; }

        IChatService ChatService { get; }

        IGameController Controller { get; }

        event EventHandler<GameTreeNode> BoardChanged;
    }
}
