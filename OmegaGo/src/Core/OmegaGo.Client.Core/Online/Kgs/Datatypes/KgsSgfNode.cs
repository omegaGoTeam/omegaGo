using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// Represents a node in the special KGS SGF file.
    /// </summary>
    public class KgsSgfNode
    {
        /// <summary>
        /// Gets or sets the index associated with a node. The root node of the tree has the index 0 and each games begins with this node already existing. The first node that has a move has the index 1 (unless that move was undone).
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the number of edges from the root node that one must traverse to reach this node. For example, if this is a grandson
        /// of the root, then Layer is 2.
        /// </summary>
        public int Layer { get; }
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

        public KgsSgfNode Parent { get; }

        public KgsSgfNode(int index, int layer, KgsSgfNode parent)
        {
            Index = index;
            Layer = layer;
            Parent = parent;
        }

        public void AddChild(int childNodeId, int position, KgsGame game)
        {
            var newNode = new KgsSgfNode(childNodeId, this.Layer + 1, this);
            Children.Insert(position, newNode);
            game.Controller.Nodes[childNodeId] = newNode;
        }

        public void AddProperty(KgsSgfProperty prop, KgsGame ongame)
        {
            Properties.Add(prop);
            ExecuteProperty(prop, ongame);
        }

        public void RemoveProperty(KgsSgfProperty prop, KgsGame ongame)
        {
            var existing =  Properties.FirstOrDefault(p => p.Name == prop.Name && (p.Loc?.SameAs(prop.Loc) ?? true));
            if (existing != null)
            {
                Properties.Remove(existing);
            }
            ExecutePropertyRemoval(prop, ongame);
        }

        

        private void ExecuteProperty(KgsSgfProperty prop, KgsGame ongame)
        {
            switch (prop.Name)
            {
                
                case "RULES":
                    RulesDescription rules = prop;
                    ongame.Info.BoardSize = new GameBoardSize(rules.Size);
                    foreach(var player in ongame.Controller.Players)
                    {
                        player.Clock = rules.CreateTimeControl();
                    }
                    ongame.Info.NumberOfHandicapStones = rules.Handicap;
                    ongame.Info.Komi = rules.Komi;
                    ongame.Info.RulesetType = KgsHelpers.ConvertRuleset(rules.Rules);
                    // (future work) TODO (Petr) ensure that even written late, these values are respected
                    break;
                case "PLAYERNAME":
                case "PLAYERRANK":
                case "DATE":
                case "PLACE":
                    // We do not need this information - we already have some and don't need the trest.
                    break;
                case "PHANTOMCLEAR":
                    // I don't know what to do with this yet.
                    break;
                case "ADDSTONE":
                    ongame.Controller.AddHandicapStonePosition(new Position(prop.Loc.X,
                        KgsCoordinates.TheirsToOurs(prop.Loc.Y, ongame.Info.BoardSize)));
                    break;
                case "COMMENT":
                    // "Putti [2k]: hi\n
                    string[] splitByNewlines =
                        prop.Text.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach(var s in splitByNewlines)
                    {
                        var tuple = KgsRegex.ParseCommentAsChat(s);
                        if (tuple != null)
                        {
                            var chatMessage = new ChatMessage(tuple.Item1, tuple.Item2,
                                DateTimeOffset.Now, tuple.Item1 == ongame.Controller.Server.Username ? ChatMessageKind.Outgoing : ChatMessageKind.Incoming);
                            ongame.Controller.KgsConnector.ChatMessageFromServer(chatMessage);
                        }
                    }
                    break;
                case "DEAD":
                    if (ongame.Controller.Phase.Type != Modes.LiveGame.Phases.GamePhaseType.LifeDeathDetermination)
                    {
                        ongame.Controller.SetPhase(Modes.LiveGame.Phases.GamePhaseType.LifeDeathDetermination);
                    }
                    ongame.Controller.BlackDoneReceived = false;
                    ongame.Controller.WhiteDoneReceived = false;
                    ongame.Controller.KgsConnector.ForceKillGroup(new Position(prop.Loc.X, KgsCoordinates.TheirsToOurs(prop.Loc.Y, ongame.Info.BoardSize)));
                    break;
                case "TIMELEFT":
                    StoneColor colorTimeLeft = (prop.Color == "black" ? StoneColor.Black : StoneColor.White);
                    ongame.Controller.Players[colorTimeLeft].Clock.UpdateFromKgsFloat(prop.Float);
                    break;
                case "MOVE":
                    Move move;
                    string propColor = prop.Color;
                    StoneColor color = propColor == "white" ? StoneColor.White : StoneColor.Black;
                    if (!prop.Loc.IsPass)
                    {
                        XY whereTo = prop.Loc;
                        Position position = new Game.Position(whereTo.X, KgsCoordinates.TheirsToOurs(whereTo.Y, ongame.Info.BoardSize));
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
                            ((KgsAgent) player.Agent).StoreMove(this.Layer, color, move);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        private void ExecutePropertyRemoval(KgsSgfProperty prop, KgsGame ongame)
        {
            switch (prop.Name)
            {
                case "DEAD":
                    if (ongame.Controller.Phase.Type != Modes.LiveGame.Phases.GamePhaseType.LifeDeathDetermination)
                    {
                        ongame.Controller.SetPhase(Modes.LiveGame.Phases.GamePhaseType.LifeDeathDetermination);
                    }
                    ongame.Controller.BlackDoneReceived = false;
                    ongame.Controller.WhiteDoneReceived = false;
                    ongame.Controller.KgsConnector.ForceRevivifyGroup(
                        new Position(prop.Loc.X, KgsCoordinates.TheirsToOurs(prop.Loc.Y, ongame.Info.BoardSize)));
                    break;
                default:
                    break;
            }

        }

        public override string ToString()
        {
            return "Node [" + Properties.Count + " props, first: " + Properties.FirstOrDefault()?.Name + "]";
        }

    }
}
