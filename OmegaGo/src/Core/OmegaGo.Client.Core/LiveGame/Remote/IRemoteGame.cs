using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.LiveGame.Remote;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.Modes.LiveGame.Remote
{
    /// <summary>
    /// This is a marker interface used by some UI project classes to determine whether a game is an online game.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Modes.LiveGame.IGame" />
    public interface IRemoteGame : IGame
    {
    }
}
