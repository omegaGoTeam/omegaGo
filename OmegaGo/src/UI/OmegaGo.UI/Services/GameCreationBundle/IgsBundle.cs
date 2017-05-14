using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    abstract class IgsBundle : GameCreationBundle
    {
        public override bool SupportsChangingRulesets => false;
        public override bool SupportsRectangularBoards => false;
        public override bool Playable => false;
        public override bool BlackAndWhiteVisible => false;
        public override bool KomiIsAvailable => false;
        public override bool HandicapMayBeChanged => false;
        public override bool IsIgs => true;
        public override bool CanDeclineSingleOpponent => false;

        public override void OnLoad(GameCreationViewModel vm)
        {
            vm.IgsLimitation = true;
            vm.SelectedRuleset = Core.Rules.RulesetType.Japanese;
            vm.TimeControlStyles.Remove(Core.Time.TimeControlStyle.Japanese);
            Connections.Igs.Events.Disconnected += (o, e) =>
            {
                vm.CloseSelf();
            };
        }
    }
}
