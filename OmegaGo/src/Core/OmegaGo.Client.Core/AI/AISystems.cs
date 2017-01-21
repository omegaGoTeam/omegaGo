using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Fuego;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Contains a list of all supported AI programs.
    /// </summary>
    public static class AISystems
    {
        private static bool registrationComplete;
        internal static IGtpEngineBuilder FuegoBuilder;
        /// <summary>
        /// Registers a Fuego wrapper builder. This method should be called once, at the start of 
        /// the application, by the frontend. The builder will be stored as a static member in this class and will be used to create a new Fuego instance for each game. Fuego must be registered externally because it uses C++ code which cannot be referenced from a Portable .NET library.
        /// </summary>
        /// <param name="builder">The builder that can create Fuego instances.</param>
        public static void RegisterFuegoBuilder(IGtpEngineBuilder builder)
        {
            FuegoBuilder = builder;
            AISystems.registrationComplete = true;
        }

        /// <summary>
        /// Gets the list of AI programs known to this application. This cannot be used until
        /// Fuego is registered.
        /// </summary>
        /// <exception cref="Exception">Fuego was not yet registered!</exception>
        public static List<IAIProgram> AiPrograms
        {
            get
            {
                if (!AISystems.registrationComplete)
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
                    };
            }

        }
    }
}