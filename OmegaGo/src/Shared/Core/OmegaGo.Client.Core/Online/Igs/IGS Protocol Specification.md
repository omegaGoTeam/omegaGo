# IGS Protocol Specification
The **Internet Go Server** (IGS) was first put online in 1992 and used Telnet as a wire protocol and simple text commands. The first Go clients that connected to it provided some extra features to users but in the end, they still had to know and use commands.

As time went on and graphical clients developed, the Telnet interface was used less and less by common users, however all clients continued to use it, albeit hidden from the user. Originally, most or all IGS commands were documented in the interface itself, with the `help` command providing detailed information on syntax and meaning of commands.

Around 1995, IGS - originally developed at an American university - was acquired by a Japanese company and renamed **Pandanet** (sometimes *Pandanet: Internet Go Server*).

Eventually, when Pandanet created established clients for Windows, Mac, Linux, Android and iOS, they had no further need to document commands. They appear to now discourage the use of "non-official" clients and don't make effort to maintain documentation.

For this reason, I put together this specification, pieced together from help files remaining on the IGS server and from reverse-engineering the communication sent between the server and the official clients.

If you discover an error in this specification, please inform me at <petrhudecek2010@gmail.com>. Thank you.

### Host

To connect to the IGS server, connect via Telnet to one of these hosts:

* igs.joyjoy.net:6969
* igs.joyjoy.net:7777
* 210.155.158.200:6969
* 210.155.158.200:7777

If one of the ports fail, try the other one. Users, games and everything is shared between these ports. Users who connect to one of these ports will communicate normally with users on the other port.

### Connection start
When you connect, the server will print a welcome message and the prompt "Login: ". You should respond with your username.
* If you send a username that doesn't exist, you will move to the main command loop, logged in a *guest account* with the name *guestXXXX* where XXXX is a number. Your typed username will be forgotten.
* If you send a username that exists, the server will respond either with "Password: " or with "1 1". The second response is the client-mode response. Reply with a password. An incorrect password will cause the login procedure to restart, but the client-mode setting will be remembered. A correct password will move you to the main command loop, logged in as the user.

Once in the main command loop, the server may send you lines of text at any moment and you may send commands to the server at any moment as well. There are few timing guarantees.

### Client mode

Each user account is associated with the state of several *toggles*, the most important of which is the client mode. If you're in client mode, the lines server sends to you will be a little different. Most importantly, they will be prefix with a "code"
:
