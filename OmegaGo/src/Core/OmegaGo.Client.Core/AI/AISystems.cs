using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Defeatist;
using OmegaGo.Core.AI.Fuego;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.Core.AI.Random;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Contains a list of all supported AI programs.
    /// </summary>
    public static class AISystems
    {
        /// <summary>
        /// Fuego AI builder
        /// </summary>
        internal static IGtpEngineBuilder FuegoBuilder;

        /// <summary>
        /// Indicates whether the AI systems have been registered
        /// </summary>
        private static bool _registrationComplete;

        /// <summary>
        /// Gets the list of AI programs known to this application. This cannot be used until
        /// Fuego is registered.
        /// </summary>
        /// <exception cref="Exception">Fuego was not yet registered!</exception>
        public static IEnumerable<IAIProgram> AIPrograms
        {
            get
            {
                if (!_registrationComplete)
                {
                    // Fuego is not available
                    return
                        new List<IAIProgram>
                        {
                            new DefeatistAI(),
                            new RandomAI(),
                            new RandomPlayerWrapper(),
                            new HeuristicPlayerWrapper(),
                            new AlphaBetaPlayerWrapper()
                        };
                }
                return
                    new List<IAIProgram>
                    {
                        new DefeatistAI(),
                        new RandomAI(),
                        new RandomPlayerWrapper(),
                        new HeuristicPlayerWrapper(),
                        new AlphaBetaPlayerWrapper(),
                        new FuegoAI()
                    };
            }
        }

        /// <summary>
        /// Registers a Fuego wrapper builder. This method should be called once, at the start of 
        /// the application, by the frontend. The builder will be stored as a static member in this class and will be used to create a new Fuego instance for each game. Fuego must be registered externally because it uses C++ code which cannot be referenced from a Portable .NET library.
        /// </summary>
        /// <param name="builder">The builder that can create Fuego instances.</param>
        public static void RegisterFuegoBuilder(IGtpEngineBuilder builder)
        {
            FuegoBuilder = builder;
            _registrationComplete = true;
        }        
    }
}