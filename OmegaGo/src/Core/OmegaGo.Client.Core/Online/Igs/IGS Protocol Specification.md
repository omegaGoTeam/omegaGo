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

### Only one connection at a time
When you login with an account that is already logged in to IGS, the older connection will be immediately terminated.

The new connection will receive the message
```
9 Throwing other copy out.
```
immediately preceding the message-of-the-day greetings file.

### Client mode

Each user account is associated with the state of several *toggles*, the most important of which is the client mode. If you're in client mode, the lines server sends to you will be a little different. Most importantly, they will be prefix with a "code". For example, the code "1" means "prompt" and the code "5" means "error".

Clients should generally always operate in client mode as it's easier to handle programatically. You must complete the first login procedure before you can switch to client mode, but you may login as guest also. During this first login procedure, the server doesn't explicitly tell you whether it's got you in client mode or not: it depends on what mode you were last time when using this account.

### Prompt

Whenever the server responds to one of your commands, this response will always be terminated by a *prompt* line. In client mode, this will start with the code "1". 

**However** some commands will **not receive any response**. This also means that no prompt will be returned after these commands. This specification will tell you when a command does not receive any response (as opposed to the empty string response, which is terminated by a prompt line).

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
There are more commands. However, this documentation may omit the commands that the OmegaGo application does not use.

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

The most important toggles are:
* `quiet`: Set this to `off` to receive messages whenever a user logs in, logs out, or a game starts or is completed.
* `newundo`: Set this to `on` to be able to receive `undoplease` requests.
* `nmatch`: Set this to `on` to show a more precise resignation message and allow `nmatch` matches. These matches have more options, especially as regards timing. However, not all clients support `nmatch`.
* `client`: Set this to `on` to set several settings friendly to computers, unfriendly to users.
* `verbose`: Set this to `off` to hide userfriendly baords.

### user
```
Usage: user [country][rank (or rating)][rank (or rating) range][player name]
 For the complete listing of players, enter:   user
 'user' lists players by:
    1) Country:      Example:  user Korea  (or user korea, user kr [email])
    2) Rank:         Example:  user 4k  (4 kyu)
    3) Rank range:   Example:  user 4k-3d  (4 Kyu - 3 Dan)
                     Example:  user 4k-3d  o (4k - 3 Dan open for matches)
    4) Player name:  Example:  user till
 Example:  user korea
 (The country is determined by the email address, and cannot be set otherwise)
Name        Info            Country  Rank Won/Lost Obs  Pl Idle Flags Language
     taeha  1/10 1/12 game  Korea     2d   35/  26  -   34   2s    -- default
    figaro                  Korea     3k*   9/   3   1  35  52s    -- default

 Name = player's name, Info = player's 'info', Country = where the player is
 from, Rank =  rank, Won = games won, Lost = games lost, Obs = game observed,
 Pl = game number of the game played, Idle = idle time, Flags (See Flags),
 Language = language (supported languages are: korean and default {english})
    The Flags are:  Q, S, X, and !
      Q = toggle quiet on (system messages are not seen)
      S = toggle shout off (shouts not seen, except administrator shouts)
      X = toggle open off (player not accepting match requests)
      ! = toggle looking on (player actively seeking a match)
See also:  info language rank rank rating stats toggle who
```
The command's output is:
```
42 Name        Info            Country  Rank Won/Lost Obs  Pl Idle Flags Language
42   isfadm02  <None>          Japan     2k    0/   0  -   -   25s    QX default
42    livegw9  <None>          Japan     NR    0/   0  -   -    1m    QX default
42   livegw12  <None>          Japan     NR    0/   0  -   -   39s    QX default
42   livegw13  <None>          Japan     NR    0/   0  -   -   36s    QX default
42   livegw11  <None>          Japan     NR    0/   0  -   -   48s    QX default
42 supurinter  <none>          Japan     9k* 2788/2844  -   -   55s    QX default
42   AutoDone  <None>          Japan     2k    0/   0  -   -    2s    -X default
42     zz0008  <None>          Japan     NR    0/   0  -   -    2m    -X default
...
42      Wotan                  USA       3k    2/   2  -   -   11s    QX default
42     NoGo16  Fair play       Germany  11k* 125/  99  -   -    0s    -! default
9                 ******** 823 Players 151 Total Games ********
1 5
```
At least one space character is always present between column. Name is ASCII. Info may contain
Chinese character or other non-ASCII symbols.

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

