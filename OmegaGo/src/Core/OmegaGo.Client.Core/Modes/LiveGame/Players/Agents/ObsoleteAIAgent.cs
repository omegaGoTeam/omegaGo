using System;
using System.Linq;
using System.Threading.Tasks;
using OmegaGo.Core.AI;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// Represents the agent that makes move for AI programs.
    /// </summary>
    /// <seealso cref="ObsoleteAgentBase" />
    //public class ObsoleteAIAgent : ObsoleteAgentBase
    //{
    //    /// <summary>
    //    /// The AI program that feeds moves to this agent.
    //    /// </summary>
    //    readonly IAIProgram _aiProgram;
    //    /// <summary>
    //    /// The strength (1-10) that this AI program should play at.
    //    /// </summary>
    //    public int Strength = 5;

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ObsoleteAIAgent"/> class, with the specified AI program making the decisions.
    //    /// </summary>
    //    /// <param name="aiProgram">The AI program that makes decisions.</param>
    //    public ObsoleteAIAgent(IAIProgram aiProgram)
    //    {
    //        _aiProgram = aiProgram;
    //    }

    //    public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.MakeRandomMove;
    //    public override async void PleaseMakeAMove()
    //    { 
    //        GameBoard createdBoard = ObsoleteFastBoard.CreateBoardFromGame(Game);
    //        var aiTask = Task.Run(() => _aiProgram.RequestMove(new AIPreMoveInformation(
    //          Player.Info.Color,
    //          createdBoard,
    //          Game.BoardSize,
    //          new TimeSpan(0, 0, 2),
    //          Strength,
    //          Game.PrimaryMoveTimeline.ToList()
    //          )));
    //        AiDecision decision = await aiTask;
    //        switch (decision.Kind)
    //        {
    //            case AgentDecisionKind.Move:
    //                Game.GameController.MakeMove(Player, decision.Move);
    //                break;
    //            case AgentDecisionKind.Resign:
    //                Game.GameController.Resign(Player);
    //                break;
    //            default:
    //                throw new Exception("This decision kind does not exist.");
    //        }
    //    }
    //}
}
