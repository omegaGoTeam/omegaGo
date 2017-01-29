using System.Collections.Generic;
using OmegaGo.Core.AI.Joker23.GameEngine;

// ReSharper disable All - this is imported code

namespace OmegaGo.Core.AI.Joker23.Players { 

public class AlphaBetaPlayer : JokerPlayer {

    private int[,] influence;
    private char[,] boardCopy;
    private JokerPoint bestMove;
    private int _maxDepth;

    private static int INF = int.MaxValue; //sweet sweet victory
    private static int[] dx = {1, -1, 0, 0};
    private static int[] dy = {0, 0, -1, 1};

    public AlphaBetaPlayer(char color) : base(color) {
    }

    public override void planMove(JokerGame game) {

        if (influence == null) {
            initInfluenceMap(game.getHeight());
        }

        boardCopy = game.getBoardCopy();
        this._maxDepth = game.getWidth() + 1;
        alphabeta(this._maxDepth, -INF, INF, getColor());

        makeMove(game, bestMove);
    }
        public JokerPoint betterPlanMove(JokerGame game, int maxDepth)
        {
            // Takes 95% CPU

            if (influence == null)
            {
                initInfluenceMap(game.getHeight());
            }

            boardCopy = game.getBoardCopy();
            this._maxDepth = maxDepth;

            alphabeta(this._maxDepth, -INF, INF, getColor());

            return bestMove;
        }

        private int alphabeta(int depth, int alpha, int beta, char turn) {
            // Takes 95% CPU
            int me = GameEngine.Rules.findCaptured(getColor(), getOpponentColor(), boardCopy, boardCopy.GetLength(0)).Count;
            int oppo = GameEngine.Rules.findCaptured(getOpponentColor(), getColor(), boardCopy, boardCopy.GetLength(0)).Count;

            if (oppo > 0 && me > 0) {
            if(turn == getColor()) {
                return -1000000000;
            } else {
                return 1000000000;
            }
        } else if (oppo + me > 0){
            if (oppo > 0) {
                return -1000000000;
            } else if (me > 0) {
                return 1000000000;
            }
        }

        if(depth == 0) {
            return AiUtil.getLiberties(getColor(), boardCopy) - AiUtil.getLiberties(getOpponentColor(), boardCopy);
        }
        //both player have the same next moves
        //List<Point> nextMoves = AiUtil.getNextMoves(boardCopy, influence, 10); //hard coded horizontal pruning

        

        IEnumerable<JokerPoint> nextMoves = AiUtil.getNextMoves(boardCopy, 3);
        if(turn == getColor()) {

            foreach(JokerPoint next in nextMoves) {
                int priorInf = influence[next.x,next.y];
                updateInfluenceMap(next.x, next.y, boardCopy.GetLength(0));
                boardCopy[next.x,next.y] = getColor();

                int temp = alphabeta(depth - 1, alpha, beta, getOpponentColor());

                boardCopy[next.x,next.y] = '*';
                restoreInfluenceMap(next.x, next.y, priorInf, boardCopy.GetLength(0));

                if(alpha < temp) {
                    alpha = temp;
                    if(depth == this._maxDepth) {
                        bestMove = next;
                    }
                }

                if(beta <= alpha) {
                    return alpha;
                }
            }

            return alpha;

        } else {

            foreach(JokerPoint next in nextMoves) {

                int priorInf = influence[next.x,next.y];
                updateInfluenceMap(next.x, next.y, boardCopy.GetLength(0));
                boardCopy[next.x,next.y] = getOpponentColor();

                int temp = alphabeta(depth - 1, alpha, beta, getColor());

                boardCopy[next.x,next.y] = '*';
                restoreInfluenceMap(next.x, next.y, priorInf, boardCopy.GetLength(0));

                if(beta > temp) {
                    beta = temp;
                    if(depth == this._maxDepth) {
                        bestMove = next;
                    }
                }

                if(beta <= alpha) {
                    return beta;
                }
            }

            return beta;
        }
    }


    private void initInfluenceMap(int n) {
        this.influence = new int[n,n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    influence[i, j] = 2;
                }
            }

        for(int i=0; i<n; i++) {
            influence[0,i] =
                influence[i,0] =
                influence[i,n-1] =
                influence[n-1,i] = 1;
        }

        influence[n/2,n/2] = 4;
    }

    private void updateInfluenceMap(int row, int col, int n) {
        influence[row,col] = 0;
        for(int i=0; i<dx.Length; i++) {
            int nr = row + dx[i];
            int nc = col + dy[i];

            if(nr < 0 || nr >= n || nc < 0 || nc >= n) {
                continue;
            }
            influence[nr,nc] *= 2;
        }
    }

    private void restoreInfluenceMap(int row, int col, int prev, int n) {
        influence[row,col] = prev;
        for(int i=0; i<dx.Length; i++) {
            int nr = row + dx[i];
            int nc = col + dy[i];

            if(nr < 0 || nr >= n || nc < 0 || nc >= n) {
                continue;
            }
            influence[nr,nc] /= 2;
        }
    }
}
}