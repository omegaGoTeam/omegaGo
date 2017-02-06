using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.AI
{
    public sealed class AiPlayerBuilder : PlayerBuilder<GamePlayer, AiPlayerBuilder>
    {
        private IAIProgram _aiProgram = null;
        private int _strength = 5;
        private TimeSpan _timeLimit = new TimeSpan(0, 0, 2);

        public AiPlayerBuilder(StoneColor color) : base(color)
        {
        }

        /// <summary>
        /// Sets the strength of AI
        /// </summary>
        /// <param name="strength">Strength of AI</param>
        /// <returns>Builder</returns>
        public AiPlayerBuilder AiStrength(int strength)
        {
            _strength = strength;
            return this;
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

        /// <summary>
        /// Sets the AI move time limit
        /// </summary>
        /// <param name="timeLimit">Time limit</param>
        /// <returns>Builder</returns>
        public AiPlayerBuilder TimeLimit(TimeSpan timeLimit)
        {
            _timeLimit = timeLimit;
            return this;
        }

        public override GamePlayer Build() => 
            new GamePlayer(CreatePlayerInfo(), new AiAgent(Color, _aiProgram, _strength, _timeLimit), TimeClock);
    }
}
