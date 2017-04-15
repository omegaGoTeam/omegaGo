using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Online.Common
{
    public interface ICommonEvents
    {
        /// <summary>
        ///     Occurs when the opponent in a GAME asks us to let them undo a move
        /// </summary>
        event EventHandler<GameInfo> UndoRequestReceived;
    }
}
