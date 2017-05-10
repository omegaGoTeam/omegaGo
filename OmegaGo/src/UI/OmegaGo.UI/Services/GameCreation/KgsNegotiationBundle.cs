using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    public abstract class KgsNegotiationBundle : KgsBundle
    {
        private GameCreationViewModel _vm;
        private string _opponentName;

        protected KgsNegotiationBundle(KgsChallenge challenge)
        {
            Challenge = challenge;
        }

        protected KgsChallenge Challenge { get; }

        public override GameCreationFormStyle Style => GameCreationFormStyle.KgsChallengeNegotiation;
        public override string TabTitle => Challenge.ToString();
        public override bool HandicapMayBeChanged => false;
        public override string OpponentName => _opponentName;
        public override bool Frozen => true;
        public override bool WillCreateChallenge => false;
        public override bool AcceptableAndRefusable => true;
        public override void OnLoad(GameCreationViewModel vm)
        {
            _vm = vm;
            LoadProposalDataIntoForm(Challenge.Proposal);
           
            Connections.Kgs.Events.Unjoin += Events_Unjoin;
            Challenge.StatusChanged += Challenge_StatusChanged;
            RefreshStatus();
            base.OnLoad(vm);
        }

        private void LoadProposalDataIntoForm(Proposal proposal)
        {
            var vm = _vm;
            vm.FormTitle = Localizer.Creationg_KgsChallenge;
            vm.RefusalCaption = Localizer.UnjoinChallenge;
            vm.CustomSquareSize = proposal.Rules.Size.ToString();
            vm.SelectedRuleset = KgsHelpers.ConvertRuleset(proposal.Rules.Rules);
            vm.IsRankedGame = proposal.GameType == GameType.Ranked;
            vm.IsPubliclyListedGame = proposal.Global;
            UpdateOpponentFromProposal(proposal.Players);
            foreach (var player in proposal.Players)
            {
                if (player.GetName() == Connections.Kgs.Username)
                {
                    vm.SelectedColor = player.Role == Role.White
                        ? Core.Game.StoneColor.White
                        : Core.Game.StoneColor.Black;
                }
                else if (!String.IsNullOrEmpty(player.GetName()))
                {
                    string opponent = player.GetNameAndRank();
                    this._opponentName = opponent;
                    vm.OpponentName = opponent;
                    if (player.Role == Role.White)
                    {
                        vm.SelectedColor = Core.Game.StoneColor.Black;
                    }
                    else if (player.Role == Role.Black)
                    {
                        vm.SelectedColor = Core.Game.StoneColor.White;
                    }
                    else
                    {
                        // Other roles don't affect color.
                    }
                }
            }
            if (proposal.Nigiri)
            {
                vm.SelectedColor = Core.Game.StoneColor.None;
            }
            vm.Handicap = proposal.Rules.Handicap;
            vm.CompensationString = proposal.Rules.Komi.ToString(CultureInfo.InvariantCulture);
            vm.UseRecommendedKomi = false;
            UpdateTimeControlFromRules(proposal.Rules);
            RefreshStatus();
        }

        private void UpdateTimeControlFromRules(RulesDescription rules)
        {
            var time = _vm.TimeControl;
            switch (rules.TimeSystem)
            {
                case RulesDescription.TimeSystemNone:
                    time.Style = Core.Time.TimeControlStyle.None;
                    break;
                case RulesDescription.TimeSystemJapanese:
                    time.Style = Core.Time.TimeControlStyle.Japanese;
                    time.MainTime = (rules.MainTime / 60).ToString();
                    time.NumberOfJapanesePeriods = rules.ByoYomiPeriods.ToString();
                    time.OvertimeSeconds = rules.ByoYomiTime.ToString();
                    break;
                case RulesDescription.TimeSystemCanadian:
                    time.Style = Core.Time.TimeControlStyle.Canadian;
                    time.MainTime = (rules.MainTime / 60).ToString();
                    time.StonesPerPeriod = rules.ByoYomiStones.ToString();
                    time.OvertimeMinutes = (rules.ByoYomiTime/60).ToString();
                    break;
                case RulesDescription.TimeSystemAbsolute:
                    time.Style = Core.Time.TimeControlStyle.Absolute;
                    time.MainTime = (rules.MainTime/60).ToString();
                    break;
            }
        }

        protected void ClearOpponentName()
        {
            _opponentName = Localizer.NoOpponentYet;
        }

        protected void RefreshStatus()
        {
            if (Challenge.IncomingChallenge != null)
            {
                LoadProposalDataIntoForm(Challenge.IncomingChallenge);
            }
            if (Challenge.CreatorsNewProposal != null)
            {
                LoadProposalDataIntoForm(Challenge.CreatorsNewProposal);
            }
            _vm.OpponentName = _opponentName;
            _vm.AcceptChallengeCommand.RaiseCanExecuteChanged();
            _vm.DeclineSingleOpponentCommand.RaiseCanExecuteChanged();
        }

        private void Challenge_StatusChanged(object sender, EventArgs e)
        {
            RefreshStatus();
        }

        private void Events_Unjoin(object sender, KgsChannel e)
        {
            // Hack: This is a memory leak. This event will never be collected. Oh well.
            if (e.ChannelId == Challenge.ChannelId)
            {
                _vm.CloseSelf();
            }
        }

        private void UpdateOpponentFromProposal(KgsPlayer[] players)
        {
            var opponent = players.FirstOrDefault(player => player.GetName() != Connections.Kgs.Username);
            _opponentName = opponent.GetNameAndRank() ?? Localizer.NoOpponentYet;
            _vm.OpponentName = _opponentName;
        }

        public override async Task RefuseChallenge(GameCreationViewModel gameCreationViewModel)
        {
            await Connections.Kgs.Commands.GenericUnjoinAsync(Challenge);
        }

        public override bool IsDeclineSingleOpponentEnabled()
        {
            return Challenge.IncomingChallenge != null;
        }

        public override bool IsAcceptButtonEnabled()
        {
            return Challenge.Acceptable || (Challenge.IncomingChallenge != null);
        }
    }
}
