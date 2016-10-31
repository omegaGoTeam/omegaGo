package Players;

import java.awt.Point;
import java.util.*;

import GameEngine.Game;

public class RandomPlayer extends Player {

	private Random rand;

	public RandomPlayer(char color) {
		super(color);
		rand = new Random();
	}

	public void planMove(Game game) {
		char[][] board = game.getBoard();
		LinkedList<Point> list = new LinkedList<Point>();

		for(int i=0; i<board.length; i++) {
			for(int j=0; j<board[i].length; j++){
				if(board[i][j] == '*') {
					list.add(new Point(i, j));
				}
			}
		}

		Collections.shuffle(list);
		while(!list.isEmpty()) {
			if(makeMove(game, list.poll())) {
				return;
			}
		}
	}
}
