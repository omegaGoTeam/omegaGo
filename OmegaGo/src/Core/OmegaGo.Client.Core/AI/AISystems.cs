using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    public static class AISystems
    {
        public static List<IAIProgram> AiPrograms { get; } = new List<IAIProgram>()
        {
            new Defeatist.Defeatist(),
            new Random.RandomAI(),
            new Joker23.RandomPlayerWrapper(),
            new Joker23.HeuristicPlayerWrapper()
        };
    }
}

namespace OmegaGo.Core.AI.Joker23
{
}
