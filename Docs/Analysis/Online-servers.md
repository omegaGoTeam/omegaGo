# Online servers

To our knowledge, there are five major online servers:

| Short name | Long name | Note|
| :------------- |:-------------|:-----|
|IGS|Pandanet|the first to exist|
|KGS|KGS Go Server|the first with a Java web interface|
|OGS|Online-Go.com|very modern and pretty|
|DGS|Dragon Go Server|for correspondance games|
|Tygem|Tygem Baduk Server|popular among high-ranking players|

They share some functionality, but many things are very different in each server.

For example, each server uses a different method to connect to it. IGS uses Telnet ASCII commands, KGS uses a JSON metaserver that translates into a binary interface, OGS uses _sockets.io_, DGS uses HTTP, RSS and WAP and Tygem uses a binary interface.

Each server supports some functionality that others don't. For example, IGS connects to a social network, KGS transfers audio commentary and OGS allows conditional moves.

We have decided to support two online servers and have selected IGS and KGS because
* These servers would benefit the most from a modern client (as opposed to OGS which already has extremely good client interface, and DGS which is used only by well-enfranchised players).
* These servers presented the least difficulties in implementing a connection (as opposed to OGS which is lacking documentation and uses _socket.io_ and Tygem which does not make its API public at all).
