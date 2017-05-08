using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Remote;

namespace OmegaGo.Core.Online.Common
{
    /// <summary>
    /// Contains events that can be triggered by all online servers. <see cref="RemoteGameController"/> and viewmodels may use these without
    /// requiring specialization.
    /// </summary>
    public interface ICommonEvents
    {
        /// <summary>
        ///     Occurs when the opponent in a GAME asks us to let them undo a move
        /// </summary>
        event EventHandler<GameInfo> UndoRequestReceived;

        /// <summary>
        ///     Occurs when the opponent in a GAME declines our request to undo a move.
        ///     This will also prevent all further undo's in this game.
        /// </summary>
        event EventHandler<GameInfo> UndoDeclined;
    }
}
