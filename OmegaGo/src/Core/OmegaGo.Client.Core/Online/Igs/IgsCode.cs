namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// When IGS is in "client mode", each line it sends (besides lines printed after a "FILE" output line) begins with an <see cref="IgsCode"/> number. 
    /// </summary>
    public enum IgsCode
    {
        ///<summary>
        /// A prompt. A prompt always terminates a block of lines sent by the IGS server.
        /// 
        /// <para>
        /// Prompt lines have a single argument, which may be:
        /// <list type="bullet">
        /// <item>0 - Requesting username</item>
        /// <item>1 - Requesting password</item>
        /// <item>5 - Requesting general command</item>
        /// <item>6 - You are playing a game</item>
        /// <item>7 - You are in Life/Death Determination phase</item>
        /// <item>8 - You are observing a game</item>
        /// </list> 
        /// </para> 
        ///</summary>
        Prompt = 1,
        ///<summary>
        /// A beep line contains the non-printable \7 "bell" character and is printed by the server when it wants the player's attention,
        /// perhaps because a chat message arrived or a player made a move.
        ///</summary>
        Beep = 2,
        ///<summary>
        /// Board being drawn
        ///</summary>
        Board = 3,
        ///<summary>
        ///The server is going down
        ///</summary>
        Down = 4,
        ///<summary>
        /// The server is reporting an error. This error should be reported to the client, and sometimes automatic action should be taken on it.
        /// Known errors are:
        /// <list type="bullet">
        /// <item>"Cannot find recipient" - when trying to Tell to an offline or nonexistent user</item>
        /// </list>
        ///</summary>
        Error = 5,
        ///<summary>
        ///File being sent
        ///</summary>
        File = 6,
        ///<summary>
        /// This line contains basic information about a single game that's being played.
        ///</summary>
        Games = 7,
        ///<summary>
        /// A line containing the string "8 File" is sent at the beginning and end of each response to a successful "help" command.
        ///</summary>
        Help = 8,
        ///<summary>
        /// A general informational message. Sometimes we can ignore these, othertimes they contain valuable information
        ///</summary>
        Info = 9,
        ///<summary>
        ///Last command
        ///</summary>
        Last = 10,
        ///<summary>
        ///Kibitz strings
        ///</summary>
        Kibitz = 11,
        ///<summary>
        ///Loading a game
        ///</summary>
        Load = 12,
        ///<summary>
        ///Look
        ///</summary>
        Look = 13,
        ///<summary>
        ///Message listing
        ///</summary>
        Message = 14,
        ///<summary>
        /// Move #:(B) A1. However, this may also be a game heading.
        ///</summary>
        Move = 15,
        ///<summary>
        ///Observe report
        ///</summary>
        Observe = 16,
        ///<summary>
        ///Refresh of a board
        ///</summary>
        Refresh = 17,
        ///<summary>
        ///Stored command
        ///</summary>
        Saved = 18,
        ///<summary>
        /// An incoming chat message that is to be recorded in a game record
        ///</summary>
        Say = 19,
        ///<summary>
        /// The score line gives points for each player at the end of the game.
        ///</summary>
        Score = 20,
        ///<summary>
        /// Someone broadcasts a message to ALL online players. A shout message may have the form !name!: message, or !!name!!: message if sent 
        /// by an administrator.
        ///</summary>
        Shout = 21,
        ///<summary>
        /// Current game status - This describes the full board and information about both players.
        ///</summary>
        Status = 22,
        ///<summary>
        ///Stored games
        ///</summary>
        Stored = 23,
        ///<summary>
        /// Someone sends us, directly, a chat message that will not be recorded in any log, using IGS's "tell" command.
        /// The message will have the form *name*: message
        ///</summary>
        Tell = 24,
        ///<summary>
        ///Thist report
        ///</summary>
        Thist = 25,
        ///<summary>
        ///times command
        ///</summary>
        Times = 26,
        ///<summary>
        ///who command
        ///</summary>
        Who = 27,
        ///<summary>
        /// Undo report that states that a user undid a move.
        ///</summary>
        Undo = 28,
        ///<summary>
        ///Shout string
        ///</summary>
        Show = 29,
        ///<summary>
        ///Translation info
        ///</summary>
        Trans = 30,
        ///<summary>
        ///IGSChannel yelling
        ///</summary>
        Yell = 32,
        ///<summary>
        ///teaching game
        ///</summary>
        Teach = 33,
        ///<summary>
        ///SGF variation
        ///</summary>
        Sgf = 34,
        ///<summary>
        ///tic tac toe
        ///</summary>
        Ttt = 37,
        ///<summary>
        /// IGS Version - we will summarily ignore this line.
        ///</summary>
        Version = 39,
        ///<summary>
        /// Your . string. However, omegaGo doesn't use dot strings so we may ignore these.
        ///</summary>
        Dot = 40,
        ///<summary>
        ///Long user report
        ///</summary>
        User = 42,
        /// <summary>
        /// A stone is marked dead by a player. (49 Game 444 Soothie is removing @ J1)
        /// </summary>
        StoneRemoval = 49,
        /// <summary>
        /// Information about which game the incoming "say" chat line refers to
        /// </summary>
        SayInformation = 51,
        /// <summary>
        /// An unrecognized IGS line code
        /// </summary>
        Unknown
    }
}
