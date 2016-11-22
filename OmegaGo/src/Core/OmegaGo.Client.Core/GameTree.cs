using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    public sealed class GameTree
    {
        private GameTreeNode _lastNode;

        // Information taken from official SGF file definition
        // http://www.red-bean.com/sgf/proplist_ff.html

        // BR: game-info
        public string BlackRank { get; set; }

        // BT: game-info
        public string BlackTeam { get; set; }

        // CP: game-info
        public string Copyright { get; set; }

        // DT: game-info
        public string Date { get; set; }

        // GC: game-info
        public string GameComment { get; set; }

        // EV: game-info
        public string Event { get; set; }

        // GN: game-info
        public string GameName { get; set; }

        // HA: game-info
        public int Handicap { get; set; }

        // KM: game-info
        public double Komi { get; set; }

        // ON: game-info
        public string Opening { get; set; }

        // OT: game-info
        public string Ovetime { get; set; }

        // PB: game-info
        public string PlayerBlack { get; set; }

        // PC: game-info
        public string Place { get; set; }

        // PW: game-info
        public string PlayerWhite { get; set; }

        // RE: game-info
        public string Result { get; set; }

        // RO: game-info
        public string Round { get; set; }

        // RU: game-info
        public string Rules { get; set; }

        // SO: game-info
        public string Source { get; set; }

        // SZ: root
        public GameBoardSize Size { get; set; }

        // TM: game-info
        public double TimeLimit { get; set; }

        // US: game-info
        public string User { get; set; }

        // WR: game-info
        public string WhiteRank { get; set; }

        // WT: game-info
        public string WhiteTeam { get; set; }

        public GameTreeNode GameTreeRoot { get; set; }

        /// <summary>
        /// Reference to the last added node
        /// </summary>
        public GameTreeNode LastNode
        {
            get { return _lastNode; }
            set { _lastNode = value; }
        }
        
        public GameTree()
        {

        }
        
        public void AddMoveToEnd(Move move)
        {
            if (GameTreeRoot == null)
            {
                GameTreeRoot = new GameTreeNode(move);
                LastNode = GameTreeRoot;
                return;
            }
            
            GameTreeNode parent = GameTreeRoot;
            GameTreeNode child = parent.NextMove;

            while (child != null)
            {
                parent = child;
                child = parent.NextMove;
            }

            GameTreeNode newNode = new GameTreeNode(move);
            parent.Branches.AddNode(newNode);

            // Remember lastly added node and notify about game tree change
            LastNode = newNode;
        }
    }
}