### tell
Usage: `tell [username] [message of multiple words]`
```
Usage: tell <person> <message to the person>
 'tell' sends your message to another person, and is used for conversation.
 One line messages are the limit.
  1) 'tell' may be used to start a conversation.  tweet would like to talk
      to beam, so tweet uses the 'tell' command.
        Example:  #> tell beam Hi.
     a) After starting a 'tell', you can continue talking to the same person
        by using:   tell .    In other words, 'person' can be reduced to:  .
        Example:  #> tell . Like to play a game?
     b) _or_, this (tell .) can also be shortened to just:   .
        Example:  #> . Like to play a game?
     In case (a) and (b), beam (the recipient of the 'tell') will see:
        *tweet*:  Like to play a game?
  2) beam can respond to tweet with:  tell tweet hello
     or beam can respond with a shortcut to tweet, by using:  ^
        Example:  #> tell ^ hello
        This will work only when responding for the first time.
  3) To 'tell' to the last person entering IGS in the 'who' list, use:  $
        Example:  #> tell $ hello
  4) To 'tell' to both players in a game, use:  tell #<and game number>
        Example:  #> tell #23 Hi tim and fmc. Interesting game.  :-)
See also:  message say shout yell
```
The `tell` command may be used outside games. `tell` messages are not remembered by the server. 

The recipient will receive a message like this:
```
24 *Soothie*: message contents
```

And you will receive either
```
9 Setting your '.' to Soothie
```
or
```
40 Soothie
```
Both mean the same thing (your _dot string_ is now "Soothie") but this is not something you need in a client.

You may also receive
```
5 Cannot find recipient.
```
if the recipient does not exist or is not online.
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

Initiating a match will output something like this to the requester:
```
9 Requesting match in 75 min with OmegaGo2 as White.
```
and something like this to the target:
```
9 Match[19x19] in 75 minutes requested with Soothie as Black.
9 Use <match Soothie W 19 75 0> or <decline Soothie> to respond.
2 \7
```
A match request may fail, for several reasons, for example:
```
> match gagh
5 gagh is not logged on.
```
or
```
> match OmegaGo2
5 That player is currently not accepting matches.
```
These responses will always contain an error message with the reply code 5.

You may also use `match` to accept a match request. If you use `match [playername]` on a player who has an active match request towards you, that request will be accepted.

If you add additional parameters to the `match` command, intending to accept, then if those parameters match the parameters of the match request, the match request will be accepted. If they don't match, a _mismatch of request_ occurred (see further down in this specification for what happens then).

### `decline`
```
Usage:  decline <playername>

        'decline' will refuse a match, after one is offered.

         For example, if a 'match' request is offered, it will look like:

            Match[19x19] in 75 minutes requested with tim as White.
            Use <match  tim B 19 75 10> or <decline  tim> to respond

         If you wish to decline, enter:   decline tim

         If you wish to accept, enter:    match tim B 19 75 10
                                   or:    match tim
```
Used to decline refuse a `match` or an `nmatch` match request.

### `nmatch`
`nmatch` is an undocumented command that requests or accepts a game.
```
5 Usage :
5 nmatch <oppname> <color(BWN)> <handicap> <boardsize(2-19)> <time(sec)> <byotime(sec)>
5 <byomoves(0-25)> <koryocount> <koryosec(sec)> <prebyoyomi(sec)>
```
All arguments are mandatory.

Seconds must be always a multiple of 60. If Canadian timing is used, put `0` into the three last arguments. `N` means `nigiri` (choose color automatically).

Timing arguments are 
* `maintime`
* `byotime`
* `byomoves`
* `koryocount`
* `koryosec`
* `prebyoyomi`

You cannot set a handicap if N is chosen as a color.

Possible time systems are:
* ABSOLUTE: set `maintime` and put 0 in everything else
* CANADIAN: set `maintime`, `byotime` and `bymoves` and put 0 in everything else
* ADVANCED JAPANESE: set `maintime`, `byotime` as the grace period each move before overtime starts to be consumed, `byomoves` to `1`, `koryocount` to number of overtime periods and `koryosec` to length of overtime periods. These need not be a mulitple of 60.
* BRONSTEIN: set `maintime`, `prebyoyomi` to the number of seconds each player gets before time starts to subtract from the main time; put 0 in everything else

In theory, you can combine the Bronstein `prebyoyomi` with other time controls, but other clients may not support this.

If your opponent doesn't have the `nmatch` toggle on, you will receive 

```
5 Opponent's client does not support nmatch.
```

If you receive an nmatch request, you will receive
```
NMatch requested with MORAL70(B 4 19 60 600 25 0 0 0).
9 Use <nmatch MORAL70 B 4 19 60 600 25 0 0 0> or <decline MORAL70> to respond.
```

