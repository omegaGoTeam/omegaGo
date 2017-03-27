# Adding an additional server

_Relevant requirement:_

*   _8.2\. It will not be difficult to add an additional online server later on._

We designed omegaGo to allow for easily adding support for additional Internet servers. Despite that, however, it remains a significant challenge to do so. This is, unfortunately, inevitable, because there is very little similarity among Go servers and the API of these servers, if public at all, tend to have poor documentation.

To add support for an additional server, you should:

*   Create a new class that implements _IServerConnection_.
*   Add an instance of the new class as a singleton to the _Connections_ static class.
*   Create a new view that represents the server lobby.
    *   Game creation and game accept may or may not require creating a new variant of GameCreationView.
*   Add a button to the main menu that opens this lobby.
*   Create a new subclass of GameController that handles functionality specific to this server.
    *   This will possibly require the creation of subclasses for individual phases.
*   Add settings to the InterfaceMementos class for the new login form and lobby.
*   Statistics
    *   Add settings to the StatisticsRecords class for keeping track of rank.
    *   Display the statistics in the user interface
    *   Update the statistics when the user logs in

As you can see from the size of the above list, adding support for an additional server is a major undertaking. No wonder there's only one other application that supports multiple servers! (qGo) 

The above list may seem daunting, but you should still be able to share a lot of functionality with the other servers, and so may use common superclasses and/or copy code.  The most difficult part of this task will be to analyze the API, write it down and learn how it behaves. 

Good luck finding good documentation, it is extremely scarce. You must understand that Internet Go servers, despite their popularity, are quite often the work of a single developer or perhaps a small team of two or three. Having a public API is often a sidejob, distracting them from maintaining the server or developing their own client. So I suppose I'm happy with what I have.

The third server to be added should be OGS (http://online-go.com) since, contrary to IGS and KGS, it is much more friendly to beginners. OGS does have responsive admins who can help, and it has a developed public API, but its documentation is very poor, especially the parts that rely on socket.io.