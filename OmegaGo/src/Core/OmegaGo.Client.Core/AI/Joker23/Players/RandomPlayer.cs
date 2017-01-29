using System.Collections.Generic;
using OmegaGo.Core.AI.Joker23.GameEngine;

namespace OmegaGo.Core.AI.Joker23.Players {
    public class RandomPlayer : JokerPlayer
    {

        private System.Random rand;

        public RandomPlayer(char color) : base(color)
        {
            rand = new System.Random();
        }

        public override void planMove(JokerGame game)
        {
            char[,] board = game.getBoard();
            List<JokerPoint> list = new List<JokerPoint>();

            for (int i = 0; i < game.getHeight(); i++)
            {
                for (int j = 0; j < game.getWidth(); j++)
                {
                    if (board[i, j] == '*')
                    {
                        list.Add(new JokerPoint(i, j));
                    }
                }
            }

            JokerPoint randomMove = list[rand.Next(list.Count)];

            while (!list.isEmpty())
            {
                if (makeMove(game, randomMove))
                    return;
            }
        }

        public JokerPoint makeMove(char[,] board, int width, int height)
        {
            List<JokerPoint> list = new List<JokerPoint>();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (board[i, j] == '*')
                    {
                        list.Add(new JokerPoint(i, j));
                    }
                }
            }

            if (list.Count == 0)
            {
                return new Joker23.JokerPoint(0, 0);
            }
            JokerPoint randomMove = list[rand.Next(list.Count)];
            return randomMove;
        }
    }
}