And then you may receive
```
24 *SYSTEM*: MORAL70 canceled the nmatch request.
```

An `nmatch` is declined the same way a normal match is declined and results in the same answer for the other player.

A mismatch in accepting will result in this:
```
5 There is a dispute regarding your match(nmatch):
5 OmegaGo1 request: B 0 19 60 600 25 0 0 0
5 Soothie request: B 0 19 60 600 25 0 0 0
```
and both match requests will remain active.

During an `nmatch`, the game heading (before each move) contains the following three lines in addition, so that, in total, this is sent:

```
15 Game 475 I: OmegaGo1 (0 58 -1) vs Soothie (0 19 -1)
15 TIME:475:OmegaGo1(W): 0 58/60 0/600 25/25 0/0 0/0 0/0
15 TIME:475:Soothie(B): 0 19/60 0/600 25/25 0/0 0/0 0/0
15 GAMERPROPS:475: 19 0 6.50
15   1(W): B4
```

The information in the `TIME` lines is, in order:
`15`, `TIME`, game number, player name, player color, `0` if in main time or `1` if in overtime, main time remaining, byoyomi time remaining, stones remaining in period, ???, ???, ??? (koryo information). TODO

The `GAMERPROPS` information is game number, board size, handicap stones, komi.

Otherwise, an `nmatch` is just like a `match`.


### `undoplease`
A player may send `undoplease [game number]` to request the opponent to undo a move.

You will receive either the reply
```
5 Opponent's client does not support undoplease.
```
if your opponent's `newundo` toggle is off, or
```
9 Requesting undo.
```
otherwise.

The opponent will in that case receive the server-initiated message:
```
24 *SYSTEM*: OmegaGo1 requests undo.
1 6
1 6
```
**Warning:** Due to an IGS bug, this notification sends a **double prompt message**.

The opponent may accept or refuse the undo by using `undo` or `noundo`.

### `undo`
If you agree to an `undoplease` by `undo [gamenumber]`, then you will either receive an error message or no response, in which case the following message will be sent to both players:
```
28 Soothie undid the last move (B3) .
28 Soothie undid the last move (B2) .

15 Game 44 I: OmegaGo1 (0 4493 -1) vs Soothie (0 4500 -1)
```
Up to 2 moves might be undone with this, because `undo` will always undo all most recent moves up to the latest move made by the player who requested the `undo`.

Spectators to the match will see something like this:
```
28 Undo in game 233: OmegaGo1 vs OmegaGo2:   A5
1 8

28 Undo in game 233: OmegaGo1 vs OmegaGo2:   A4
1 8
15 Game 233 I: OmegaGo1 (0 4491 -1) vs OmegaGo2 (0 4480 -1)
15   1(W): A3

1 8
```

### `noundo`
If you respond to an undo request with `noundo [gamenumber]`, the undo request will be denied. The undo request will not receive any response, but the message
```
9 Soothie declines undo.
```
will be sent to both players. The game will also become a _no-undo_ game and further `undoplease` requests will be denied automatically.

### `say`
Usage (single game): `say [message]`<br>
Usage (multiple games): `say [gamenumber] message`

```
Usage: say message

       'say' can be used by players to send messages to each other instead of
       using the 'tell' command. It can only be used by players in a 'match'.
       'say' messages have the same format as the 'tell' command.

       Messages sent to the opponent with 'say' are recorded in the game
       record. This is useful if you wish to save notes in the game record.
       'tell' does not save comments in the game record.

               Example:  say This is a good move.

       'say' can be shortened to:   ,
               Example:  , This is a good move.

       During the game, the say will look like:
              *player*:  This is a good move.

See also:  chatter kibitz sgf sgfviewing shout tell yell
```
The server will respond with empty string followed by the prompt.

The server will send the other player in the game a message like this:
```
51 Say in game 116

19 *Soothie*: a
```
You will receive an error message if you're not in the game.

