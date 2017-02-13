using System;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Kgs
{
    public class KgsGameBuilder : GameBuilder<KgsGame, KgsGameBuilder>
    {
        private readonly KgsGameInfo _info;

        public KgsGameBuilder(KgsGameInfo info)
        {
            this._info = info;
            this.BoardSize(info.BoardSize);
            this.CountingType(Rules.CountingType.Territory);
            this.HandicapPlacementType(Phases.HandicapPlacement.HandicapPlacementType.Fixed);
            this.Komi(info.Komi);
            this.Handicap(info.NumberOfHandicapStones);
        }

        public override KgsGame Build() =>
            new KgsGame(_info, CreateRuleset(), CreatePlayers());

        protected override bool ValidatePlayer(GamePlayer player) =>
            player.Agent.Type != AgentType.Remote || player.Agent is KgsAgent;
    }
}
