using System.Collections.Generic;
using OmegaGo.Core.AI.Joker23.GameEngine;

namespace OmegaGo.Core.AI.Joker23.Players
{

    public abstract class JokerPlayer
    {

        protected int numPieces;    //number of pieces the player has
        protected char color;       //the color of the player (either W or B)
        protected LinkedList<JokerMove> moves; //moves list for this specific player

        public JokerPlayer(char color)
        {
            this.numPieces = 0;
            this.color = color; 
            this.moves = new LinkedList<JokerMove>();
        }

        /**planMove
         * this is the AI thing that plans the move for
         * the player
         */
        public abstract void planMove(JokerGame game);

        /** playMove
         * this method will allow the player to play a move on to the board
         *
         * @param row : the row tha the piece will be played on
         * @param col : the column that the piece will be played on
         */

        public bool makeMove(JokerGame game, JokerPoint point)
        {
            if (game.play(this.color, point.x, point.y))
            {
                moves.AddLast(new JokerMove(color, point));
                return true;
            }
            else
            {
                return false;
            }
        }

        /////////////getters and setters////////////////////////
        /**
         * player gains n many pieces
         */
        public void gain(int n)
        {
            this.numPieces += n;
        }

        /**
         * player loses n many pieces
         */
        public void lose(int n)
        {
            this.numPieces -= n;
        }

        public int getNumPieces()
        {
            return numPieces;
        }

        public char getColor()
        {
            return color;
        }

        public void setColor(char color)
        {
            this.color = color;
        }

        public char getOpponentColor()
        {
            if (this.color == 'W')
            {
                return 'B';
            }
            return 'W';
        }

        public LinkedList<JokerMove> getPreviousMoves()
        {
            return moves;
        }
    }
}
