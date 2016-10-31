package Players;

import java.util.*;
/**
 * add your AI implementation to this class
 * so that you can access it in the game
 */
public class PlayerFactory {

	private HashMap<String, Player> playerMap; // a map that maps the player name to the AI class

	public PlayerFactory(char color) {
		playerMap = new HashMap<String, Player>();

		//YOU MAY ADD TO THIS SECTION
		playerMap.put("Human", new Human(color)); //instantiates as a not real color
		playerMap.put("The Fish", new RandomPlayer(color)); // random AI for testing!
		playerMap.put("The Puppy", new HueristicPlayer(color)); // a player that is driven by heuristics
		playerMap.put("Fluffy", new AlphaBetaPlayer(color)); //
	}

	/**
	 * @return string array of the player names
	 */
	public String[] getPlayerNames() {
		String[] ret = new String[playerMap.size()];

		int i = 0;
		for(String name : playerMap.keySet()) {
			ret[i++] = name;
		}

		return ret;
	}

	public Player get(String name) {
		return playerMap.get(name);
	}
}
