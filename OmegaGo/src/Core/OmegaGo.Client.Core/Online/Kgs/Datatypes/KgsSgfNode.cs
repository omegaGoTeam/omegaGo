using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Players.Igs;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs.Sgf
{
    public class KgsSgfNode
    {
        public int Index { get; set; }
        public List<KgsSgfNode> Children { get; } = new List<KgsSgfNode>();
        public List<KgsSgfProperty> Properties { get; } = new List<KgsSgfProperty>();

        public KgsSgfNode(int index)
        {
            Index = index;
        }

        public void AddChild(int childNodeId, int position, KgsGame game)
        {
            var newNode = new Sgf.KgsSgfNode(childNodeId);
            Children.Insert(position, newNode);
            game.Nodes[childNodeId] = newNode;
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
                    // TODO solve later
                    break;
                case "PLAYERNAME":
                case "PLAYERRANK":
                case "DATE":
                case "PLACE":
                    // TODO ignore for now
                    break;
                case "COMMENT":
                case "TIMELEFT":
                    // TODO ignore for lesser now
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
