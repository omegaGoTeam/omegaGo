using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    internal class IgsIncomingMatchRequestBundle : IgsBundle
    {
        private readonly IgsMatchRequest _igsMatchRequest;

        public IgsIncomingMatchRequestBundle(IgsMatchRequest igsMatchRequest)
        {
            _igsMatchRequest = igsMatchRequest;
        }

        public override GameCreationFormStyle Style => GameCreationFormStyle.IncomingIgs;

        public override bool AcceptableAndRefusable => true;

        public override bool WillCreateChallenge => false;


        public override bool Frozen => true;

        public override string OpponentName => _igsMatchRequest.OpponentName;

        public override async Task RefuseChallenge(GameCreationViewModel gameCreationViewModel)
        {
            await Connections.Igs.Commands.DeclineMatchRequestAsync(_igsMatchRequest);
        }
        public override async Task<IGame> AcceptChallenge(GameCreationViewModel gameCreationViewModel)
        {
            IgsGame game = await Connections.Igs.Commands.AcceptMatchRequestAsync(_igsMatchRequest);
            return game;
        }
        public override void OnLoad(GameCreationViewModel gameCreationViewModel)
        {
            base.OnLoad(gameCreationViewModel);

            gameCreationViewModel.FormTitle = Localizer.Creation_IncomingIgsChallenge;
            gameCreationViewModel.RefusalCaption = Localizer.RefuseChallenge;

        }
    }
}