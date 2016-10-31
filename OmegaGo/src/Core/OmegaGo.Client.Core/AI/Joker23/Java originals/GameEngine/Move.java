package GameEngine;

import java.awt.Point;
/**
 * Move
 * this is a representation of a move made by a player
 *
 * @author Steven Zhang
 */

public final class Move {
	private char color; 		// this is the color of the piece
	private Point location;		// this is the placement of the piece

	public Move(char color, Point location) {
		this.color = color;
		this.location = location;
	}

	public char getColor() {
		return color;
	}

	public void setColor(char color) {
		this.color = color;
	}

	public Point getLocation() {
		return location;
	}

	public void setLocation() {
		this.location = location;
	}

	public String toString() {
		return "(" + location.x + ", " + location.y + ")";
	}
}

