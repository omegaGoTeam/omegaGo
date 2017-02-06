using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online.Kgs;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online.Igs
{
    internal class IgsGameController : OnlineGameController
    {
        public IgsGameController(IgsGameInfo gameInfo, IRuleset ruleset, PlayerPair players) : base(gameInfo, ruleset, players)
        {
            var igsServer = game.Metadata.Server;
            igsServer.Events.TimeControlAdjustment += Events_TimeControlAdjustment;

            // Temporary: The following lines will be moved to the common constructor when life/death begins to work
            // for KGS.
            igsServer.IncomingResignation += IgsServer_IncomingResignation;
            igsServer.StoneRemoval += IgsServer_StoneRemoval; // TODO (after refactoring) < move to Life/death
            igsServer.Events.EnterLifeDeath += Events_EnterLifeDeath;
            igsServer.GameScoredAndCompleted += IgsServer_GameScoredAndCompleted;
        }
    }
}
