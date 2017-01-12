/**
 * Game
 * this class will store the game state
 * each cell in the board will have one of 3 characters :
 * 	'*' means the cell is empty
 * 	'B' means the cell is occupied by a black piece
 * 	'W' means the cell is occupied by a white piece
 *
 * @author Steven Zhang
 */

using System;
using System.Collections.Generic;
using OmegaGo.Core.AI.Joker23.Players;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming
#pragma warning disable 219

namespace OmegaGo.Core.AI.Joker23.GameEngine {
    public class JokerGame {
        private static int turn;          // which turn it is

        protected int height;               // height
        protected int width;                // width
        public char[,] board;           // representation of the board
        public LinkedList<JokerMove> moves;  // list of moves
        protected HashSet<String> seenBoardStates;   // past board states
        protected JokerPlayer p1, p2;

        /** constructors **/

        /**
         * @param height,width : the board size nxm
         */
        public JokerGame(int height, int width, JokerPlayer p1, JokerPlayer p2) {
            turn = 0;
            this.p1 = p1;
            this.p2 = p2;
            this.height = height;
            this.width = width;
            this.moves = new LinkedList<JokerMove>();
            this.seenBoardStates = new HashSet<String>();
            

            board = new char[height, width];
            for (int i = 0; i < height; i++) {
                // The following means: Arrays.fill(board[i], '*');
                for (var j = 0; j < width; j++)
                {
                    board[i,j] = '*';
                }
            }

            // There was a to-do item here: "remove hardcode"
            //this.p1 = new Human('W');
            //this.p2 = new RandomPlayer('B');

        }

        public void playGame() {

            JokerPlayer winningPlayer = null;


            while (winningPlayer == null) {
                if ((turn & 1) == 0) {
                    winningPlayer = takeTurn(p1, p2);
                }
                else {
                    winningPlayer = takeTurn(p2, p1);
                }

            }

            String winningMessage = "";

            if (winningPlayer.getColor() == 'W') {
                winningMessage = "White wins!";
            }

            else {
                winningMessage = "Black wins!";
            }

        }

        private JokerPlayer takeTurn(JokerPlayer activePlayer, JokerPlayer opposingPlayer) {

            activePlayer.planMove(this);
            activePlayer.gain(1);
            

            if (makeCaptures(activePlayer, opposingPlayer)) {
                return activePlayer;
            }

            if (makeCaptures(opposingPlayer, activePlayer)) {
                return opposingPlayer;
            }

            return null;

        }

        private bool makeCaptures(JokerPlayer capturingPlayer, JokerPlayer gettingCapturedPlayer) {

            LinkedList<JokerPoint> toRemove = Rules.findCaptured(capturingPlayer.getColor(), gettingCapturedPlayer.getColor(), board, height);

            if (toRemove.isEmpty()) {
                return false;
            }

            while (!toRemove.isEmpty()) {

                JokerPoint tmp = toRemove.First.Value;
                toRemove.RemoveFirst();

                board[tmp.x,tmp.y] = '*';

            }

            return true;
        }

        /**
         * places a piece on the board
         *
         * @param color : the color of the piece must be 'W' or 'B'
         * @param row,col : location of the piece to be placed 0 indexed
         *
         * @return boolean whether the piece is actually placed
         */
        public bool play(char color, int row, int col) {

            // sanity checks
            if (color != 'B' && color != 'W') {
                return false; //not a valid color
            }

            if (row < 0 || row >= height || col < 0 || col >= width) {
                return false; //not a valid coordinate
            }

            if (!Rules.isMoveLegal(color, row, col, board)) {
                // Occupation check only
                // No suicide check!
                return false;
            }

            char[,] boardCopy = Rules.copyBoard(board, height, width);

            boardCopy[row,col] = color;

            if (seenBoardStates.Contains(Rules.serializeBoardState(boardCopy, height, width))) {
                // Superko check
                return false;
            }

            board[row,col] = color;

            seenBoardStates.Add(Rules.serializeBoardState(board, height, width));

            moves.AddLast(new JokerMove(color, new JokerPoint(row, col)));

            turn++;

            return true;
        }

        /** getters and setters **/
        public int getHeight() {
            return height;
        }

        public int getWidth() {
            return width;
        }

        public LinkedList<JokerMove> getGamePlay() {
            return moves;
        }

        public char[,] getBoard() {
            return board;
        }

        public char[,] getBoardCopy() {
            char[,] temp = new char[height,width];

            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    temp[i,j] = board[i,j];
                }
            }

            return temp;
        }

        public JokerMove getLastMove() {
            if (moves.isEmpty()) {
                return null;
            }
            return moves.Last.Value;
        }

        public int getTurn() {
            return (turn & 1);
        }
    }

}