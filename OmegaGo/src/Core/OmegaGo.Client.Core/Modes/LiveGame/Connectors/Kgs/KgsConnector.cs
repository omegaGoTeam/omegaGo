using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Chat;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.Kgs
{
    public class KgsConnector : IGameConnector
    {
        public void MovePerformed(Move move)
        {
            
        }

        public event EventHandler LifeDeathForceReturnToMain;
        public event EventHandler LifeDeathRequestUndoDeathMarks;
        public event EventHandler LifeDeathForceUndoDeathMarks;
        public event EventHandler LifeDeathRequestDone;
        public event EventHandler LifeDeathForceDone;
        public event EventHandler<Position> LifeDeathRequestKillGroup;
        public event EventHandler<Position> LifeDeathForceKillGroup;
        public event EventHandler MainRequestUndo;
        public event EventHandler MainForceUndo;

        public event EventHandler<ChatMessage> ChatMessageReceived;
        
    }
}
