using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            UpdateOpponentFromProposal(challenge.Proposal.Players);
        }

        public KgsChallenge Challenge { get; }

        public override GameCreationFormStyle Style => GameCreationFormStyle.KgsChallengeNegotiation;
        public override string TabTitle => Challenge.ToString();
        public override bool HandicapMayBeChanged => false;
        public override string OpponentName
        {
            get { return _opponentName; }
        }
        public override bool Frozen => true;
        public override bool WillCreateChallenge => false;
        public override bool AcceptableAndRefusable => true;
        // TODO Petr: freeze komi
        public override void OnLoad(GameCreationViewModel vm)
        {
            _vm = vm;
            vm.FormTitle = Localizer.Creationg_KgsChallenge;
            vm.RefusalCaption = Localizer.UnjoinChallenge;
            vm.CustomSquareSize = Challenge.Proposal.Rules.Size.ToString();
            vm.SelectedRuleset = KgsGameInfo.ConvertRuleset(Challenge.Proposal.Rules.Rules);
            var you = Challenge.Proposal.Players.FirstOrDefault(pl => pl.GetName() == Connections.Kgs.Username); // TODO Petr: this is errorful;
            vm.SelectedColor = (Challenge.Proposal.Nigiri || you == null)
                ? Core.Game.StoneColor.None
                : (you.Role == Role.White ? Core.Game.StoneColor.White : Core.Game.StoneColor.Black);
            vm.Handicap = Challenge.Proposal.Rules.Handicap;
            vm.CompensationString = Challenge.Proposal.Rules.Komi.ToString(CultureInfo.InvariantCulture);
            vm.UseRecommendedKomi = false;
            UpdateTimeControlFromRules(Challenge.Proposal.Rules);
            Connections.Kgs.Events.Unjoin += Events_Unjoin;
            Challenge.StatusChanged += Challenge_StatusChanged;
            base.OnLoad(vm);
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

        private void Challenge_StatusChanged(object sender, EventArgs e)
        {
            // TODO Petr change accept/decline single from this
            if (Challenge.IncomingChallenge != null)
            {
                UpdateOpponentFromProposal(Challenge.IncomingChallenge.Players);
                _vm.OpponentName = _opponentName;
            }
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
            var opponent = players.FirstOrDefault(player => player.User?.Name != Connections.Kgs.Username);
            _opponentName = opponent.Name ?? "[no opponent yet]";
        }

        // TODO Petr: updating username

        public override async Task RefuseChallenge(GameCreationViewModel gameCreationViewModel)
        {
            await Connections.Kgs.Commands.GenericUnjoinAsync(Challenge);
        }
    }
}
