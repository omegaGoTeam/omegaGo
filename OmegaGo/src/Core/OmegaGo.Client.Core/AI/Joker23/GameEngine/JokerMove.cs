
namespace OmegaGo.Core.AI.Joker23.GameEngine
{
    /**
     * Move
     * this is a representation of a move made by a player
     *
     * @author Steven Zhang
     */

    public class JokerMove {
        private char color;         // this is the color of the piece
        private JokerPoint location;     // this is the placement of the piece

        public JokerMove(char color, JokerPoint location) {
            this.color = color;
            this.location = location;
        }

        public char getColor() {
            return color;
        }

        public void setColor(char color) {
            this.color = color;
        }

        public JokerPoint getLocation() {
            return location;
        }

        public void setLocation(JokerPoint location) {
            this.location = location;
        }

        public string toString() {
            return "(" + location.x + ", " + location.y + ")";
        }
    }


}