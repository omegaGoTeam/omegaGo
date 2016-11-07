# IGS Protocol Specification
The **Internet Go Server** (IGS) was first put online in 1992 and used Telnet as a wire protocol and simple text commands. The first Go clients that connected to it provided some extra features to users but in the end, they still had to know and use commands.

As time went on and graphical clients developed, the Telnet interface was used less and less by common users, however all clients continued to use it, albeit hidden from the user. Originally, most or all IGS commands were documented in the interface itself, with the `help` command providing detailed information on syntax and meaning of commands.

Around 1995, IGS - originally developed at an American university - was acquired by a Japanese company and renamed **Pandanet** (sometimes *Pandanet: Internet Go Server*).

Eventually, when Pandanet created established clients for Windows, Mac, Linux, Android and iOS, they had no further need to document commands. They appear to now discourage the use of "non-official" clients and don't make effort to maintain documentation.

For this reason, I put together this specification, pieced together from help files remaining on the IGS server and from reverse-engineering the communication sent between the server and the official clients.

If you discover an error in this specification, please inform me at <petrhudecek2010@gmail.com>. Thank you.

This document will sometimes copy the help files of IGS verbatim. These will be given as a block of `non-proportional` text.

## General Concepts & Connection

### Host

To connect to the IGS server, connect via Telnet to one of these hosts:

* igs.joyjoy.net:6969
* igs.joyjoy.net:7777
* 210.155.158.200:6969
* 210.155.158.200:7777
* igs.joyjoy.net:28155

If one of the ports fail, try the other ones. Users, games and everything is shared between these ports. Users who connect to one of these ports will communicate normally with users on the other ports.

### Connection start
When you connect, the server will print a welcome message and the prompt "Login: ". You should respond with your username.
* If you send a username that doesn't exist, you will move to the main command loop, logged in a *guest account* with the name *guestXXXX* where XXXX is a number. Your typed username will be forgotten.
* If you send a username that exists, the server will respond either with "Password: " or with "1 1". The second response is the client-mode response. Reply with a password. An incorrect password will cause the login procedure to restart, but the client-mode setting will be remembered. A correct password will move you to the main command loop, logged in as the user.

Once in the main command loop, the server may send you lines of text at any moment and you may send commands to the server at any moment as well. There are few timing guarantees.

### Client mode

Each user account is associated with the state of several *toggles*, the most important of which is the client mode. If you're in client mode, the lines server sends to you will be a little different. Most importantly, they will be prefix with a "code". For example, the code "1" means "prompt" and the code "5" means "error".

Clients should generally always operate in client mode as it's easier to handle programatically. You must complete the first login procedure before you can switch to client mode, but you may login as guest also. During this first login procedure, the server doesn't explicitly tell you whether it's got you in client mode or not: it depends on what mode you were last time when using this account.

### Prompt

Whenever the server responds to one of your commands, this response will always be terminated by a *prompt* line. In client mode, this will start with the code "1". 

The server may also send you lines that you didn't ask for; for example, to inform you that a move was made, that somebody sent you a chat message, that the server will shut down or to share an ad with you. This interjections will also always be terminated by a prompt line.

The line then contains one more number which identifies what state you are in:

* 1 0 You must log in. Enter your username.
* 1 1 You must log in. Enter your password.
* 1 5 You are in the main command loop and aren't in any games.
* 1 6 You are playing a game.
* 1 8 You are observing a game.

### Setting up the environment
Shortly after logging in, you will likely want to set up the connection the way you want so the server sends you information in an understandable format.

The official desktop client, GoPanda2, sends the following series of commands upon login, for example:

* `toggle client on`
* `toggle quiet false`
* `toggle pairgo true`
* `toggle newundo true`
* `toggle singlegame false`
* `toggle review true`
* `toggle multiroom true`
* `toggle beep false`
* `toggle newrating true`
* `toggle seek true`
* `toggle nmatch true`
* `language EN`
* `id Gopanda2 2.5.1`
* `toggle open true`
* `toggle looking true`
* `room`
* `stored`
* `message`
* `review files`
* `friend start`
* `refuse list`
* `userlist2`
* `gamelist3`
* `keep 0`
* `join 0`
* `keep 0`

