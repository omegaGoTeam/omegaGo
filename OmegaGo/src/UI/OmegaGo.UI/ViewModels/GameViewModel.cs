using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private Game _game;

        public Game Game
        {
            get { return _game; }
        }

        public GameViewModel()
        {
            _game = new Game();
            _game.BoardSize = new GameBoardSize(19);
            
            GameTreeNode node1 = new GameTreeNode(Move.Create(StoneColor.White, new Position(5, 5)));
            GameTreeNode node2 = new GameTreeNode(Move.Create(StoneColor.Black, new Position(5, 5)));
            GameTreeNode node3 = new GameTreeNode(Move.Create(StoneColor.White, new Position(7, 5)));
            GameTreeNode node4 = new GameTreeNode(Move.Create(StoneColor.Black, new Position(6, 6)));
            GameTreeNode node5 = new GameTreeNode(Move.Create(StoneColor.White, new Position(5, 6)));

            _game.GameTree.GameTreeRoot = node1;

            node1.Branches.AddNode(node2);
            node2.Branches.AddNode(node3);
            node3.Branches.AddNode(node4);
            node4.Branches.AddNode(node5);
        }
    }
}
