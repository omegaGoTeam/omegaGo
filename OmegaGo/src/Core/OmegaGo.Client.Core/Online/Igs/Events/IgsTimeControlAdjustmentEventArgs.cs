using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Online.Igs.Events
{
    public class IgsTimeControlAdjustmentEventArgs
    {
        public CanadianTimeInformation White { get;  }
        public CanadianTimeInformation Black { get;  }

        public IgsTimeControlAdjustmentEventArgs(CanadianTimeInformation white, CanadianTimeInformation black)
        {
            this.White = white;
            this.Black = black;
        }
    }
}