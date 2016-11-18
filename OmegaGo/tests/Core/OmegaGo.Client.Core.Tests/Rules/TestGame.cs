using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Common;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Tests.Rules
{
    class TestGame
    {
        private int _width;
        private int _height;
        private Ruleset _ruleset;

        private readonly List<StoneColor[,]> _fullHistory = new List<StoneColor[,]>();

        public StoneColor[,] CurrentBoard => _fullHistory.Last();

        public List<StoneColor[,]> History => _fullHistory.Take(_fullHistory.Count - 1).ToList();

        public static TestGame New(int size, Ruleset ruleset = null)
        {
            TestGame game = new Rules.TestGame()
            {
                _width = size,
                _height = size,
                _ruleset = ruleset
            };
            game._fullHistory.Add(new StoneColor[game._width, game._height]);
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
            return OmegaGo.Core.Move.PlaceStone(player, Position.FromIgsCoordinates(coordinates));
        }

        public MoveResult IsLegal(string coordinates, StoneColor player)
        {
            return _ruleset.IsLegalMove(this.CurrentBoard,
                this.Move(coordinates, player),
                this.History);
        }
    }
}