If you are in multiple games, you must put the game number after `say`. If you are in a single game, you SHOULD NOT put the game number after `say` (it would count as part of the message and would be sent to the other player as text).


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
### `ayt`
The "Are You There" command will always result in the answer:
```
9 yes
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

If you're playing multiple games, or even if you're playing a single game, you may use:

```
addtime [gamenumber] [minutestobeadded]
```
You will not receive a response.
However, both players will receive the response 
```
9 Increase OmegaGo1's time by 10 minutes
1 6
```
If the `addtime` command fails, perhaps because you used an invalid game number, you will receive an empty response.
### `time`
```
Usage: time [game number]

    'time' will display how much time is left in a game. If a game is in
    'byoyomi', a "(B)" will be displayed after the time, plus the number
    of moves left to play. To find out how much time is left in a
    _particular_ game, enter:   time <game number>
          Example:   time 42
          An example of what 'time 42' will display:
             Game : 42
             White(ivy) : 8:49 (B) 19
             Black(tim) : 4:04 (B) 15
    If you are playing a game, you can enter 'time' without the game
    number. This will also update the board, and time.  Example:   time
    If you are observing a game, 'time', without the game number, will
    display the time info on that game and all other games being observed.

 NOTE:  'refresh' (or 'moves') will end games in overtime (minus time),
         see:  help refresh  (or  help moves)
 Some clients have "time control" to compensate for "net lag". These clients
 are able to time a players move starting from when a move is made, not when
 the signal reaches IGS. In such cases the time, or clock, appears to jump
 backward as each correction is made.