## Commands

### `toggle`
```
 Usage: toggle <option> [<value>]
 
 Toggle changes an option's 'value' such as  on/off, true/false. If given
 no 'value', toggle sets the specified option to the opposite of what the
 option currently is.

  Valid options:
  automail  Have IGS mail a copy of your game to your stats address.
  bell      If 'on' a bell will ring on redrawing boards and 'beep' messages.
  open      If 'on' then you are available to accept 'match' requests,
            otherwise you never know about them. If you are in a match,
            you will not receive match requests. Shows 'X' for off.
  looking   Means, "I really want to play". Shows a '!' under 'who'.
  quiet     If 'on' you will not see system messages about players logging
            in/out, and game results. Will not block 'shutdown' messages, or
            administrator shouts. Shows 'Q' for quiet on if shout is also off.
  verbose   If 'on' full boards will be sent to you, otherwise _only_ the
            last move coordinates will be sent.
  client    Sets IGS to transmit to a _client_. Implies: toggle verbose false
  chatter   Opens or blocks chatter while observing a game.
  kibitz    Opens or blocks kibitzes while observing a game.
  singlegame  Opens or closes multiple matches or multiple game requests.
  shout     Opens or blocks shouts, but not system messages. Shows 'S' for on
 Valid values: true, false, on, off, 0, 1;  case insensitive.
See also:  beep chatter client kibitz match rating shout stats who
```
So, for example, the first command you'll want to send after you log in is usually `toggle client on`. 

Not all toggles are described in this help file. Most notably, the `newundo` and `nmatch` toggles are missing. Here's some explanation on them:

Undocumented toggles:
* `nmatch`: Whether the client supports the `nmatch` command
* `newundo`: Whether the client supports the `undoplease` command
* `beep`: This is a synonym of `bell`.
### login
```
Usage:  login

    Login will put you back at the 'Login:' prompt, waiting for you
    to type an account name in again.
```
### quit
Synonym: `exit`

Terminates your Telnet connection.

```
Usage:  quit


        'quit' will exit you from IGS. If you are playing a game, 'quit'
        will attempt to 'save' your game.

           To leave IGS, enter:   quit
```
### games
```
Usage: games [game number]

 The 'games' command lists games currently in progress.  White is always
 shown first.  Entering  games  will list _all_ games in progress, but if
 a game _number_ is supplied, only that game will be listed.
        Example:  game 56

[##]  white name [ rk ]      black name [ rk ] (Move size H Komi BY FR) (###)
[56]       HUH00 [ 5d*] vs.       nomad [ 5d*] (224   19  0  0.5 12  I) ( 95)

 In the header, ## is the game number, followed by:
   white player, rank, black player, rank, number of moves played, board size
   handicap amount, komi value, byo-yomi period, flag, and the number of
   people observing the game.
 The  'F and R'  (FR) flags:
   If a game is a free game, there is a 'F' under the F column.
   If a game is a teaching game, there is a 'T' under the F column.
   If a game is a tournament game, there is a '*' under the F column.
   The type of game will be listed in the 'R' column.   The types of games
   are (I) for IGS Go games, (C) for chinese chess, (G) GOE Go games,
   (P) GOE Pro Go game <pmatch game>, and (S) for shogi
```
This is what a single game line will look like:
```
7 [ 4]       wari5 [ 6k*] vs.   Spinserve [ 6k*] (  0   19  0  0.5 10  I) (  0)
```

### `match`
Requests a match with a player or accepts match. The official client now uses `nmatch` instead which is more powerful and allows easier setting of handicaps and different timing systems. 

A client that doesn't support `nmatch` can request a game with an nmatch-capable client using the `match` command. However, the official GoPanda2 client only initiates game via `nmatch` and cannot initiate games against non-nmatch-capable clients, it seems.

