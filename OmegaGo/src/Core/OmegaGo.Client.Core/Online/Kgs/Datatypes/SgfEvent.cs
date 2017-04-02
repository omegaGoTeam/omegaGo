using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;

namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    public class SgfEvent
    {
        /// <summary>
        /// ID of the SGF node where the event takes place
        /// </summary>
        public int NodeId { get; set; }
        /// <summary>
        /// Uppercase. What happened and what type of data is in the object, such as 'PROP_ADDED' or 'ACTIVATED'.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// For PROP_ADDED, PROP_REMOVED, PROP_CHANGED, the property that is affected by the event.
        /// </summary>
        public KgsSgfProperty Prop { get; set; }
        /// <summary>
        /// For CHILD_ADDED, the ID of the new child (which is a child of the current node).
        /// </summary>
        public int ChildNodeId { get; set; }
        /// <summary>
        /// For CHILD_ADDED: Optional. This is the order, in the child list, of the current child. If this is missing, then it is child 0 (the first child) of the current node.
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// For PROP_GROUP_ADDED and PROP_GROUP_REMOVED, a list of properties that are affected.
        /// </summary>
        public KgsSgfProperty[] Props { get; set; }

        // Some useless properties are missing

        public void ExecuteAsIncoming(KgsConnection connection, KgsGame ongame)
        {
            switch (Type)
            {
                case "PROP_GROUP_ADDED":
                    foreach (var prop in Props)
                    {
                        ongame.Controller.Nodes[NodeId].AddProperty(prop, ongame);
                    }
                    break;
                case "CHILD_ADDED":
                    ongame.Controller.Nodes[NodeId].AddChild(ChildNodeId, Position, ongame);
                    break;
                case "PROP_ADDED":
                    ongame.Controller.Nodes[NodeId].AddProperty(Prop, ongame);
                    break;
                case "PROP_CHANGED":
                    if (Prop.Name == "COMMENT")
                    {
                        ongame.Controller.Nodes[NodeId].AddProperty(Prop, ongame);
                    }
                    break;
                case "ACTIVATED":
                    // Ignore for now
                    break;
                default:
                    // TODO Petr 
                    throw new System.Exception("Unexpected SGF event.");
                    // break;
            }
        }

        public override string ToString()
        {
            return Type + " (" + (Props?.Length) + ")";
        }
    }
}