using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    public abstract class KgsBundle : GameCreationBundle
    {
        public override bool SupportsRectangularBoards => false;
        public override bool Playable => false;
        public override bool BlackAndWhiteVisible => false;
        public override bool SupportsChangingRulesets => true;
        public override bool IsIgs => false;
        public override bool IsKgs => true;
        public override bool KomiIsAvailable => false;
        public override bool CanReturn => false;
        public override bool HandicapMayBeChanged => false;

        public override void OnLoad(GameCreationViewModel vm)
        {
        }
    }
}
