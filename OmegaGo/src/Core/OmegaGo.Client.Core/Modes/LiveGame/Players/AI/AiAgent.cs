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

        public override IllegalMoveHandling IllegalMoveHandling => IllegalMoveHandling.MakeRandomMove;

        public override async void OnTurn()
        {
            GameBoard createdBoard = GameBoard.CreateBoardFromGameTree(GameInfo, GameState.GameTree);
            var aiTask = Task.Run(() => _aiProgram.RequestMove(new AIPreMoveInformation(
                Color,
                createdBoard,
                _timeLimit,
                _strength,
                GameState.GameTree.PrimaryMoveTimeline.ToList()
                )));
            AiDecision decision = await aiTask;
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

        public override void MoveIllegal(MoveResult move)
        {
            throw new NotImplementedException();
        }
    }
}
