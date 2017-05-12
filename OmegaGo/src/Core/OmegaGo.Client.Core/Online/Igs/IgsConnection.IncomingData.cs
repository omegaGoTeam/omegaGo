using OmegaGo.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Online.Igs
{
    partial class IgsConnection
    {
        private bool _ignoreNextPrompt;

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
                catch (Exception)
                {
                    line = null;
                }
                if (line == null)
                {
                    ConnectionLost();
                    return;
                }
                line = line.Trim();


                IgsCode code = ExtractCodeFromLine(line);
                IgsLine igsLine = new IgsLine(code, line);
                Events.OnIncomingLine((weAreHandlingAnInterrupt ? "(INTERRUPT) " : "") + (interruptIsImpossible ? "(INTERRUPT IMPOSSIBLE) " : "") + line);

                // IGS occasionally sends blank lines, I don't know why. They serve no reason.
                if (line == "") continue;

                switch (this.Composure)
                {
                    case IgsComposure.Confused:
                    case IgsComposure.Ok:
                    case IgsComposure.Disconnected:
                        // No special mode.
                        break;
                    case IgsComposure.InitialHandshake:
                        if (igsLine.EntireLine.Trim() == "1 5")
                        {
                            this.Composure = IgsComposure.Ok;
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
                            this.Composure = IgsComposure.Confused;
                            this._loginError = "The password is incorrect.";
                            continue;
                        }
                        if (igsLine.EntireLine.Contains("Sorry, names can be"))
                        {
                            this.Composure = IgsComposure.Confused;
                            this._loginError = "Your name is too long.";
                            continue;
                        }
                        if (igsLine.EntireLine.Contains("This is a guest account."))
                        {
                            this.Composure = IgsComposure.Confused;
                            this._loginError = "The username does not exist.";
                            continue;
                        }
                        if (igsLine.EntireLine.Contains("1 5"))
                        {
                            this.Composure = IgsComposure.Ok;
                            continue;
                        }
                        break;

                }

                if (igsLine.Code == IgsCode.Error)
                {
                    Events.OnErrorMessageReceived(igsLine.PureLine);
                }
                currentLineBatch.Add(igsLine);

                if (weAreHandlingAnInterrupt && code == IgsCode.Prompt)
                {
                    // Interrupt message is over, let's wait for a new message
                    weAreHandlingAnInterrupt = false;
                    HandleFullInterrupt(currentLineBatch);
                    thisIsNotAMove = false;
                    interruptIsImpossible = false;
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
                if (code == IgsCode.Kibitz)
                {
                    weAreHandlingAnInterrupt = true;
                    continue;
                }
                if (code == IgsCode.Beep)
                {
                    Events.OnBeep();
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
                        OnIncomingStoneRemoval(removedStone.Item1, removedStone.Item2);
                        continue;
                    }
                    if (code == IgsCode.Move)
                    {
                        var heading = IgsRegex.ParseGameHeading(igsLine);
                        if (heading != null)
                        {
                            this.Data.LastReceivedGameHeading = heading;
                        }
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
                        if (igsLine.PureLine == "yes")
                        {
                            // This is "ayt" response, ignore it.
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }

                        if (igsLine.EntireLine ==
                            "9 You can check your score with the score command, type 'done' when finished.")
                        {
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }
                        if (igsLine.PureLine.Contains("accepted."))
                        {
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }
                        if (igsLine.PureLine.Contains("Removing @"))
                        {
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }
                        if (igsLine.PureLine.Contains("has run out of time"))
                        {
                            weAreHandlingAnInterrupt = true;
                            string whoRanOutOfTime = IgsRegex.WhoRanOutOfTime(igsLine);
                            foreach (var game in GetGamesIncluding(whoRanOutOfTime).ToList())
                            {
                                game.Controller.IgsConnector.EndTheGame(
                                    GameEndInformation.CreateTimeout(
                                        game.Controller.Players.First(pl => pl.Info.Name == whoRanOutOfTime),
                                        game.Controller.Players)
                                    );
                            }
                            continue;
                        }
                        if (igsLine.PureLine.Contains("White resigns.}"))
                        {
                            int gameInWhichSomebodyResigned = IgsRegex.WhatObservedGameWasResigned(igsLine);
                            ResignObservedGame(gameInWhichSomebodyResigned, StoneColor.White);
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }
                        if (igsLine.PureLine.Contains("Black resigns.}"))
                        {
                            int gameInWhichSomebodyResigned = IgsRegex.WhatObservedGameWasResigned(igsLine);
                            ResignObservedGame(gameInWhichSomebodyResigned, StoneColor.Black);
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }
                        if (igsLine.PureLine.Contains("has resigned the game"))
                        {
                            string whoResigned = IgsRegex.WhoResignedTheGame(igsLine);
                            if (whoResigned != this._username)
                            {
                                // .ToList() is used because the collection may be modified
                                foreach (var game in GetGamesIncluding(whoResigned).ToList())
                                {
                                    HandleIncomingResignation(game.Info, whoResigned);
                                }
                            }
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }
                        if (igsLine.PureLine.Contains("has typed done."))
                        {
                            string username = IgsRegex.GetFirstWord(igsLine);
                            weAreHandlingAnInterrupt = true;
                            foreach (var game in GetGamesIncluding(username))
                            {
                                var player = game.Controller.Players.First(pl => pl.Info.Name == username);
                                game.Controller.IgsConnector.RaiseServerSaidDone(player);
                            }
                            continue;
                        }
                        if (igsLine.PureLine.Contains("Board is restored to what it was when you started scoring"))
                        {
                            foreach (
                                var game in
                                    this.GamesYouHaveOpened.Where(
                                        gi =>
                                            gi.Controller.Phase.Type ==
                                            GamePhaseType.LifeDeathDetermination))
                            {
                                GetConnector(game.Info).ForceLifeDeathUndoDeathMarks();
                            }
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }

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
                        if (igsLine.PureLine.StartsWith("Increase "))
                        {
                            weAreHandlingAnInterrupt = true;
                            string person = IgsRegex.ParseIncreaseXTimeByYMinute(igsLine);
                            foreach (var game in this.GamesYouHaveOpened)
                            {
                                if (game.Info.Black.Name == person ||
                                    game.Info.White.Name == person)
                                {
                                    MakeUnattendedRequest("refresh " + game.Info.IgsIndex);
                                }
                            }
                        }

                        if (igsLine.PureLine.EndsWith("declines undo."))
                        {
                            string username = IgsRegex.WhoDeclinesUndo(igsLine);
                            foreach (var game in GetGamesIncluding(username))
                            {
                                Events.OnUndoDeclined(game.Info);
                            }
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }

                        if (igsLine.PureLine.EndsWith("declines your request for a match."))
                        {
                            Events.OnMatchRequestDeclined(igsLine.PureLine.Substring(0, igsLine.PureLine.IndexOf(' ')));
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }

                        IgsMatchRequest matchRequest = IgsRegex.ParseMatchRequest(igsLine);
                        if (matchRequest != null)
                        {
                            this._incomingMatchRequests.Add(matchRequest);
                            Events.OnIncomingMatchRequest(matchRequest);
                            weAreHandlingAnInterrupt = true;
                            continue;
                        }

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
                            if (this.Composure == IgsComposure.Ok)
                            {
                                Events.OnUnhandledLine(igsLine.EntireLine);
                            }
                        }
                    }
                }

            }
        }

        private IEnumerable<IgsGame> GetGamesIncluding(string username)
        {

            return this.GamesYouHaveOpened.Where(ginfo => ginfo.Info.Black.Name == username ||
                                                           ginfo.Info.White.Name == username);
        }



        private readonly Regex _regexMove = new Regex(@"([0-9]+)\((W|B)\): ([^ ]+)(.*)");
        private void HandleIncomingMove(IgsLine igsLine)
        {


            string trim = igsLine.PureLine.Trim();
            GameHeading heading = IgsRegex.ParseGameHeading(igsLine);
            if (heading != null)
            {
                IgsGame whatGame = this.GamesYouHaveOpened.Find(gm => gm.Info.IgsIndex == heading.GameNumber);
                if (whatGame == null)
                {
                    // Do not remember this game, perhaps we're in match accept procedure
                    return;
                }
                _incomingMovesAreForThisGame = whatGame;
                GetConnector(whatGame.Info).TimeControlAdjustment(new IgsTimeControlAdjustmentEventArgs(heading.WhiteTimeRemaining, heading.BlackTimeRemaining));

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
                HandleIncomingMove(_incomingMovesAreForThisGame, int.Parse(moveIndex), move);
            }
        }

        private async void HandleFullInterrupt(List<IgsLine> currentLineBatch)
        {
            if (currentLineBatch.Count > 0)
            {
                if (currentLineBatch.Any(line => line.Code == IgsCode.Status))
                {
                    var infoLine = currentLineBatch.FirstOrDefault(ln => ln.Code == IgsCode.Info);
                    if (infoLine != null)
                    {
                        ScoreLine scoreLine = IgsRegex.ParseObservedScoreLine(infoLine);
                        if (scoreLine != null) {
                            IgsGame gameInfo = this.GamesYouHaveOpened.FirstOrDefault(gi => gi.Info.IgsIndex == scoreLine.GameId);
                            if (gameInfo != null)
                            {
                                ScoreGame(gameInfo, scoreLine.BlackScore, scoreLine.WhiteScore);
                            }
                        }
                    }
                }
                if (currentLineBatch.Any(line => line.PureLine.EndsWith("accepted.") && line.Code == IgsCode.Info))
                {
                    // An outgoing match request has been accepted by another player and the game can begin.
                    GameHeading heading = this.Data.LastReceivedGameHeading;
                    var ogi = await Commands.GetGameByIdAsync(heading.GameNumber);
                    var builder = GameBuilder.CreateOnlineGame(ogi).Connection(this);
                    bool youAreBlack = ogi.Black.Name == _username;
                    bool youAreWhite = ogi.White.Name == _username;
                    if (youAreBlack)
                    {
                        builder.BlackPlayer(
                            new HumanPlayerBuilder(StoneColor.Black)
                            .Name(ogi.Black.Name)
                            .Rank(ogi.Black.Rank)
                            .Clock(new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(ogi.ByoyomiPeriod)).UpdateFrom(heading.BlackTimeRemaining))
                            .Build());
                    }
                    else
                    {
                        builder.BlackPlayer(
                            new IgsPlayerBuilder(StoneColor.Black, this)
                                .Name(ogi.Black.Name)
                                .Rank(ogi.Black.Rank)
                            .Clock(new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(ogi.ByoyomiPeriod)).UpdateFrom(heading.BlackTimeRemaining))
                                .Build());

                    }
                    if (youAreWhite)
                    {
                        builder.WhitePlayer(
                            new HumanPlayerBuilder(StoneColor.White)
                            .Name(ogi.White.Name)
                            .Rank(ogi.White.Rank)
                            .Clock(new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(ogi.ByoyomiPeriod)).UpdateFrom(heading.WhiteTimeRemaining))
                            .Build());
                    }
                    else
                    {
                        builder.WhitePlayer(
                            new IgsPlayerBuilder(StoneColor.White, this)
                                .Name(ogi.White.Name)
                                .Rank(ogi.White.Rank)
                            .Clock(new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(ogi.ByoyomiPeriod)).UpdateFrom(heading.WhiteTimeRemaining))
                                .Build());

                    }
                    IgsGame newGame = builder.Build();
                    this.GamesYouHaveOpened.Add(newGame);
                    Events.OnMatchRequestAccepted(newGame);
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
                                if (this.Composure == IgsComposure.Ok)
                                {
                                    Events.OnUnhandledLine(line.EntireLine);
                                }
                            }
                        }
                    }
                }

                if (currentLineBatch.Count == 3 && currentLineBatch[0].Code == IgsCode.SayInformation &&
                    currentLineBatch[1].Code == IgsCode.Say)
                {

                    int gameNumber = IgsRegex.ParseGameNumberFromSayInformation(currentLineBatch[0]);
                    ChatMessage chatLine = IgsRegex.ParseSayLine(currentLineBatch[1], this);
                    IgsGame relevantGame = this.GamesYouHaveOpened.Find(gi => gi.Info.IgsIndex == gameNumber);
                    if (relevantGame == null)
                    {
                        // We received a chat message for a game we no longer play.
                        return;
                    }
                    if (chatLine.Text.StartsWith(gameNumber + " "))
                    {
                        chatLine.Text = chatLine.Text.Substring((gameNumber + " ").Length);
                    }

                    GetConnector(relevantGame.Info).ChatMessageFromServer(chatLine);
                }

                if (currentLineBatch[0].Code == IgsCode.Kibitz &&
                    currentLineBatch.Count >= 2)
                {
                    // 11 Kibitz ([^ ]+).*\[([0-9]+)\]
                    Tuple<string, int> firstLine = IgsRegex.ParseKibitzHeading(currentLineBatch[0]);
                    string text = currentLineBatch[1].PureLine.Trim();
                    IgsGame relevantGame = this.GamesYouHaveOpened.Find(gi => gi.Info.IgsIndex == firstLine.Item2);
                    GetConnector(relevantGame.Info).ChatMessageFromServer(new ChatMessage(firstLine.Item1,
                        text, DateTimeOffset.Now, firstLine.Item1 == this.Username ? ChatMessageKind.Outgoing : ChatMessageKind.Incoming));
                }

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
                            Events.OnUndoRequestReceived(game.Info);
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
                    IgsGame gameInfo = this.GamesYouHaveOpened.Find(gi => gi.Info.IgsIndex == game);
                    for (int i = 0; i < numberOfMovesToUndo; i++)
                    {
                        GetConnector(gameInfo.Info).ForceMainUndo();
                    }
                }

                if (currentLineBatch[0].EntireLine.Contains("'done'"))
                {
                    IgsLine gameHeadingLine = currentLineBatch.Find(line => line.Code == IgsCode.Move);
                    int gameIndex = IgsRegex.ParseGameNumberFromHeading(gameHeadingLine);
                    _availableConnectors[gameIndex].SetPhaseFromServer(GamePhaseType.LifeDeathDetermination);
                }
                if (currentLineBatch.Any(ln => ln.Code == IgsCode.Score))
                {
                    ScoreLine scoreLine = IgsRegex.ParseScoreLine(currentLineBatch.Find(ln => ln.Code == IgsCode.Score));
                    IgsGame gameInfo = this.GamesYouHaveOpened.Find(gi =>
                        gi.Info.White.Name == scoreLine.White &&
                        gi.Info.Black.Name == scoreLine.Black);
                    ScoreGame(gameInfo, scoreLine.BlackScore, scoreLine.WhiteScore);
                }
            }
        }

    }
}
