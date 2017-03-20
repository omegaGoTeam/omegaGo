using System;
using System.Threading.Tasks;
using OmegaGo.Core.AI;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents.AI
{
    public class AiAgent : AgentBase
    {
        private readonly IAIProgram _aiProgram;
        private int _strength;
        private readonly TimeSpan _timeLimit;

        public AiAgent(StoneColor color, IAIProgram aiProgram, int strength, TimeSpan timeLimit) : base(color)
        {
            _aiProgram = aiProgram;
            _strength = strength;
            _timeLimit = timeLimit;
        }

        /// <summary>
        /// AI notes
        /// </summary>
        public event AgentEventHandler<string> AiNote;

        public override AgentType Type => AgentType.AI;

        public override IllegalMoveHandling IllegalMoveHandling => IllegalMoveHandling.PassInstead;

        public void SetStrength(int newStrength)
        {
            _strength = newStrength;
        }

        public override void GameInitialized()
        {
        }
        
        public override async void PleaseMakeAMove()
        {
            var aiTask = Task.Run(() => _aiProgram.RequestMove(new AIPreMoveInformation(
               GameInfo,
               Color,
               GameState.Players[Color],
               GameState.GameTree,
               _timeLimit,
               _strength
               )));

            AIDecision decision = await aiTask;
            foreach(var aiNote in decision.AiNotes)
            {
                SendAiNote(aiNote);
            }
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

        /// <summary>
        /// Sends a new AI note
        /// </summary>
        /// <param name="note">Note to send</param>
        private void SendAiNote(string note)
        {
            AiNote?.Invoke(this, note);
        }
        public override string ToString()
        {
            return "Agent for " + this._aiProgram;
        }
    }
}
