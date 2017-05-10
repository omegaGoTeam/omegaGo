using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.UI.Localization;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    public class KgsCreateChallengeBundle : KgsBundle
    {
        private readonly KgsRoom _room;
        public override bool HandicapMayBeChanged => true;
        public override bool Frozen => false;
        public override bool WillCreateChallenge => true;
        public override bool AcceptableAndRefusable => false;
        public override GameCreationFormStyle Style => GameCreationFormStyle.KgsChallengeCreation;
        public override string TabTitle => Localizer.Creation_KgsChallengeCreation;
        public override bool CanDeclineSingleOpponent => false;
        public override string OpponentName => "";

        public KgsCreateChallengeBundle(KgsRoom room)
        {
            _room = room;
        }
        public override void OnLoad(GameCreationViewModel vm)
        {
            vm.FormTitle = Localizer.Creation_KgsChallengeCreation;
            base.OnLoad(vm);
        }
        public override async Task CreateChallenge(GameCreationViewModel vm)
        {
            string rulesString = "chinese";
            switch (vm.SelectedRuleset)
            {
                case Core.Rules.RulesetType.AGA:
                    rulesString = RulesDescription.RulesAga;
                    break;
                case Core.Rules.RulesetType.Chinese:
                    rulesString = RulesDescription.RulesChinese;
                    break;
                case Core.Rules.RulesetType.Japanese:
                    rulesString = RulesDescription.RulesJapanese;
                    break;
            }
            var rules = new RulesDescription()
            {
                Handicap = vm.Handicap,
                Komi = float.Parse(vm.CompensationString, CultureInfo.InvariantCulture),
                Rules = rulesString,
                Size = vm.SelectedGameBoardSize.Width
            };
            switch (vm.TimeControl.Style)
            {
                case Core.Time.TimeControlStyle.Absolute:
                    rules.TimeSystem = RulesDescription.TimeSystemAbsolute;
                    rules.MainTime = vm.TimeControl.MainTime.AsInteger() * 60;
                    break;
                case Core.Time.TimeControlStyle.Canadian:
                    rules.TimeSystem = RulesDescription.TimeSystemCanadian;
                    rules.MainTime = vm.TimeControl.MainTime.AsInteger() * 60;
                    rules.ByoYomiStones = vm.TimeControl.StonesPerPeriod.AsInteger();
                    rules.ByoYomiTime = vm.TimeControl.OvertimeMinutes.AsInteger() * 60;
                    break;
                case Core.Time.TimeControlStyle.Japanese:
                    rules.TimeSystem = RulesDescription.TimeSystemJapanese;
                    rules.MainTime = vm.TimeControl.MainTime.AsInteger()*60;
                    rules.ByoYomiTime = vm.TimeControl.OvertimeSeconds.AsInteger();
                    rules.ByoYomiPeriods = vm.TimeControl.NumberOfJapanesePeriods.AsInteger();
                    break;
                case Core.Time.TimeControlStyle.None:
                    rules.TimeSystem = RulesDescription.TimeSystemNone;
                    break;
            }
            
            await Connections.Kgs.Commands.CreateChallenge(_room, vm.IsRankedGame, vm.IsPubliclyListedGame,
                    rules, vm.SelectedColor);
            Mvx.Resolve<IAppNotificationService>()
                .TriggerNotification(new Notifications.BubbleNotification(LocalizedStrings.ChallengeIsBeingCreated, null, NotificationType.Info));
            vm.CloseSelf();
        }
    }
}
