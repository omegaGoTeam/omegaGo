using System;
using OmegaGo.Core.AI;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewAiPlayer : GameCreationViewPlayer
    {
        private IAIProgram ai;
        public GameCreationViewAiPlayer(Core.AI.IAIProgram program)
        {
            this.Name = "AI: " + program.Name;
            this.ai = program;
        }

        public override string Description => this.ai.Description;
        public override bool IsAi => true;
        public AICapabilities Capabilities => this.ai.Capabilities;

        public override GamePlayer Build(StoneColor color, TimeControlSettingsViewModel timeSettings)
        {
            IAIProgram newInstance = (IAIProgram)Activator.CreateInstance(this.ai.GetType());
            return new AiPlayerBuilder(color)
                .Name(this.ai.Name + "(" + color.ToIgsCharacterString() + ")")
                .Rank("NR")
                .Clock(timeSettings.Build())
                .AiProgram(newInstance)
                .Build();
        }
    }
}