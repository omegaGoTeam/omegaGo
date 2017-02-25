using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Tests.Rules
{
    class TestGame
    {
        private int _width;
        private int _height;
        private Ruleset _ruleset;

        private readonly List<GameBoard> _fullHistory = new List<GameBoard>();

        public GameBoard CurrentBoard => _fullHistory.Last();

        public GameBoard[] History => _fullHistory.Take(_fullHistory.Count - 1).ToArray();        

        public static TestGame New(int size, Ruleset ruleset = null)
        {
            TestGame game = new Rules.TestGame()
            {
                _width = size,
                _height = size,
                _ruleset = ruleset
            };
            game._fullHistory.Add(new GameBoard(new GameBoardSize(game._width, game._height)));
            return game;
        }
  
        public TestGame Place(string coordinates, StoneColor color)
        {
            Position whereTo = Position.FromIgsCoordinates(coordinates);
            CurrentBoard[whereTo.X, whereTo.Y] = color;
            return this;
        }

        public Move Move(string coordinates, StoneColor player)
        {
            return Core.Game.Move.PlaceStone(player, Position.FromIgsCoordinates(coordinates));
        }

        public MoveResult IsLegal(string coordinates, StoneColor player)
        {
            return _ruleset.IsLegalMove(Move(coordinates, player), History);
        }
    }
}
