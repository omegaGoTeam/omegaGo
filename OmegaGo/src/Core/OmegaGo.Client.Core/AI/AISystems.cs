using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Joker23.Players;

namespace OmegaGo.Core.AI
{
    public static class AISystems
    {
        public static List<IAIProgram> AiPrograms { get; } = new List<IAIProgram>()
        {
            new Defeatist.Defeatist(),
            new Random.RandomAI(),
            new RandomPlayerWrapper(),
            new HeuristicPlayerWrapper(),
            new AlphaBetaPlayerWrapper()
        };
    }
}
