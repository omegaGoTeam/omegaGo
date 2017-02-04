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
            switch (Type)
            {
                case "PROP_GROUP_ADDED":
                    foreach (var prop in Props)
                    {
                        ongame.Nodes[NodeId].AddProperty(prop, ongame);
                    }
                    break;
                case "CHILD_ADDED":
                    ongame.Nodes[NodeId].AddChild(ChildNodeId, Position, ongame);
                    break;
                case "PROP_ADDED":
                    ongame.Nodes[NodeId].AddProperty(Prop, ongame);
                    break;
                default:
                    break;
                    // TODO
                   // throw new System.Exception("Unexpected SGF event.");
            }
        }
    }
}