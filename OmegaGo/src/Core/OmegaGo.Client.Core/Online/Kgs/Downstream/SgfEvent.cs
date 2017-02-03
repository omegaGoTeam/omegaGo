using OmegaGo.Core.Modes.LiveGame.Online;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public class SgfEvent
    {
        public int NodeId { get; set; }
        public string Type { get; set; }
        public KgsSgfProperty Prop { get; set; }
        public int ChildNodeId { get; set; }
        public int Position { get; set; }
        public KgsSgfProperty[] Props { get; set; }
        public int PrevNodeId { get; set; }
        // Some useless properties are missing

        public void ExecuteAsIncoming(KgsConnection connection, KgsGame ongame)
        {

        }
    }

    public class KgsSgfProperty : RulesDescription
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public XY Loc { get; set; }
        public XY Loc2 { get; set; }
        public string Text { get; set; }
        public float Float { get; set; }
        public int Int { get; set; }
    }

    public class XY
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}