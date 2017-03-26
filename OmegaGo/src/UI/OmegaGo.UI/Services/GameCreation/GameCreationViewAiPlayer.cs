using System;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.Converters;
using OmegaGo.UI.Services.Localization.LocalizedMetadata;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    public class GameCreationViewAiPlayer : GameCreationViewPlayer
    {
        private readonly IAIProgram _ai;
        private readonly AiProgramLocalizedMetadata _aiLocalizedMetadata;

        public GameCreationViewAiPlayer(IAIProgram program)
        {
            _aiLocalizedMetadata = new AiProgramLocalizedMetadata(program);
            this.Name = "AI: " + _aiLocalizedMetadata.Name;
            this._ai = program;
        }

        public override string Description => _aiLocalizedMetadata.Description;

        public override bool IsAi => true;

        public IAIProgram AI => _ai;

        public AICapabilities Capabilities => this._ai.Capabilities;

        public override GamePlayer Build(StoneColor color, TimeControlSettingsViewModel timeSettings, PlayerSettingsViewModel settings)
        {
            IAIProgram newInstance = (IAIProgram)Activator.CreateInstance(_ai.GetType());
            if (settings.IsFuego)
            {
                Fuego fuego = (Fuego) newInstance;
                fuego.Ponder = settings.FuegoPonder;
                fuego.AllowResign = settings.FuegoResign;
                fuego.MaxGames = settings.FuegoMaxGames;
            }
            else if (settings.IsFluffy)
            {
                Fluffy fluffy = (Fluffy) newInstance;
                fluffy.TreeDepth = settings.FluffyTreeDepth;
            }
            return new AiPlayerBuilder(color)
                .Name(this._aiLocalizedMetadata.Name + "(" + color.ToIgsCharacterString() + ")")
                .Rank("NR")
                .Clock(timeSettings.Build())
                .AiProgram(newInstance)
                .Build();
        }
    }
}