using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Defeatist;
using OmegaGo.Core.AI.FuegoSpace;
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
        /// Fuego AI builder. <see cref="FuegoSingleton"/> uses this to create the Fuego engine proper. 
        /// </summary>
        internal static IGtpEngineBuilder FuegoBuilder;

        /// <summary>
        /// Indicates whether Fuego has been registered.
        /// </summary>
        private static bool RegistrationComplete;

        /// <summary>
        /// Gets the list of AI programs known to this application. This should only be used after Fuego is registered 
        /// (if we are compiling with Fuego).
        /// </summary>
        public static IEnumerable<IAIProgram> AIPrograms
        {
            get
            {
                if (!RegistrationComplete)
                {
                    // Fuego is not available
                    return
                        new List<IAIProgram>
                        {
                            new DefeatistAI(),
                            new RandomAI(),
                            new RandomPlayerWrapper(),
                            new HeuristicPlayerWrapper(),
                            new Fluffy()
                        };
                }
                return
                    new List<IAIProgram>
                    {
                        new DefeatistAI(),
                        new RandomAI(),
                        new RandomPlayerWrapper(),
                        new HeuristicPlayerWrapper(),
                        new Fluffy(),
                        new FuegoSpace.Fuego()
                    };
            }
        }

        /// <summary>
        /// Registers a Fuego wrapper builder. This method should be called once, at the start of 
        /// the application, by the frontend. The builder will be stored as a static member in this class and will be used to create a new Fuego instance for each game. Fuego must be registered externally because it uses C++ code which cannot be referenced from a Portable .NET library.
        /// </summary>
        /// <param name="builder">The builder that can create Fuego instances.</param>
        public static void RegisterFuegoBuilder(IGtpEngineBuilder builder, ulong availableMemory)
        {
            ulong requiredMemoryForFuego = 850000000ul; // 850MB

            if (availableMemory < requiredMemoryForFuego)
                return;

            FuegoBuilder = builder;
            FuegoSingleton.Instance.AppWideInitialization();
            AISystems.RegistrationComplete = true;
        }        
    }
}