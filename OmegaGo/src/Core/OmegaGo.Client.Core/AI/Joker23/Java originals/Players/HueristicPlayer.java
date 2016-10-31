package Players;

import java.util.*;
import java.awt.Point;

import GameEngine.Game;
import GameEngine.Move;
import util.AiUtil;

public class HueristicPlayer extends Player {

	private int[][] orderMap;
	private int[] dx = {1, -1, 0, 0};
	private int[] dy = {0, 0, -1, 1};
	public HueristicPlayer(char color) {
		super(color);
	}

	public void planMove(Game game) {
		if(orderMap == null) {
			initOrderMap(game.getHeight());
		}

		int n = game.getHeight();

		Move prevMove;
		if((prevMove = game.getLastMove()) != null) {
			int row = prevMove.getLocation().x;
			int col = prevMove.getLocation().y;

			updateOrderMap(row, col, n);
		}

		char[][] tmpBoard = game.getBoardCopy();
		int best = -Integer.MIN_VALUE;
		LinkedList<Point> bestMoves = new LinkedList<Point>();
		for(int i=0; i<n; i++) {
			for(int j=0; j<n; j++ ){
				if(tmpBoard[i][j] != '*') {
					continue;
				}

				tmpBoard[i][j] = color;
				int heuristic = AiUtil.getLiberties(getColor(), tmpBoard);
				heuristic -= (AiUtil.getLiberties(getOpponentColor(), tmpBoard) * 2);
				heuristic += orderMap[i][j];

				if(heuristic > best) {
					best = heuristic;
					bestMoves.clear();
					bestMoves.add(new Point(i, j));
				} else if (heuristic == best) {
					bestMoves.add(new Point(i, j));
				}

				tmpBoard[i][j] = '*';
			}
		}

		Collections.shuffle(bestMoves);
		Point bestMove = bestMoves.poll();

		updateOrderMap(bestMove.x, bestMove.y, n);
		makeMove(game, bestMove);
	}

	private void updateOrderMap(int row, int col, int n) {
		orderMap[row][col] = 0;
		for(int i=0; i<dx.length; i++) {
			int nr = row + dx[i];
			int nc = col + dy[i];

			if(nr < 0 || nr >= n || nc < 0 || nc >= n) {
				continue;
			}
			orderMap[nr][nc] *= 2;
		}
	}

	private void initOrderMap(int n) {
		this.orderMap = new int[n][n];

		for(int i=0; i<n; i++){
			Arrays.fill(orderMap[i], 3);
		}

		for(int i=0; i<n; i++) {
			orderMap[0][i] =
				orderMap[i][0] =
				orderMap[i][n-1] =
				orderMap[n-1][i] = 1;
		}
	}

	private void print(int[][] map) {
		for(int i=0; i<map.length; i++) {
			for(int j=0; j<map.length; j++) {
				System.out.print("[" + map[i][j] + "]");
			}System.out.println();
		}System.out.println();
	}
}
