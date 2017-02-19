using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;

namespace OmegaGo.Core.Modes.LiveGame.Local
{
    /// <summary>
    /// Creates a local game
    /// </summary>
    public class LocalGameBuilder : GameBuilder<LocalGame, LocalGameBuilder>
    {
        /// <summary>
        /// Builds the local game
        /// </summary>
        /// <returns></returns>
        public override LocalGame Build() =>
            new LocalGame(CreateGameInfo(), CreateRuleset(), CreatePlayers());

        /// <summary>
        /// Local game allows local players only
        /// </summary>
        /// <param name="player">Player to validate</param>
        /// <returns>Is player valid?</returns>
        protected override bool ValidatePlayer(GamePlayer player) =>
            player.Agent.Type != AgentType.Remote;
    }
}