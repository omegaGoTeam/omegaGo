using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online.Igs
{
    public class IgsGame : RemoteGame
    {
        public IgsGame(IgsGameInfo info, IRuleset ruleset, PlayerPair players) : base(info)
        {
            Metadata = info;
            Controller = new GameController(this, ruleset, players);
            foreach(var player in Controller.Players)
            {
                player.AssignToGame(info, Controller);
            }
        }

        public IgsGameInfo Metadata { get; }

        public override IGameController Controller { get; }
    }
}
