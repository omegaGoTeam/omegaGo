package Players;

import java.util.List;
import java.util.LinkedList;
import java.awt.Point;

import GameEngine.Game;
import GameEngine.Move;

public abstract class Player {

	protected int 	numPieces;	//number of pieces the player has
	protected char	color;		//the color of the player (either W or B)
	protected LinkedList<Move> moves; //moves list for this specific player

	public Player(char color) {
		this.numPieces = 0;
		this.color = color; //TODO (future work) throw exception if we don't have the right color
		this.moves = new LinkedList<Move>();
	}

	/**planMove
	 * this is the AI thing that plans the move for
	 * the player
	 */
	public abstract void planMove(Game game);

	/** playMove
	 * this method will allow the player to play a move on to the board
	 *
	 * @param row : the row tha the piece will be played on
	 * @param col : the column that the piece will be played on
	 */

	public boolean makeMove(Game game, Point point) {
		if(game.play(this.color, point.x, point.y)) {
			moves.add(new Move(color, point));
			return true;
		} else {
			return false;
		}
	}

	/////////////getters and setters////////////////////////
	/**
	 * player gains n many pieces
	 */
	public void gain(int n) {
		this.numPieces += n;
	}

	/**
	 * player loses n many pieces
	 */
	public void lose(int n) {
		this.numPieces -= n;
	}

	public int getNumPieces() {
		return numPieces;
	}

	public char getColor() {
		return color;
	}

	public void setColor(char color) {
		this.color = color;
	}

	public char getOpponentColor() {
		if(this.color == 'W') {
			return 'B';
		} return 'W';
	}

	public List<Move> getPreviousMoves() {
		return moves;
	}
}
