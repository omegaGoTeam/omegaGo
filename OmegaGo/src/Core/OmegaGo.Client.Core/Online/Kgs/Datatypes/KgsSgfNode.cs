using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;

namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// Represents a node in the special KGS SGF file.
    /// </summary>
    public class KgsSgfNode
    {
        /// <summary>
        /// Gets or sets the index associated with a node. The root node of the tree has the index 0 and each games begins with this node already existing. The first node that has a move has the index 1.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Gets the children of this node. Order matters.
        /// </summary>
        public List<KgsSgfNode> Children { get; } = new List<KgsSgfNode>();
        /// <summary>
        /// Gets SGF roperties associated with this node. 
        /// 
        /// The SGF in KGS is not quite like "standard" SGF. Multiple marks of the same type are considered different properties, so for example TR[aa][bb][cc] on KGS would be three different properties, one for each location. All rules-related properties (SZ[], TM[], etc) are grouped together in one "rules" property, etc. Furthere, some properties, such as DEAD, are not part of SGF but are used internally by KGS to track the state of the game board.
        /// </summary>
        public List<KgsSgfProperty> Properties { get; } = new List<KgsSgfProperty>();

        public KgsSgfNode(int index)
        {
            Index = index;
        }

        public void AddChild(int childNodeId, int position, KgsGame game)
        {
            var newNode = new KgsSgfNode(childNodeId);
            Children.Insert(position, newNode);
            game.Controller.Nodes[childNodeId] = newNode;
        }

        public void AddProperty(KgsSgfProperty prop, KgsGame ongame)
        {
            Properties.Add(prop);
            ExecuteProperty(prop, ongame);
        }

        private void ExecuteProperty(KgsSgfProperty prop, KgsGame ongame)
        {
            switch (prop.Name)
            {
                case "RULES":
                    // TODO Petr : solve later
                    break;
                case "PLAYERNAME":
                case "PLAYERRANK":
                case "DATE":
                case "PLACE":
                    // TODO Petr : ignore for now
                    break;
                case "COMMENT":
                case "TIMELEFT":
                    // TODO Petr : ignore for lesser now
                    break;
                case "MOVE":
                    Move move;
                    string propColor = prop.Color;
                    StoneColor color = propColor == "white" ? StoneColor.White : StoneColor.Black;
                    if (prop.Loc is XY)
                    {
                        XY whereTo = (XY) prop.Loc;
                        Position position = new Game.Position(whereTo.X, whereTo.Y);
                        move = Move.PlaceStone(color, position);
                    }
                    else
                    {
                        move = Move.Pass(color);
                    }
                    foreach (var player in ongame.Controller.Players)
                    {
                        if (player.Agent is KgsAgent)
                        {
                            ((KgsAgent) player.Agent).StoreMove(this.Index, color, move);
                        }
                    }
                    break;
            }
        }
    }
}
