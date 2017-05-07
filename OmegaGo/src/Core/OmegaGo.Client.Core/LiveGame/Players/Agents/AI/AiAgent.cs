using System;
using System.Threading.Tasks;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents.AI
{
    public class AiAgent : AgentBase
    {
        private readonly IAIProgram _aiProgram;
        
        public IAIProgram AI => _aiProgram;

        public AiAgent(StoneColor color, IAIProgram aiProgram) : base(color)
        {
            _aiProgram = aiProgram;
        }

        /// <summary>
        /// AI notes
        /// </summary>
        public event AgentEventHandler<string> AiNote;

        public override AgentType Type => AgentType.AI;

        public override IllegalMoveHandling IllegalMoveHandling => IllegalMoveHandling.PassInstead;

        public override void GameInitialized()
        {
            if (AI is Fuego)
            {
                (AI as Fuego).Initialize(this);
            }
        }
        
        public override async void PleaseMakeAMove()
        {
            GameTreeNode respondingToWhatNode = GameState.GameTree.LastNode;
            var aiTask = Task.Run(() => _aiProgram.RequestMove(new AiGameInformation(
               GameInfo,
               Color,
               GameState.Players[Color],
               GameState.GameTree
               )));

            AIDecision decision = await aiTask;
            foreach(var aiNote in decision.AiNotes)
            {
                SendAiNote(aiNote);
            }
            if (respondingToWhatNode != GameState.GameTree.LastNode)
            {
                // Ignore. That result is now obsolete.
                _aiProgram.YourMoveWasRejected();
                return;
            }
            switch (decision.Kind)
            {
                case AgentDecisionKind.Move:
                    if (decision.Move.Kind == MoveKind.PlaceStone)
                    {
                        OnPlaceStone(decision.Move.Coordinates);
                    }
                    else
                    {
                        OnPass();
                    }
                    break;
                case AgentDecisionKind.Resign:
                    OnResign();
                    break;
                default:
                    throw new Exception("This decision kind does not exist.");
            }
        }

        public override void GamePhaseChanged(GamePhaseType phase)
        {
            if (phase == GamePhaseType.Finished)
            {
                (AI as OldFuego)?.Finished();
            }
            base.GamePhaseChanged(phase);
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

        public override void MovePerformed(Move move)
        {
            _aiProgram.MovePerformed(move, this.GameState.GameTree, this.GameState.Players[move.WhoMoves], GameInfo);
        }
        public override void MoveUndone()
        {
            _aiProgram.MoveUndone();
        }
    }
}
