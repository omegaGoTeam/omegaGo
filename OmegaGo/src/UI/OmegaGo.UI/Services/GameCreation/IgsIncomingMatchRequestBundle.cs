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
        public override bool CanReturn => false;


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
        public override void OnLoad(GameCreationViewModel vm)
        {
            base.OnLoad(vm);
            
            vm.FormTitle = Localizer.Creation_IncomingIgsChallenge;
            vm.RefusalCaption = Localizer.RefuseChallenge;
            vm.CustomSquareSize = _igsMatchRequest.BoardSize.ToString();
            vm.SelectedColor = _igsMatchRequest.YourColor;
            vm.TimeControl.Style = Core.Time.TimeControlStyle.Canadian;
            vm.TimeControl.OvertimeMinutes = _igsMatchRequest.OvertimeMinutes.ToString();
            vm.TimeControl.MainTime = _igsMatchRequest.MainTime.ToString();
            vm.TimeControl.StonesPerPeriod = "25";

        }
    }
}