using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.AI
{
    public class AiAgent : AgentBase
    {
        private readonly IAIProgram _aiProgram;
        private int _strength;
        private readonly TimeSpan _timeLimit;

        public void SetStrength(int newStrength)
        {
            _strength = newStrength;
        }

        public AiAgent(StoneColor color, IAIProgram aiProgram, int strength, TimeSpan timeLimit) : base(color)
        {
            _aiProgram = aiProgram;
            _strength = strength;
            _timeLimit = timeLimit;
        }

        public override AgentType Type => AgentType.AI;

        public override IllegalMoveHandling IllegalMoveHandling => IllegalMoveHandling.PassInstead;

        public override async void OnTurn()
        {
            var aiTask = Task.Run(() => _aiProgram.RequestMove(new AIPreMoveInformation(
                GameInfo,
                Color,
                GameState.GameTree,
                _timeLimit,
                _strength
                )));

            AiDecision decision = await aiTask;
            OnLogMessage(decision.Explanation);
            switch (decision.Kind)
            {
                case AgentDecisionKind.Move:
                    OnPlaceStone(decision.Move.Coordinates);
                    break;
                case AgentDecisionKind.Resign:
                    OnResign();
                    break;
                default:
                    throw new Exception("This decision kind does not exist.");
            }
        }

        public override void MoveIllegal(MoveResult moveResult)
        {
            throw new Exception("This should never be called.");
        }

        private void OnLogMessage(string msg)
        {
            LogMessage?.Invoke(this, msg);
        }

        public event EventHandler<string> LogMessage;
    }
}
