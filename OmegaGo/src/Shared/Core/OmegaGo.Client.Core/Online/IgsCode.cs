using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online
{
    public enum IgsCode
    {
        ///<summary>
        ///\7 telnet
        ///</summary>
        Beep = 2,
        ///<summary>
        ///Board being drawn
        ///</summary>
        Board = 3,
        ///<summary>
        ///The server is going down
        ///</summary>
        Down = 4,
        ///<summary>
        ///An error reported
        ///</summary>
        Error = 5,
        ///<summary>
        ///File being sent
        ///</summary>
        Fil = 6,
        ///<summary>
        ///Games listing
        ///</summary>
        Games = 7,
        ///<summary>
        ///Help file
        ///</summary>
        Help = 8,
        ///<summary>
        ///Generic info
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
        ///Move #:(B) A1
        ///</summary>
        Move = 15,
        ///<summary>
        ///Observe report
        ///</summary>
        Observe = 16,
        ///<summary>
        ///A Prompt (never)
        ///</summary>
        Prompt = 1,
        ///<summary>
        ///Refresh of a board
        ///</summary>
        Refresh = 17,
        ///<summary>
        ///Stored command
        ///</summary>
        Saved = 18,
        ///<summary>
        ///Say string
        ///</summary>
        Say = 19,
        ///<summary>
        ///Score report
        ///</summary>
        Score = 20,
        ///<summary>
        ///SGF variation
        ///</summary>
        Sgf = 34,
        ///<summary>
        ///Shout string
        ///</summary>
        Shout = 21,
        ///<summary>
        ///Shout string
        ///</summary>
        Show = 29,
        ///<summary>
        ///Current Game status
        ///</summary>
        Status = 22,
        ///<summary>
        ///Stored games
        ///</summary>
        Stored = 23,
        ///<summary>
        ///teaching game
        ///</summary>
        Teach = 33,
        ///<summary>
        ///Tell string
        ///</summary>
        Tell = 24,
        ///<summary>
        ///your . string
        ///</summary>
        Dot = 40,
        ///<summary>
        ///Thist report
        ///</summary>
        Thist = 25,
        ///<summary>
        ///times command
        ///</summary>
        Tim = 26,
        ///<summary>
        ///Translation info
        ///</summary>
        Trans = 30,
        ///<summary>
        ///tic tac toe
        ///</summary>
        Ttt = 37,
        ///<summary>
        ///who command
        ///</summary>
        Who = 27,
        ///<summary>
        ///Undo report
        ///</summary>
        Undo = 28,
        ///<summary>
        ///Long user report
        ///</summary>
        User = 42,
        ///<summary>
        ///IGS Version
        ///</summary>
        Version = 39,
        ///<summary>
        ///IGSChannel yelling
        ///</summary>
        Yell = 32
    }
}
