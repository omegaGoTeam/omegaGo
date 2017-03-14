using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Local
{
    /// <summary>
    /// Creates a local game
    /// </summary>
    public class LocalGameBuilder : GameBuilder<LocalGame, LocalGameBuilder>
    {
        /// <summary>
        /// Builds the local game.
        /// For handicap size 1, free handicap placement is used. 
        /// </summary>
        /// <returns>Built local game</returns>
        public override LocalGame Build()
        {
            GameInfo gameInfo = CreateGameInfo();
            if (gameInfo.NumberOfHandicapStones == 1)
                gameInfo.HandicapPlacementType = Phases.HandicapPlacement.HandicapPlacementType.Free;

            return new LocalGame(gameInfo, CreateRuleset(), CreatePlayers());
        }

        /// <summary>
        /// Local game allows local players only
        /// </summary>
        /// <param name="player">Player to validate</param>
        /// <returns>Is player valid?</returns>
        protected override bool ValidatePlayer(GamePlayer player) =>
            player.Agent.Type != AgentType.Remote;
    }
}