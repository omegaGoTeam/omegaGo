package GameEngine;

import java.util.HashSet;
import java.util.List;
import java.util.LinkedList;

import java.awt.Point;

public class Rules {

	public static boolean isMoveLegal(char color, int row, int col, char[][] board) {
		if (board[row][col] != '*') {
			return false;
		}


		return true;
	}

	public static boolean gameOver(char[][] board) {
		for(int i=0; i<board.length; i++) {
			for(int j=0; j<board[i].length; j++) {
				if(board[i][j] == '*') {
					return false;
				}
			}
		}

		return true;
	}

	public static String serializeBoardState(char[][] board) {

		String boardString = "";

		for(int i = 0; i < board.length; i++) {
			for(int j = 0; j < board[i].length; j++) {
				boardString = boardString + board[i][j];
			}
		}
		return boardString;
	}

	public static char[][] copyBoard(char[][] board) {

		char[][] boardCopy = new char[board.length][board[0].length];

		for(int i = 0; i < board.length; i++) {
			for(int j = 0; j < board[i].length; j++) {
				boardCopy[i][j] = board[i][j];
			}
		}
		return boardCopy;
	}

	public static char getWinner(char[][] board) {
		int w = 0;
		int b = 0;

		int n = board.length;

		for(int i=0; i<n; i++) {
			for(int j=0; j<n; j++) {
				if(board[i][j] == 'W') {
					w ++;
				}

				if(board[i][j] == 'B') {
					b ++;
				}
			}
		}

		if(w > b + 1) {
			return 'W'; //white wins
		} else if (b > w + 1) {
			return 'B'; //black wins
		} else {
			return 'N'; //no one wins
		}
	}

	private static boolean[][] vis;
	private static int[] dx = {1, -1 ,0 ,0};
	private static int[] dy = {0, 0, -1, 1};
	private static char[][] board;

	public static List<Point> findCaptured(char wall, char indanger, char[][] input) {
		vis = new boolean[input.length][input[0].length];
		board = input;

		for(int i=0; i<board.length; i++) {
			for(int j=0; j<board[i].length; j++) {
				if (board[i][j] == '*' && !vis[i][j]) {
					dfs(i, j, board.length, wall);
				}
			}
		}

		List<Point> ret = new LinkedList<Point>();
		for(int i=0; i<board.length; i++) {
			for(int j=0; j<board[i].length; j++) {
				if(board[i][j] == indanger && !vis[i][j]) {
					ret.add(new Point(i, j));
				}
			}
		}

		return ret;
	}

	private static void dfs (int row, int col, int n, char wall) {
		if(row < 0 || row >= n || col < 0 || col >= n || board[row][col] == wall || vis[row][col]) {
			return;
		}

		vis[row][col] = true;

		for(int i=0; i<dx.length; i++) {
			int r1 = row + dx[i];
			int c1 = col + dy[i];

			dfs(r1, c1, n, wall);
		}
	}
}
