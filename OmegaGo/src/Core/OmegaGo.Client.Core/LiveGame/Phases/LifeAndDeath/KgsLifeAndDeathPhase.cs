using System.Linq;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    class KgsLifeAndDeathPhase : RemoteLifeAndDeathPhase
    {
        private readonly KgsGameController _controller;

        public KgsLifeAndDeathPhase(KgsGameController controller) : base(controller)
        {
            _controller = controller;
        }

        protected override async Task LifeDeathRequestKillGroup(Position groupMember)
        {
            if (this.DeadPositions.Any(pos => pos == groupMember))
            {
                await this._controller.Server.Commands.LifeDeathMarkLife(groupMember, _controller.Info);
            }
            else
            {
                await this._controller.Server.Commands.LifeDeathMarkDeath(groupMember, _controller.Info);
            }
        }
    }
}
