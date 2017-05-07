using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Modes.LiveGame;

namespace OmegaGo.Core.AI.FuegoSpace
{
    class FuegoEngine
    {
        private static FuegoEngine _instance;
        private IGtpEngine _engine;

        public static FuegoEngine Instance => FuegoEngine._instance ?? (FuegoEngine._instance = new FuegoEngine());

        private FuegoEngine()
        {
            AppWideInitialization();
        }
        
        public GameController CurrentGame { get; set; }   

        private void AppWideInitialization()
        {
            _engine = AISystems.FuegoBuilder.CreateEngine(0);
        }
    }
}
