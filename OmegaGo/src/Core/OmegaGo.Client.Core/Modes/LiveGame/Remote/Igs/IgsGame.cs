using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Igs
{
    /// <summary>
    /// Represents a IGS game
    /// </summary>
    public class IgsGame : RemoteGame<IgsGameInfo, IgsGameController>
    {
        public IgsGame(IgsGameInfo info, IRuleset ruleset, PlayerPair players, IgsConnection serverConnection) : base(info)
        {                 
            Controller = new IgsGameController(Info, ruleset, players, serverConnection);
        }

        /// <summary>
        /// IGS game controller
        /// </summary>
        public override IgsGameController Controller { get; }
    }
}
