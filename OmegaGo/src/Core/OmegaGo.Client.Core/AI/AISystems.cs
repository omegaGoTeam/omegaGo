using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Fuego;

namespace OmegaGo.Core.AI
{
    public static class AISystems
    {
        private static bool _registrationComplete;
        internal static IGtpEngineBuilder FuegoBuilder;
        public static void RegisterFuegoBuilder(IGtpEngineBuilder builder)
        {
            FuegoBuilder = builder;
            _registrationComplete = true;
        }

        public static List<IAIProgram> AiPrograms
        {
            get
            {
                if (!_registrationComplete)
                {
                    throw new Exception("Fuego was not yet registered!");
                }
                return

                    new List<IAIProgram>
                    {
                        new Defeatist.Defeatist(),
                        new Random.RandomAI(),
                        new Joker23.RandomPlayerWrapper(),
                        new Joker23.HeuristicPlayerWrapper(),
                        new Joker23.AlphaBetaPlayerWrapper(),
                        new FuegoWrapper()
                    }

                    ;
            }

        }
    }
}