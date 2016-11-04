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

namespace OmegaGo.Core.Online.Igs
{
    partial class IgsConnection
    {
        private async Task HandleIncomingData(StreamReader sr)
        {
            bool weAreHandlingAnInterruptMessage = false;
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

                if (weAreHandlingAnInterruptMessage && code == IgsCode.Prompt)
                {
                    // Interrupt message is over, let's wait for a new message
                    weAreHandlingAnInterruptMessage = false;
                    continue;
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
                if (code == IgsCode.Shout)
                {
                    HandleIncomingShoutMessage(line);
                    weAreHandlingAnInterruptMessage = true;
                    continue;
                }
                if (code == IgsCode.Move)
                {
                    HandleIncomingMove(igsLine);
                    weAreHandlingAnInterruptMessage = true;
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

    }
}
