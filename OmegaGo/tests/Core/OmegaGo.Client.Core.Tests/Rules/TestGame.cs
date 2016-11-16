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
        private int width;
        private int height;

        private readonly List<StoneColor[,]> _fullHistory = new List<StoneColor[,]>();

        public StoneColor[,] CurrentBoard => _fullHistory.Last();

        public List<StoneColor[,]> History => _fullHistory.Take(_fullHistory.Count - 1).ToList();

        public static TestGame New(int size)
        {
            TestGame game = new Rules.TestGame()
            {
                width = size,
                height = size
            };
            game._fullHistory.Add(new StoneColor[game.width, game.height]);
            return game;
        }
        public TestGame Play(string coordinates, Ruleset ruleset)
        {
            throw new NotImplementedException();
        }

        public TestGame Place(string coordinates, StoneColor color)
        {
            Position whereTo = Position.FromIgsCoordinates(coordinates);
            CurrentBoard[whereTo.X, whereTo.Y] = color;
            return this;
        }

        public Move Move(string coordinates, StoneColor player = StoneColor.Black)
        {
            return OmegaGo.Core.Move.Create(player, Position.FromIgsCoordinates(coordinates));
        }

    }
}
