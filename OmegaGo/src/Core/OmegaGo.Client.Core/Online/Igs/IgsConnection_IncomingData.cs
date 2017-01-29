using OmegaGo.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Igs
{
    partial class IgsConnection
    {
        private bool _ignoreNextPrompt;
        /*
         * 
         * 
         *  Creator:
         *  
            15 Game 10 I: Soothie (0 4500 -1) vs OmegaGo1 (0 4500 -1)
            9 Handicap and komi are disable.
            9 Match [10] with OmegaGo1 in 75 accepted.
            9 Please use say to talk to your opponent -- help say.
            1 6

         * 
         *  Acceptor:
    
            15 Game 10 I: Soothie (0 4500 -1) vs OmegaGo1 (0 4500 -1)
            9 Handicap and komi are disable.
            9 Creating match [10] with Soothie.
            9 Please use say to talk to your opponent -- help say.
            1 6
            */

        private async Task HandleIncomingData(StreamReader sr)
        {
            bool thisIsNotAMove = false;
            bool weAreHandlingAnInterrupt = false;
            bool interruptIsImpossible = false;
            List<IgsLine> currentLineBatch = new List<IgsLine>();
            while (true)
            {
                string line;
                try
                {
                    line = await sr.ReadLineAsync();
                }
                catch (Exception ex)
                    when (ex is ObjectDisposedException || ex is System.Runtime.InteropServices.COMException)
                {
                    line = null;
                }
                if (line == null)
                {
                    OnIncomingLine("The connection has been terminated.");
                    // TODO add thread safety
                    this._client = null;
                    return;
                }
                line = line.Trim();

                // IGS occasionally sends blank lines, I don't know why. They serve no reason.
                if (line == "") continue;

                IgsCode code = ExtractCodeFromLine(line);
                IgsLine igsLine = new IgsLine(code, line);
                OnIncomingLine(line);

                switch (this._composure)
                {
                    case IgsComposure.Confused:
                    case IgsComposure.Ok:
                    case IgsComposure.Disconnected:
                        // No special mode.
                        break;
                    case IgsComposure.InitialHandshake:
                        if (igsLine.EntireLine.Trim() == "1 5")
                        {
                            this._composure = IgsComposure.Ok;
                            continue;
                        }
                        else
                        {
                            // Ignore.
                            continue;
                        }
                    case IgsComposure.LoggingIn:
                        if (igsLine.EntireLine.Contains("Invalid password."))
                        {
                            this._composure = IgsComposure.Confused;
                            this._loginError = "The password is incorrect.";
                            continue;
                        }
                        if (igsLine.EntireLine.Contains("This is a guest account."))
                        {
                            this._composure = IgsComposure.Confused;
                            this._loginError = "The username does not exist.";
                            continue;
                        }
                        if (igsLine.EntireLine.Contains("1 5"))
                        {
                            this._composure = IgsComposure.Ok;
                            continue;
                        }
                        break;

                }

                if (igsLine.Code == IgsCode.Error)
                {
                    OnErrorMessageReceived(igsLine.PureLine);
                }
                currentLineBatch.Add(igsLine);

                if (weAreHandlingAnInterrupt && code == IgsCode.Prompt)
                {
                    // Interrupt message is over, let's wait for a new message
                    weAreHandlingAnInterrupt = false;
                    HandleFullInterrupt(currentLineBatch);
                    thisIsNotAMove = false;
                    currentLineBatch = new List<IgsLine>();
                    continue;
                }
                if (code == IgsCode.Prompt)
                {
                    thisIsNotAMove = false;
                    currentLineBatch = new List<IgsLine>();
                    interruptIsImpossible = false;
                    if (this._ignoreNextPrompt)
                    {
                        this._ignoreNextPrompt = false;
                        continue;
                    }
                }
                if (code == IgsCode.Beep)
                {
                    OnBeep();
                    continue;
                }

                if (!interruptIsImpossible)
                {
                    if (code == IgsCode.Tell)
                    {
                        if (igsLine.PureLine.StartsWith("*SYSTEM*"))
                        {
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }
                        HandleIncomingChatMessage(line);
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (code == IgsCode.SayInformation)
                    {
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (code == IgsCode.Status)
                    {
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (code == IgsCode.Shout)
                    {
                        HandleIncomingShoutMessage(line);
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (code == IgsCode.StoneRemoval)
                    {
                        Tuple<int, Position> removedStone = IgsRegex.ParseStoneRemoval(igsLine);
                        // TODO
                        /*
                    _gamesYouHaveOpened.Find(gi => gi.Metadata.ServerId == removedStone.Item1).GameController.MarkGroupDead(removedStone.Item2);
                    */
                        continue;
                    }
                    if (code == IgsCode.Move)
                    {
                        if (!thisIsNotAMove)
                        {
                            HandleIncomingMove(igsLine);
                            weAreHandlingAnInterrupt = true;
                        }
                        continue;
                    }
                    if (code == IgsCode.Undo)
                    {
                        thisIsNotAMove = true;
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                }
                if (code == IgsCode.Info)
                {
                    // 9 Adding game to observation list.
                    if (igsLine.EntireLine.Contains("9 Adding game to observation list."))
                    {
                        interruptIsImpossible = true;
                    }
                    if (!interruptIsImpossible)
                    {
                        /* TODO
                    if (igsLine.EntireLine == "9 You can check your score with the score command, type 'done' when finished.")
                    {
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (igsLine.PureLine.Contains("Removing @"))
                    {
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (igsLine.PureLine.EndsWith("has resigned the game."))
                    {
                        string whoResigned = IgsRegex.WhoResignedTheGame(igsLine);
                        if (whoResigned != this._username)
                        {
                            foreach (var game in GetGamesIncluding(whoResigned))
                            {
                                // TODO
                              //    game.GameController.Resign(game.Players.Find(pl => pl.Name == whoResigned));
                            }
                        }
                        weAreHandlingAnInterrupt = true;
                    }
                    if (igsLine.PureLine.Contains("has typed done."))
                    {
                        string username = IgsRegex.GetFirstWord(igsLine);
                        foreach (var game in GetGamesIncluding(username))
                        {
                            // TODO
                            //**game.GameController.LifeDeath_Done(game.Players.Find(pl => pl.Name == username));
                        }
                    }
                    if (igsLine.PureLine.Contains("Board is restored to what it was when you started scoring"))
                    {
                        foreach (var game in _gamesYouHaveOpened.Where(gi => gi.GameController.GamePhase == GamePhase.LifeDeathDetermination))
                        {
                            game.GameController.LifeDeath_UndoPhase();
                        }
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }*/

                    if (igsLine.PureLine.Contains("Removed game file"))
                    {
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (igsLine.PureLine.Contains("game completed."))
                    {
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (igsLine.PureLine.StartsWith("!!*Pandanet*!!:"))
                    {
                        // Advertisement
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    if (IgsRegex.IsIrrelevantInterruptLine(igsLine))
                    {
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    /*
                    if (igsLine.PureLine.EndsWith("declines undo."))
                    {
                        string username = IgsRegex.WhoDeclinesUndo(igsLine);
                        foreach (var game in GetGamesIncluding(username))
                        {
                            OnUndoDeclined(game);
                        }
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }

                    if (igsLine.PureLine.EndsWith("declines your request for a match."))
                    {
                        OnMatchRequestDeclined(igsLine.PureLine.Substring(0, igsLine.PureLine.IndexOf(' ')));
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    IgsMatchRequest matchRequest = IgsRegex.ParseMatchRequest(igsLine);
                    if (matchRequest != null)
                    {
                        this._incomingMatchRequests.Add(matchRequest);
                        OnIncomingMatchRequest(matchRequest);
                        weAreHandlingAnInterrupt = true;
                        continue;
                    }
                    */
                    }
                }

                if (!weAreHandlingAnInterrupt)
                {
                    // We cannot handle this generally - let's hand it off to whoever made the request for this information.
                    lock (this._mutex)
                    {
                        if (this._requestInProgress != null)
                        {
                            this._requestInProgress.IncomingLines.Post(igsLine);
                        }
                        else
                        {
                            if (this._composure == IgsComposure.Ok)
                            {
                                OnUnhandledLine(igsLine.EntireLine);
                            }
                        }
                    }
                }

            }
        }

        private IEnumerable<GameInfo> GetGamesIncluding(string username)
        {

            return this._gamesYouHaveOpened.Where(ginfo => ginfo.Info.Black.Name == username ||
                                                           ginfo.Info.White.Name == username)
                .Select(ginfo => ginfo.Metadata);
        }



        private readonly Regex _regexUser = new Regex(@"42 +([^ ]+) +.* ([A-Za-z-.].{6})  (...)(\*| ) [^/]+/ *[^ ]+ +[^ ]+ +[^ ]+ +[^ ]+ +([^ ]+) default", RegexOptions.None);
        private IgsUser CreateUserFromTelnetLine(string line)
        {
            Match match = _regexUser.Match(line);
            if (!match.Success)
            {
                throw new Exception("IGS SERVER returned invalid user string.");
            }
            /*
             *  1 - Name
             *  2 - Country
             *  3 - Rank
             *  4 - Calculated or self-described rank?
             *  5 - Flags
             * 
             */

            IgsUser user = new IgsUser()
            {
                Name = match.Groups[1].Value,
                Country = match.Groups[2].Value,
                Rank = match.Groups[3].Value.StartsWith(" ") ? match.Groups[3].Value.Substring(1) : match.Groups[3].Value,
                LookingForAGame = match.Groups[5].Value.Contains("!"),
                RejectsRequests = match.Groups[5].Value.Contains("X")
            };
            return user;

        }
        private readonly Regex _regexMove = new Regex(@"([0-9]+)\((W|B)\): ([^ ]+)(.*)");
        private void HandleIncomingMove(IgsLine igsLine)
        {


            string trim = igsLine.PureLine.Trim();
            GameHeading heading = IgsRegex.ParseGameHeading(igsLine);
            if (heading != null)
            {
                OnlineGame whatGame = _gamesYouHaveOpened.Find(gm => gm.Metadata.IgsIndex == heading.GameNumber);
                if (whatGame == null)
                {
                    // Synchronization mishap
                    return;
                }
                _incomingMovesAreForThisGame = whatGame;
            }
            else if (trim.Contains("Handicap"))
            {
                //  15   0(B): Handicap 3
                int handicapStones = IgsRegex.ParseHandicapMove(igsLine);
                OnIncomingHandicapInformation(_incomingMovesAreForThisGame, handicapStones);
            }
            else
            {
                Match match = this._regexMove.Match(trim);
                string moveIndex = match.Groups[1].Value;
                string mover = match.Groups[2].Value;
                string coordinates = match.Groups[3].Value;
                string captures = match.Groups[4].Value;
                StoneColor moverColor = mover == "B" ? StoneColor.Black : StoneColor.White;
                Move move;
                if (coordinates == "Pass")
                {
                    move = Move.Pass(moverColor);
                }
                else
                {
                    move = Move.PlaceStone(moverColor,
                        Position.FromIgsCoordinates(coordinates));
                }
                string[] captureSplit = captures.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string capture in captureSplit)
                {
                    move.Captures.Add(Position.FromIgsCoordinates(capture));
                }
                OnIncomingMove(_incomingMovesAreForThisGame, int.Parse(moveIndex), move);
            }
        }

        private void HandleFullInterrupt(List<IgsLine> currentLineBatch)
        {
            if (currentLineBatch.Count > 0)
            {
                /*
                               if (currentLineBatch.Any(line => line.PureLine.EndsWith("accepted.") && line.Code == IgsCode.Info))
                {
                    GameHeading heading = IgsRegex.ParseGameHeading(currentLineBatch[0]);
                    ObsoleteGameInfo game = new ObsoleteGameInfo()
                    {
                        BoardSize = new GameBoardSize(19), // TODO
                        Server = this,
                        ServerId = heading.GameNumber,
                    };
                    game.Players.Add(new GamePlayer(heading.BlackName, "?", game));
                    game.Players.Add(new GamePlayer(heading.WhiteName, "?", game));
                    game.Ruleset = new JapaneseRuleset(game.BoardSize);
                    this._gamesInProgressOnIgs.RemoveAll(gm => gm.ServerId == heading.GameNumber);
                    this._gamesInProgressOnIgs.Add(game);
                    this._gamesYouHaveOpened.Add(game);
                    OnMatchRequestAccepted(game);

                }
                if (currentLineBatch.Any(line => line.PureLine.Contains("Creating match") && line.Code == IgsCode.Info))
                {
                    // Make it not be an interrupt and let it be handled by the match creator.
                    foreach (IgsLine line in currentLineBatch)
                    {
                        lock (this._mutex)
                        {
                            if (this._requestInProgress != null)
                            {
                                this._requestInProgress.IncomingLines.Post(line);
                            }
                            else
                            {
                                if (this._composure == IgsComposure.Ok)
                                {
                                    OnUnhandledLine(line.EntireLine);
                                }
                            }
                        }
                    }
                }
                */
                if (currentLineBatch.Count == 3 && currentLineBatch[0].Code == IgsCode.SayInformation &&
                    currentLineBatch[1].Code == IgsCode.Say)
                {
                   
                    int gameNumber = IgsRegex.ParseGameNumberFromSayInformation(currentLineBatch[0]);
                    ChatMessage chatLine = IgsRegex.ParseSayLine(currentLineBatch[1]);
                    OnlineGame relevantGame = this._gamesYouHaveOpened.Find(gi => gi.Metadata.IgsIndex == gameNumber);
                    if (relevantGame == null)
                    {
                        // We received a chat message for a game we no longer play.
                        return;
                    }
                    // TODO
                    //OnIncomingInGameChatMessage(relevantGame, chatLine);
                }
                /*
                if (currentLineBatch[0].Code == IgsCode.Tell &&
                    currentLineBatch[0].PureLine.StartsWith("*SYSTEM*") &&
                    currentLineBatch[0].PureLine.EndsWith("requests undo."))
                {
                    string requestingUser = IgsRegex.WhoRequestsUndo(currentLineBatch[0]);
                    var games = GetGamesIncluding(requestingUser);
                    if (games.Any())
                    {
                        foreach (var game in games)
                        {
                            OnUndoRequestReceived(game);
                        }
                    }
                    else
                    {
                        throw new Exception("Received an undo request for a game that's not in progress.");
                    }
                    this._ignoreNextPrompt = true;
                }
                if (currentLineBatch[0].Code == IgsCode.Undo)
                {
                    int numberOfMovesToUndo = currentLineBatch.Count(line => line.Code == IgsCode.Undo);
                    IgsLine gameHeadingLine = currentLineBatch.Find(line => line.Code == IgsCode.Move);
                    int game = IgsRegex.ParseGameNumberFromHeading(gameHeadingLine);
                    ObsoleteGameInfo gameInfo = this._gamesYouHaveOpened.Find(gi => gi.ServerId == game);
                    for (int i = 0; i < numberOfMovesToUndo; i++)
                    {
                        OnLastMoveUndone(gameInfo);
                    }
                }
                if (currentLineBatch[0].EntireLine.Contains("'done'"))
                {
                    IgsLine gameHeadingLine = currentLineBatch.Find(line => line.Code == IgsCode.Move);
                    int game = IgsRegex.ParseGameNumberFromHeading(gameHeadingLine);
                    ObsoleteGameInfo gameInfo = this._gamesYouHaveOpened.Find(gi => gi.ServerId == game);
                    gameInfo.GameController.MainPhase_EnterLifeDeath();
                }
                if (currentLineBatch.Any(ln => ln.Code == IgsCode.Score))
                {
                    ScoreLine scoreLine = IgsRegex.ParseScoreLine(currentLineBatch.Find(ln => ln.Code == IgsCode.Score));
                    ObsoleteGameInfo gameInfo = this._gamesYouHaveOpened.Find(gi =>
                        gi.White.Name == scoreLine.White &&
                        gi.Black.Name == scoreLine.Black);
                    OnGameScoreAndCompleted(gameInfo, scoreLine.BlackScore, scoreLine.WhiteScore);
                }
                */
            }
        }
        
    }
}
