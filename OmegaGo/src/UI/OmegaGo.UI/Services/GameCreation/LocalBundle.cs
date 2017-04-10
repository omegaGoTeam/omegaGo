using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.GameCreation
{
    abstract class LocalBundle : GameCreationBundle.GameCreationBundle
    {
        public override GameCreationFormStyle Style => GameCreationFormStyle.LocalGame;

        public override bool SupportsRectangularBoards => true;
    }
}
