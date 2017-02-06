using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Igs
{
    public class IgsGame : RemoteGame
    {
        public IgsGame(IgsGameInfo info, IRuleset ruleset, PlayerPair players, IgsConnection serverConnection) : base(info)
        {
            Info = info;
            Controller = new IgsGameController(Info, ruleset, players, serverConnection);
            foreach (var player in Controller.Players)
            {
                player.AssignToGame(info, Controller);
            }
        }

        public new IgsGameInfo Info { get; }

        public sealed override IGameController Controller { get; }
    }
}
