using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online.Igs
{
    public class IgsGame : RemoteGame
    {
        public IgsGame(IgsGameInfo info, IRuleset ruleset, PlayerPair players) : base(info)
        {
            Info = info;
            Controller = new IgsGameController(Info, ruleset, players);
            foreach(var player in Controller.Players)
            {
                player.AssignToGame(info, Controller);
            }
        }

        public new IgsGameInfo Info { get; }

        public sealed override IGameController Controller { get; }
    }
}
