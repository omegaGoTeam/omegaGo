using OmegaGo.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Igs
{
    partial class IgsConnection
    {
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
            bool thisRequestIsObservationStart = false;
            bool weAreHandlingAnInterruptMessage = false;
            List<IgsLine> currentLineBatch = new List<IgsLine>();
            while (true)
            {
                string line = await sr.ReadLineAsync();
                if (line == null)
                {
                    OnLogEvent("The connection has been terminated.");
                    // TODO add thread safety
                    _client = null;
                    return;
                }
                line = line.Trim();

                // IGS occasionally sends blank lines, I don't know why. They serve no reason.
                if (line == "") continue;

                IgsCode code = ExtractCodeFromLine(line);
                IgsLine igsLine = new IgsLine(code, line);
                OnLogEvent(line);

                switch (_composure)
                {
                    case IgsComposure.Confused:
                    case IgsComposure.Ok:
                    case IgsComposure.Disconnected:
                        // No special mode.
                        break;
                    case IgsComposure.InitialHandshake:
                        if (igsLine.EntireLine.Trim() == "1 5")
                        {
                            _composure = IgsComposure.Ok;
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
                            _loginError = "The password is incorrect.";
                        }
                        if (igsLine.EntireLine.Contains("This is a guest account."))
                        {
                            _loginError = "The username does not exist.";
                        }
                        if (igsLine.EntireLine.Contains("1 5"))
                        {
                            _composure = IgsComposure.Ok;
                            continue;
                        }
                        break;

                }

                currentLineBatch.Add(igsLine);

                if (weAreHandlingAnInterruptMessage && code == IgsCode.Prompt)
                {

                    // Interrupt message is over, let's wait for a new message
                    weAreHandlingAnInterruptMessage = false;
                    HandleFullInterrupt(currentLineBatch);
                    thisRequestIsObservationStart = false;
                    currentLineBatch = new List<IgsLine>();
                    continue;
                }
                if (code == IgsCode.Prompt)
                {
                    thisRequestIsObservationStart = false;
                    currentLineBatch = new List<IgsLine>();
                }
                if (code == IgsCode.Beep)
                {
                    OnBeep();
                    continue;
                }
                if (code == IgsCode.Tell)
                {
                    HandleIncomingChatMessage(line);
                    weAreHandlingAnInterruptMessage = true;
                    continue;
                }
                if (code == IgsCode.SayInformation)
                {
                    weAreHandlingAnInterruptMessage = true;
                    continue;
                }
                if (code == IgsCode.Shout)
                {
                    HandleIncomingShoutMessage(line);
                    weAreHandlingAnInterruptMessage = true;
                    continue;
                }
                if (code == IgsCode.Move)
                {
                    if (!thisRequestIsObservationStart)
                    {
                        HandleIncomingMove(igsLine);
                        weAreHandlingAnInterruptMessage = true;
                    }
                    continue;
                }
                if (code == IgsCode.Info)
                {
                    if (igsLine.PureLine.StartsWith("!!*Pandanet*!!:"))
                    {
                        // Advertisement
                        weAreHandlingAnInterruptMessage = true;
                        continue;
                    }
                    if (igsLine.PureLine.StartsWith("Adding game to observation list"))
                    {
                        thisRequestIsObservationStart = true;
                        continue;
                    }
                    if (IgsRegex.IsIrrelevantInterruptLine(igsLine))
                    {
                        weAreHandlingAnInterruptMessage = true;
                        continue;
                    }

                    if (igsLine.PureLine.EndsWith("declines your request for a match."))
                    {
                        OnMatchRequestDeclined(igsLine.PureLine.Substring(0, igsLine.PureLine.IndexOf(' ')));
                        weAreHandlingAnInterruptMessage = true;
                        continue;
                    }
                    IgsMatchRequest matchRequest = IgsRegex.ParseMatchRequest(igsLine);
                    if (matchRequest != null)
                    {
                        this.IncomingMatchRequests.Add(matchRequest);
                        OnIncomingMatchRequest(matchRequest);
                        weAreHandlingAnInterruptMessage = true;
                        continue;
                    }
                }

                if (!weAreHandlingAnInterruptMessage)
                {
                    // We cannot handle this generally - let's hand it off to whoever made the request for this information.
                    lock (_mutex)
                    {
                        if (_requestInProgress != null)
                        {
                            _requestInProgress.IncomingLines.Post(igsLine);
                        }
                        else
                        {
                            if (_composure == IgsComposure.Ok)
                            {
                                OnUnhandledLine(igsLine.EntireLine);
                            }
                        }
                    }
                }

            }
        }

        private void HandleFullInterrupt(List<IgsLine> currentLineBatch)
        {
            /* Acceptor:    
             15 Game 10 I: Soothie (0 4500 -1) vs OmegaGo1 (0 4500 -1)
             9 Handicap and komi are disable.
             9 Creating match [10] with Soothie.
             9 Please use say to talk to your opponent -- help say.
             1 6

             Creator of accepted game:
             9 Handicap and komi are disable.
             9 Match [806] with OmegaGo1 in 75 accepted.
             9 Please use say to talk to your opponent -- help say.
             1 6
             */
            if (currentLineBatch.Any(line => line.PureLine.EndsWith("accepted.") && line.Code == IgsCode.Info))
            {
                GameHeading heading = IgsRegex.ParseGameHeading(currentLineBatch[0]);
                GameInfo game = new Core.GameInfo()
                {
                    BoardSize = new Core.GameBoardSize(19), // TODO
                    Server = this,
                    ServerId = heading.GameNumber,
                };
                game.Players.Add(new Core.Player(heading.BlackName, "?", game));
                game.Players.Add(new Core.Player(heading.WhiteName, "?", game));
                game.Ruleset = new JapaneseRuleset(game.BoardSize);
                this._gamesInProgressOnIgs.RemoveAll(gm => gm.ServerId == heading.GameNumber);
                this._gamesInProgressOnIgs.Add(game);
                this._gamesYouHaveOpened.Add(game);
                this.OnMatchRequestAccepted(game);

            }
            if (currentLineBatch.Any(line => line.PureLine.Contains("Creating match") && line.Code == IgsCode.Info))
            {
                // Make it not be an interrupt and let it be handled by the match creator.
                foreach (IgsLine line in currentLineBatch)
                {
                    lock (_mutex)
                    {
                        if (_requestInProgress != null)
                        {
                            _requestInProgress.IncomingLines.Post(line);
                        }
                        else
                        {
                            if (_composure == IgsComposure.Ok)
                            {
                                OnUnhandledLine(line.EntireLine);
                            }
                        }
                    }
                }
            }
            if (currentLineBatch.Count == 3 && currentLineBatch[0].Code == IgsCode.SayInformation &&
                currentLineBatch[1].Code == IgsCode.Say)
            {
                /*
                   51 Say in game 405
                   19 *Soothie*: Hi!
                   1 6
                 */
                int gameNumber = IgsRegex.ParseGameNumberFromSayInformation(currentLineBatch[0]);
                ChatMessage chatLine = IgsRegex.ParseSayLine(currentLineBatch[1]);
                GameInfo relevantGame = _gamesYouHaveOpened.Find(gi => gi.ServerId == gameNumber);
                if (relevantGame == null)
                {
                    throw new Exception("We received a chat message for a game we no longer play.");
                }
                OnIncomingInGameChatMessage(relevantGame, chatLine);
            }
        }
    }
}
