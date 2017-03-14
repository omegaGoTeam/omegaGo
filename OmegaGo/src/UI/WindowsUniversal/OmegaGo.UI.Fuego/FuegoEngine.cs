using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuegoLib;
using OmegaGo.Core.AI.Fuego;

namespace OmegaGo.UI.WindowsUniversal.Fuego
{
    /// <summary>
    /// This is created by <see cref="FuegoBuilder"/>. One instance of this class is created for each game in which
    /// Fuego plays (or two instances if Fuego plays as both Black and White). 
    /// </summary>
    /// <seealso cref="OmegaGo.Core.AI.Fuego.IGtpEngine" />
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

        public GtpResponse SendCommand(string command)
        {
            string res = _fuegoInstance.HandleCommand(command);
            bool success = false;
            if (res.StartsWith("= "))
            {
                success = true;
                res = res.Substring(2);
            }
            if (res.StartsWith("? ")) {
                success = false;
                res = res.Substring(2);
            }
            return new GtpResponse(success, res.Trim());
        }
    }
}
