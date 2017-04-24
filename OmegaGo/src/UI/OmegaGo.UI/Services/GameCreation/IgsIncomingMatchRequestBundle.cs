using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.Services.Audio;
using System;

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
        public override string TabTitle => _igsMatchRequest.OpponentName + " (IGS)";


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
            gameCreationViewModel.CustomSquareSize = _igsMatchRequest.BoardSize.ToString();
            gameCreationViewModel.SelectedColor = _igsMatchRequest.YourColor;
            gameCreationViewModel.TimeControl.Style = Core.Time.TimeControlStyle.Canadian;
            gameCreationViewModel.TimeControl.OvertimeMinutes = _igsMatchRequest.OvertimeMinutes.ToString();
            gameCreationViewModel.TimeControl.MainTime = _igsMatchRequest.MainTime.ToString();
            gameCreationViewModel.TimeControl.StonesPerPeriod = "25";

        }
    }
}