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
    public class KgsJoinChallengeBundle : KgsNegotiationBundle
    {
        public override bool CanDeclineSingleOpponent => false;
        public override string TabTitle => OpponentName + " (KGS)";

        public KgsJoinChallengeBundle(KgsChallenge challenge) : base(challenge)
        {
        }

        public override async Task<IGame> AcceptChallenge(GameCreationViewModel gameCreationViewModel)
        {
            await Connections.Kgs.Commands.AcceptChallenge(Challenge);
            return null;
        }
    }
}
