using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Connectors;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Kgs;

namespace OmegaGo.Core.LiveGame.Connectors.Kgs
{
    internal class KgsConnector : IRemoteConnector
    {
        private KgsGameController kgsGameController;
        private KgsConnection serverConnection;

        public KgsConnector(KgsGameController kgsGameController, KgsConnection serverConnection)
        {
            this.kgsGameController = kgsGameController;
            this.serverConnection = serverConnection;
        }

#pragma warning disable CS0067
        public event EventHandler LifeDeathReturnToMainForced;
        public event EventHandler LifeDeathUndoDeathMarksRequested;
        public event EventHandler LifeDeathUndoDeathMarksForced;
        public event EventHandler LifeDeathDoneRequested;
        public event EventHandler LifeDeathDoneForced;
        public event EventHandler<Position> LifeDeathKillGroupRequested;
        public event EventHandler<Position> LifeDeathKillGroupForced;
        public event EventHandler MainUndoRequested;
        public event EventHandler MainUndoForced;
#pragma warning restore CS0067
        public event EventHandler<GameEndInformation> GameEndedByServer;

        public async void MovePerformed(Move move)
        {
            if (kgsGameController.Players[move.WhoMoves].Agent is KgsAgent) return;
            await this.serverConnection.Commands.MakeMove(this.kgsGameController.Info, move);
        }

        public void EndTheGame(GameEndInformation gameEndInfo)
        {
            GameEndedByServer?.Invoke(this, gameEndInfo);
        }
    }
}
