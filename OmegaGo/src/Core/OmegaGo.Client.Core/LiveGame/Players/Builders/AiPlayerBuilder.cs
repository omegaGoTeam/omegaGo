using System;
using OmegaGo.Core.AI;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;

namespace OmegaGo.Core.Modes.LiveGame.Players.Builders
{
    public sealed class AiPlayerBuilder : PlayerBuilder<GamePlayer, AiPlayerBuilder>
    {
        private IAIProgram _aiProgram;

        public AiPlayerBuilder(StoneColor color) : base(color)
        {
        }

        /// <summary>
        /// Sets the AI program this player will use
        /// </summary>
        /// <param name="program">AI program</param>
        /// <returns>Builder</returns>
        public AiPlayerBuilder AiProgram(IAIProgram program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            _aiProgram = program;
            return this;
        }

        public override GamePlayer Build() => 
            new GamePlayer(CreatePlayerInfo(), new AiAgent(Color, _aiProgram), TimeClock);
    }
}
