using System;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Igs
{
    public class IgsGameBuilder : GameBuilder<IgsGame, IgsGameBuilder>
    {
        private readonly IgsGameInfo _info;

        private IgsConnection _connection;

        public IgsGameBuilder(IgsGameInfo info)
        {
            _info = info;
            this.BoardSize(info.BoardSize);
            this.CountingType(Rules.CountingType.Territory);
            this.HandicapPlacementType(Phases.HandicapPlacement.HandicapPlacementType.Fixed);
            this.Komi(info.Komi);
            this.Handicap(info.NumberOfHandicapStones);
        }

        public IgsGameBuilder Connection(IgsConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            _connection = connection;
            return this;
        }

        public override IgsGame Build() =>
            new IgsGame(_info, CreateRuleset(), CreatePlayers(), _connection);

        /// <summary>
        /// Alows local and IGS players
        /// </summary>
        /// <param name="player">Player to validate</param>
        protected override bool ValidatePlayer(GamePlayer player) =>
            player.Agent.Type != AgentType.Remote || player.Agent is IgsAgent;

        public IgsGameBuilder Name(string gameName)
        {
            _info.GameName = gameName;
            return this;
        }
    }
}