```
The response will be like this:
```
26 Game : 28
26 White(OmegaGo1) : 124:58
26 Black(Soothie) : 71:39
1 6
```
### making a move
```
Usage: <letter><number>

   The coordinate system used by IGS is [A - Z][1 - 25], depending on the
   board size. The letter 'I' is not used. The maximun board size is 19.
   Spaces are not allowed in the move coordinates.
     Example:  On a 19 x 19 board, the lower left point is:   A1
                                   the upper right point is:  T19
   See:  help goboard

   The first move by B (Black) can be:  handicap #    (#) is the number of
   handicap stones.  (#) must be between 2 - 9     Example:   handicap 5

   To pass, enter:   pass        To undo, enter:   undo

   At the end of a game, dead stones are removed while scoring by entering
   the coordinates of a dead stone or groups of dead stones.

   Most 'client' programs will allow the use of a 'mouse' to click on the
   desired coordinate, instead of using the keyboard to enter moves.

See also: goboard chinesechess choice CC client match team shogi undo
```
To make a move, enter `[coordinates] [gamenumber]`.
You will receive an error message if the move is illegal, or both players will receive:
```
15 Game 38 I: OmegaGo1 (0 4500 -1) vs Soothie (0 4495 -1)
15   0(B): B2
```
The index of the first move is `0`. The server may also decide that there's a handicap, in which case the first move is made automatically, and it's, for example, `Handicap 2` instead of `B2`. Handicap stones are placed as per Japanese rules (fixed placement). Handicap only works for boards of 9x9, 13x13 and 19x19, I think.

The numbers in the heading after player names signify remaining time.
```
15 Game 28 I: OmegaGo1 (0 7498 -1) vs Soothie (0 4276 -1)
```
means: 7498 seconds of main time left for OmegaGo1 and 4276 seconds of main time left for Soothie

The first number in the parentheses indicates the number of prisoners the player has.

If you are in Canadian overtime, instead the line might look like this:
```
15 Game 631 I: OmegaGo1 (0 617 24) vs Soothie (0 649 22)
```
which means that in this period, Soothie still has to make 22 moves within 649 seconds.

When you first enter Canadian overtime, both players will receive
```
9 The player OmegaGo1 is now in byo-yomi.
9 You have 24 stones and 11 minutes
```
in the message that sends the first move in byo-yomi.

In response to a move, you may also receive
```
9 Soothie has run out of time.
1 6
```
which ends the game (both players will receive this).


## Match procedure
When two players play Go on IGS, the following sequence happens:

* One player sends an invitation to the other player.
* Second player reacts to it:
   * If they accept, then procedure proceeds.
   * If they reject, then procedure ends.
   * If they respond with a different set of parameters, an informative message saying so is sent to both players, and both the match request and the acceptance are treated as ongoing active match requests, but neither is accepted. Either player may accept the other's match request by accepting _their_ parameters.
* Game starts and starting information is sent to both players. The starting player must make a move within 60 cards or lose the game. This loss is repoted to both players thus:
```
9 OmegaGo1 has run out of time.
1 6
9 OmegaGo1 lost the game 88 due to no-greeting. move 0 points.

9 Removed game file Soothie-OmegaGo1 from database.
1 5
```
or thus, if a chat message was sent:
```
9 OmegaGo1 has run out of time.
1 6
9 OmegaGo1 lost the game 88 due to no-move. move 0 points.

9 Removed game file Soothie-OmegaGo1 from database.
1 5
```
* The second player must respond with a move within the next 60 seconds after the first move or lose to no-greeting.
* The first move of each player also causes their opponent to receive a `say` message with the contents `Hi!` as though it was sent by the other player.
* Players now make moves as detailed above in the specification.
* When a `pass` is sent three times consecutively (i.e. one player must send it twice), the game moves to the Life/Death Determination Phase.
* In this phase, first the players receive something like this:
```
9 You can check your score with the score command, type 'done' when finished.
15 Game 177 I: OmegaGo1 (0 4497 -1) vs Soothie (0 4377 -1)
15   4(B): Pass
```
* Then the players may send coordinates as normal, except that a coordinate no longer means "make move at this position" but "consider the chain that contains this position to be dead". Sending such a coordinate **receives an empty response followed by a prompt**, and the server sends this message to both players:
```
9 Removing @ B2
49 Game 177 OmegaGo1 is removing @ B2
```
Note that this, in effect, causes the sending player to receive two prompt lines one after another.
* If the position did not contain a chain or the chain is already declared dead, this response will be sent only to the sending player:
```
5 You cannot remove a liberty.
```
* In this phase, both players may type `undo [gamenumber]` regardless of any toggles. This causes all previous removal commands to be considered void and sends this message to both players (the `undo` command itself is unanswered):
```
9 Board is restored to what it was when you started scoring
9 Please type 'done again.
```
Note that the typo at `'done` (missing apostrophe) is present in the IGS protocol and is not a fault of this specification.
* Players may also type `done [gamenumber]` to signify that they're done with determinining life and death. The server will answer with an empty string and send this message to the other player:
```
9 Soothie has typed done.
```
* If both players typed `done` without any other successful coordinates or `undo` commands in the meantime, the game ends. The game may also end after a single `done` if the server determines that the opponent should not have any more objections or sometimes when a hidden time limit elapses. When this happens, a message like this is sent to both players:
```
22 OmegaGo1 17k  0 4497 25 F 6.5 0
22 Soothie 17k  0 4377 25 F 6.5 0
22  0: 3333333333333333333
22  1: 3333333333333333103
22  2: 3333333333333333333
22  3: 3333333333333333333
22  4: 3333333333333333333
22  5: 3333333333333333333
22  6: 3333333333333333333
22  7: 3333333333333333333
22  8: 3333333333333333333
22  9: 3333333333333333333
22 10: 3333333333333333333
22 11: 3333333333333333333
22 12: 3333333333333333333
22 13: 3333333333333333333
22 14: 3333333333333333333
22 15: 3333333333333333333
22 16: 3333333333333333333
22 17: 3333333333333333333
22 18: 3333333333333333333
20 OmegaGo1 (W:O):  6.5 to Soothie (B:g):  0.0
1 7

9 game completed.
1 7

9 Removed game file OmegaGo1-Soothie from database.
1 5
```
The line starting with the code `20` gives the final score. The board given on the preceding lines shows encoded territory.

## Professional games
In a professional game streamed by IGS, both usernames may be the same, such as `kansai1` or `kansai2`.

The information with each move sent will be like this:

```
15 Game 593 I: kansai1 (0 0 -1) vs kansai1 (0 0 -1)
15  90(B): L16
9 Game is titled: The 61st Kansai Ki-in Best Player Tournament, 2nd Round, Arakaki Shun 9-dan(W) vs Yokota Shigeaki 9-dan(B)
2
1 8
```

Note the information line with the game title and the time information sent as 0 0 -1.

The official client does show player names and ranks instead of "kansai1" and "kansai2", but not always.

## Multiple games vs. single game

If you are only in a single game, you don't need to type the game number as an argument to each command.



## List of reply codes
In client mode, all lines sent by the server, except for when the server is sending a file, will begin with a code followed by a single space. This is the list of codes used by IGS.

This list may miss some reply codes: a full list is in OmegaGo's `IgsCode.cs` file. 

* 1 - Prompt
* 2 - Console Bell - this line contains the ASCII character BEL
* 5 - Error Message
* 7 - Game Listing
* 8 - Help File Begins or Ends
* 9 - General Informational Message
* 14 - Message Listing
* 20 - Game Result

IGS may send one or more **files** in response to a command, usually in response to the `help` command. These files are different in that they are copied verbatim, as ASCII text, and the lines of these files do not begin with an IGS reply code. 

However, at the beginning and end of each sent file, there will be a line that contains a reply code (such as `8`, `9` or `14`) immediately followed by `File`, e.g. `14 File`.

## Additional sources

Some information on the protocol may also be found at these pages:
* http://web.archive.org/web/20050310114628/nngs.cosmic.org/help.html
* http://senseis.xmp.net/?IGS

### Reverse engineering

Some of this information was received from listening to the Telnet packets exchanged between the IGS server and the glGo and Pandanet2 desktop clients. You may use the application _Wireshark_ for this purpose.

