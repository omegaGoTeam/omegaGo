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
            _game.GameTree.GameTreeRoot =
                new GameTreeNode(new Move() { Coordinates = new Position(5, 5), WhoMoves = Color.White },
                new GameTreeNode(new Move() { Coordinates = new Position(6, 5), WhoMoves = Color.Black },
                new GameTreeNode(new Move() { Coordinates = new Position(7, 5), WhoMoves = Color.White },
                new GameTreeNode(new Move() { Coordinates = new Position(6, 6), WhoMoves = Color.Black },
                new GameTreeNode(new Move() { Coordinates = new Position(5, 6), WhoMoves = Color.White },
                null)))));
            _game.GameTree.GameTreeRoot.UpdateParents();
        }
    }
}
