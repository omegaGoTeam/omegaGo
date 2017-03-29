using System;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsEvents
    {
        private readonly IgsConnection _igsConnection;

        public IgsEvents(IgsConnection igsConnection)
        {
            this._igsConnection = igsConnection;
        }

        public event EventHandler<bool> LoginComplete;

        public void RaiseLoginComplete(bool success)
        {
            LoginComplete?.Invoke(this, success);
        }

        public event EventHandler Disconnected;

        internal void RaiseDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}