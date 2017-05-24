package Players;

import java.awt.event.*;
import javax.swing.*;

import GameEngine.Game;
import util.*;

public class Human extends Player{

	protected Listener listener;
	protected boolean strapped;

	public Human(char color) {
		super(color);
		this.listener = new Listener(this);
		this.strapped = false;
	}

	public void planMove(Game game) {
		listener.setGame(game);
		game.getGameBoard().setMouseAdaptor(listener);
		setStrapped(true);
		while(strapped) {
			try {
				Thread.sleep(1);
			} catch(Exception e ) {
				e.printStackTrace();
			}
		}
	}

	public void setStrapped(boolean strapped) {
		this.strapped = strapped;
	}

	private class Listener extends MouseAdapter {

		private Game game;
		private Player human;

		public Listener(Player human) {
			this.human = human;
		}

		public void setGame(Game game) {
			this.game = game;
		}

		public void mousePressed(MouseEvent e) {
			int i = e.getX()/64; //TODO (future work) change this to variable
			int j = e.getY()/64;

			char color = human.getColor();
			if(!game.play(color, i, j)) {
				JOptionPane.showMessageDialog(null, "Invalid Move!");
			} else {
				game.getGameBoard().removeMouseAdaptor(this);
				setStrapped(false);
			}
		}
	}
}
