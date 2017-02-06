using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    internal class OnlineGameController : GameController
    {
        public OnlineGameController(GameInfo gameInfo, IRuleset ruleset, PlayerPair players, IServerConnection serverConnection) : base(gameInfo, ruleset, players)
        {            
            RemoteInfo = game.RemoteInfo;
            Ruleset = ruleset;
            Players = players;
            Server = serverConnection;
            GameTree = new GameTree(ruleset);
        }

        /// <summary>
        /// Gets the server connection, or null if this is not an online game.
        /// </summary>
        public IServerConnection Server { get; }
    }
}