```
Usage:  match <opponentname> [color] [board size] [time] [byoyomi minutes]
    'match' is for starting a game with an opponent. You can offer or decline
  a match request. Start a game with 'match', followed by the opponent's name,
  color you wish ( W or B ), board size, time (measured in minutes) for each
  player, and byoyomi minutes per player.  Example:   match ivy W 19 15 10
  If no boundaries are given, the default settings are:  board size = 19
  color = B, time = 90 minutes per player, byoyomi = 10 minutes per player.
      Example:   match ivy   (This is the same as:  match ivy B 19 90 10)
      Note:  match ivy B 19 0 0    would mean there are no time limits.
  The first move by B (Black) can be:  handicap #    (#) is the number of
  the handicap stones.  To place moves on the board, see:   help coords
   a) A game can be 'adjourned' if both players enter:   adjourn
   b) An 'adjourned' can be restarted with the 'load' command. See: help load
** MUST READ **   To 'score' or end a game, see:   help score
   ^^^^^^^^^
    IGS supports multiple games. You can play more than one game, but if
  you want to play only one game and not accept additional 'match' requests,
  there are 2 options.   (See:   help toggle)
    1)  While playing a game, type:   toggle singlegame  (toggles off or on)
    2)  While playing a game, type:   toggle open
```

### `nmatch`
`nmatch` is an undocumented command that requests or accepts a game.
```
5 Usage :
5 nmatch <oppname> <color(BWN)> <handicap> <boardsize(2-19)> <time(sec)> <byotime(sec)>
5 <byomoves(0-25)> <koryocount> <koryosec(sec)> <prebyoyomi(sec)>
```

TODO

### `observe`
Starts or ends the observation of a game. You may observe multiple games at a time.

```
Usage:  observe <game number>
   The 'observe' command is used to observe a game, or games, in progress.
   To see a games listing, enter:   games
   Then choose a game you wish to observe, then enter:  observe <game number>
        Example:  observe 56

   After you start observing a game, you will be listed in the 'who' list
   with a number next to your name under 'Info', as observing game <number>.

   To stop observing a game, enter the same command again:
        observe <game number you were observing>
        Example:  If you were observing game 56, and wanted to stop
                  observing game 56, enter:   observe 56
        Or, you can use:  unobserve

 Some clients have "time control" to compensate for "net lag". These clients
 are able to time a players move starting from when a move is made, not when
 the signal reaches IGS. In such cases the time, or clock, appears to jump
 backward as each correction is made. Some people misinterpret the time
 correction as cheating.
```
### `addtime`
```
Usage: addtime <time to be added>

     'addtime' is used by a player to add extra time to an opponent's clock.
     Added time is measured in minutes.

        Examples:   addtime 1  (adds 1 minute)
                    addtime 60 (adds 1 hour)

     When an player adds extra time to his opponent's time, 'addtime' will
     also enter a 'kibitz' in the game record (sgf) saying how much time
     was added to which player.
```
TODO what if you're playing multiple games??

## List of reply codes
In client mode, all lines sent by the server, except for when the server is sending a file, will begin with a code followed by a single space. This is the list of codes used by IGS.

This list may miss some reply codes: a full list is in OmegaGo's `IgsCode.cs` file. 

* 1 - Prompt
* 5 - Error Message
* 7 - Game Listing
* 8 - Help File Begins or Ends
* 9 - General Informational Message
* 14 - Message Listing

IGS may send one or more **files** in response to a command, usually in response to the `help` command. These files are different in that they are copied verbatim, as ASCII text, and the lines of these files do not begin with an IGS reply code. 

However, at the beginning and end of each sent file, there will be a line that contains a reply code (such as `8`, `9` or `14`) immediately followed by `File`, e.g. `14 File`.

## Additional sources

Some information on the protocol may also be found at these pages:
* http://web.archive.org/web/20050310114628/nngs.cosmic.org/help.html
* http://senseis.xmp.net/?IGS

