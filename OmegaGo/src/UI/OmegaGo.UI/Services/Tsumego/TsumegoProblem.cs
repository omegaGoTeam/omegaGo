namespace OmegaGo.UI.Services.Tsumego
{
    public class TsumegoProblem
    {
        public string Name;

        public TsumegoProblem(string name)
        {
            this.Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}