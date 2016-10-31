package Players;

import java.util.*;
import java.awt.Point;

import GameEngine.Game;
import GameEngine.Rules;
import util.AiUtil;

public class AlphaBetaPlayer extends Player {

	private int[][] influence;
	private char[][] boardCopy;
	private Point bestMove;
	private int maxDepth;

	private final static int INF = Integer.MAX_VALUE; //sweet sweet victory
	private final static int[] dx = {1, -1, 0, 0};
	private final static int[] dy = {0, 0, -1, 1};

	public AlphaBetaPlayer(char color) {
		super(color);
	}

	public void planMove(Game game) {

		if (influence == null) {
			initInfluenceMap(game.getHeight());
		}

		boardCopy = game.getBoardCopy();
		maxDepth = boardCopy.length + 1;

		alphabeta(maxDepth, -INF, INF, getColor());

		makeMove(game, bestMove);
	}

	private int alphabeta(int depth, int alpha, int beta, char turn) {

		int me = Rules.findCaptured(getColor(), getOpponentColor(), boardCopy).size();
		int oppo = Rules.findCaptured(getOpponentColor(), getColor(), boardCopy).size();

		if(oppo > 0 && me > 0) {
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

		List<Point> nextMoves = AiUtil.getNextMoves(boardCopy, 3);
		if(turn == getColor()) {

			for(Point next : nextMoves) {
				int priorInf = influence[next.x][next.y];
				updateInfluenceMap(next.x, next.y, boardCopy.length);
				boardCopy[next.x][next.y] = getColor();

				int temp = alphabeta(depth - 1, alpha, beta, getOpponentColor());

				boardCopy[next.x][next.y] = '*';
				restoreInfluenceMap(next.x, next.y, priorInf, boardCopy.length);

				if(alpha < temp) {
					alpha = temp;
					if(depth == maxDepth) {
						bestMove = next;
					}
				}

				if(beta <= alpha) {
					return alpha;
				}
			}

			return alpha;

		} else {

			for(Point next : nextMoves) {

				int priorInf = influence[next.x][next.y];
				updateInfluenceMap(next.x, next.y, boardCopy.length);
				boardCopy[next.x][next.y] = getOpponentColor();

				int temp = alphabeta(depth - 1, alpha, beta, getColor());

				boardCopy[next.x][next.y] = '*';
				restoreInfluenceMap(next.x, next.y, priorInf, boardCopy.length);

				if(beta > temp) {
					beta = temp;
					if(depth == maxDepth) {
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
		this.influence = new int[n][n];

		for(int i=0; i<n; i++){
			Arrays.fill(influence[i], 2);
		}

		for(int i=0; i<n; i++) {
			influence[0][i] =
				influence[i][0] =
				influence[i][n-1] =
				influence[n-1][i] = 1;
		}

		influence[n/2][n/2] = 4;
	}

	private void updateInfluenceMap(int row, int col, int n) {
		influence[row][col] = 0;
		for(int i=0; i<dx.length; i++) {
			int nr = row + dx[i];
			int nc = col + dy[i];

			if(nr < 0 || nr >= n || nc < 0 || nc >= n) {
				continue;
			}
			influence[nr][nc] *= 2;
		}
	}

	private void restoreInfluenceMap(int row, int col, int prev, int n) {
		influence[row][col] = prev;
		for(int i=0; i<dx.length; i++) {
			int nr = row + dx[i];
			int nc = col + dy[i];

			if(nr < 0 || nr >= n || nc < 0 || nc >= n) {
				continue;
			}
			influence[nr][nc] /= 2;
		}
	}
}
