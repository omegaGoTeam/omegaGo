using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreationBundle
{
    /// <summary>
    /// Represents a method by which one can enter the <see cref="GameCreationViewModel"/>.  
    /// See https://docs.google.com/document/d/1NiCWUH7otuoTEcxfbepS74mHEujEhrfboFybJBxJzWM/edit?usp=sharing for details.
    /// </summary>
    abstract class GameCreationBundle
    {
        public abstract GameCreationFormStyle Style { get; }
        public abstract void OnLoad(GameCreationViewModel gameCreationViewModel);
    }
}
