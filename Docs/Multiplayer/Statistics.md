# Statistics

_Relevant requirement:_

*   _14.5\. The app allows the user to view their statistics kept on online servers to which the user is logged in._

Both of our servers - IGS and KGS - associate each player with a rank. We display this rank to the user.

We update this rank:

*   On IGS, whenever we request a full list of users (because our rank is sent as part of this list)
*   On KGS, whenever we log in (our rank is sent as part of login info)

The servers store additional information about each player. Most notably:

*   **Game history.** The list of games the player has played, including opponent names and whether the game was a loss or a win.
*   **Rating graph.** How the player's ELO has evolved over time.

Currently, we do not download and display this data for these reasons:

*   **Interface.** It would require user interface elements that we did not design (tables and charts).
*   **IGS: Lack of known API.** I don't know of a way to get this information from IGS.

Here's how I would proceed, if in the future we want to add this functionality:

*   Use the Wireshark packet sniffing program to discover how the official client, GoPanda2, accesses the rating graph and document this.
*   Create a component for displaying a table of games and a line chart.
*   Add UI to either the Statistics screen or the lobby screens for the servers that would open these components.
*   Add code that whenever one of these components is opened, an API command is sent to the server and its result is used to populate the component.