using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    public class KgsChallengeManagementBundle : KgsNegotiationBundle { 
        public override bool CanDeclineSingleOpponent => true;

        public KgsChallengeManagementBundle(KgsChallenge challenge) : base(challenge)
        {
        }

        public override async Task DeclineSingleOpponent()
        {
            await Connections.Kgs.Commands.DeclineChallengeAsync(Challenge, Challenge.IncomingChallenge);
            Challenge.IncomingChallenge = null;
            ClearOpponentName();
            RefreshStatus();

        }
        public override async Task<IGame> AcceptChallenge(GameCreationViewModel vm)
        {

            await Connections.Kgs.Commands.ChallengeProposalAsync(Challenge, Challenge.IncomingChallenge);
            RefreshStatus();
            return null;
        }
    }
}
