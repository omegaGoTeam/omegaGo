﻿using System;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Kgs
{
    public class KgsGameBuilder : GameBuilder<KgsGame, KgsGameBuilder>
    {
        private readonly KgsGameInfo _info;

        /// <summary>
        /// Connection to KGSc
        /// </summary>
        private KgsConnection _connection = null;

        public KgsGameBuilder(KgsGameInfo info)
        {
            this._info = info;
            this.BoardSize(info.BoardSize);
            this.CountingType(Rules.CountingType.Territory);
            this.HandicapPlacementType(Phases.HandicapPlacement.HandicapPlacementType.Fixed);
            this.Komi(info.Komi);
            this.Handicap(info.NumberOfHandicapStones);
        }

        public KgsGameBuilder Connection(KgsConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            _connection = connection;
            return this;
        }

        /// <summary>
        /// Builds a KGS game
        /// </summary>
        /// <returns></returns>
        public override KgsGame Build() =>
            new KgsGame(_info, CreateRuleset(), CreatePlayers(), _connection);

        protected override bool ValidatePlayer(GamePlayer player) =>
            player.Agent.Type != AgentType.Remote || player.Agent is KgsAgent;
    }
}
