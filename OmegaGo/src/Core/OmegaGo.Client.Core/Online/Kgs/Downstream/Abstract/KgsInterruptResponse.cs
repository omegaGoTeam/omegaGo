namespace OmegaGo.Core.Online.Kgs.Downstream.Abstract
{
    public abstract class KgsInterruptResponse : KgsResponse
    {
        public abstract void Process(KgsConnection connection);
    }
}
