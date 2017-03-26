namespace OmegaGo.Core.AI.FuegoSpace
{
    public class GtpResponse
    {
        public bool Successful { get; }
        public string Text { get; }
        public GtpResponse(bool success, string text)
        {
            this.Successful = success;
            this.Text = text;
        }
        public override string ToString()
        {
            return (Successful ? "= " : "? ") + Text;
        }
    }
}