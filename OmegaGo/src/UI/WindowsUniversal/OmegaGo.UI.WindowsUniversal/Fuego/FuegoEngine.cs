using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuegoLib;
using OmegaGo.Core.AI.Fuego;

namespace OmegaGo.UI.WindowsUniversal.Fuego
{
    class FuegoEngine : IGtpEngine
    {
        private int boardSize;
        private FuegoInstance _fuegoInstance;

        public FuegoEngine(int boardSize)
        {
            this.boardSize = boardSize;
            this._fuegoInstance = new FuegoInstance();
            this._fuegoInstance.StartGame((byte)boardSize);
        }

        public string SendCommand(string command)
        {
            return _fuegoInstance.HandleCommand(command);
        }
    }
}
