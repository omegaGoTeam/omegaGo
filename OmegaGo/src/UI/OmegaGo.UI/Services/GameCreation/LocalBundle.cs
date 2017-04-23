using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.GameCreation
{
    /// <summary>
    /// Base class for methods that result in a local game.
    /// </summary>
    abstract class LocalBundle : GameCreation.GameCreationBundle
    {
        public override GameCreationFormStyle Style => GameCreationFormStyle.LocalGame;

        public override bool SupportsRectangularBoards => true;

        public override bool SupportsChangingRulesets => true;

        public override bool AcceptableAndRefusable => false;
        public override bool Frozen => false;

        public override bool Playable => true;

        public override bool BlackAndWhiteVisible => true;

        public override bool WillCreateChallenge => false;

        public override bool KomiIsAvailable => true;

        public override bool HandicapMayBeChanged => true;
        public override string TabTitle => Localizer.NewGame;
    }
}
