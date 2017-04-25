using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    public class KgsChallengeManagementBundle : KgsNegotiationBundle { 
        public override bool CanDeclineSingleOpponent => false;

        public KgsChallengeManagementBundle(KgsChallenge challenge) : base(challenge)
        {
        }
    }
}
