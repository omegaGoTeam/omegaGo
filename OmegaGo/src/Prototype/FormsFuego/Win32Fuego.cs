using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;

namespace FormsFuego
{
    /// <summary>
    ///     This builder creates Fuego instances for the Win32 WinForms prototype.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.AI.FuegoSpace.IGtpEngineBuilder" />
    public class Win32FuegoBuilder : IGtpEngineBuilder
    {
        /// <summary>
        ///     Creates a new Fuego instance by launching a new Fuego.exe process and running the command 'boardsize N' to set the
        ///     board size.
        /// </summary>
        /// <param name="boardSize">Size of the board.</param>
        /// <returns></returns>
        public IGtpEngine CreateEngine(int boardSize)
        {
            var wf = new Win32Fuego();
            wf.RestartProcess();
            if (boardSize != 0)
            {
                // FuegoEngine interprets boardsize 0 to mean "you may change boardsize at any time" so we'll do that here, too.
                wf.SendCommand("boardsize " + boardSize);
            }
            return wf;
        }
    }

    /// <summary>
    ///     A Fuego instance that is run as a Windows process. It expects the file 'fuego.exe' and 'book.dat' to be present in
    ///     the current directory. This code is mostly copied from GameOfGo and we DO NOT have licence to use it. That means
    ///     that this should not be released publicly.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.AI.FuegoSpace.IGtpEngine" />
    public class Win32Fuego : IGtpEngine
    {
        // ReSharper disable once CollectionNeverQueried.Local --this may become useful if it becomes the onyl way to get some information about Fuego thinking
        private readonly List<string> _debugLines = new List<string>();
        private readonly ConcurrentQueue<string> _inputs = new ConcurrentQueue<string>();
        private StreamWriter _writer;
        private Process Process;
        
        public GtpResponse SendCommand(string command)
        {
            WriteCommand(command);
            string code;
            string msg;
            bool success;
            ReadResponse(out code, out success, out msg);
            return new GtpResponse(success, msg);
        }

        public void Dispose()
        {
            
        }

        private void ReadResponse(out string code, out bool success, out string msg)
        {
            success = false;
            code = null;
            msg = null;

            this._debugLines.Clear();

            bool haveResult = false;
            while (true)
            {
                Thread.Sleep(50); // allow more text to come out

                while (!this._inputs.IsEmpty)
                {
                    string line;
                    this._inputs.TryDequeue(out line);
#if DEBUG

                    Debug.WriteLine("Read: " + (line ?? "(NULL)"));
#endif

                    // If empty line, eats it, otherwise parses the line.
                    if (!string.IsNullOrEmpty(line))
                    {
                        switch (line[0])
                        {
                            case '?':
                                // If line starts with '?', indicates an error has occurred in Fuego.
                                haveResult = true;
                                ParseEngineOutput(line, out code, out msg);
                                success = false;
                                break;
                            case '=':
                                // If line starts with '=', no error.
                                haveResult = true;
                                ParseEngineOutput(line, out code, out msg);
                                success = true;
                                break;
                            default:
                                // If line starts with something else, save it.
                                this._debugLines.Add(line);
                                break;
                        }
                    }
                }

                // Hopefully the above Thread.Sleep() delayed enough to get the full response.  The result line (starts with
                // = or ?) can come first, and we put all other lines in _debugLines.
                Thread.Sleep(50);
                if (this._inputs.IsEmpty && haveResult)
                    break;
            }
        } // Parses everything after the first character on a response line.

        private void ParseEngineOutput(string rval, out string id, out string msg)
        {
            if (rval[1] == ' ')
            {
                // code is not present
                id = null;
                msg = rval.Substring(2);
            }
            else
            {
                // code is present
                int strpos = rval.IndexOf(' ', 2);
                id = rval.Substring(1, strpos - 1);
                msg = rval.Substring(strpos + 1);
            }
        }

        private void WriteCommand(string cmd, string value = null)
        {
#if DEBUG

            Debug.WriteLine("COMMAND: " + cmd);
#endif
            this._writer.Write(cmd);
            if (value != null)
            {
                Debug.Write(' ');
                this._writer.Write(' ');
                Debug.Write(value);
                this._writer.Write(value);
            }
            this._writer.Write("\n\n");
            this._writer.Flush();
            Thread.Sleep(10);
        }

        public void RestartProcess()
        {
            // Kill any existing process.
            if (this.Process != null)
            {
                try
                {
                    this.Process.OutputDataReceived -= Process_OutputDataReceived;
                    this.Process.Kill();
                    this.Process = null;
                }
                    // ReSharper disable EmptyGeneralCatchClause
                catch
                    // ReSharper restore EmptyGeneralCatchClause
                {
                }
            }

            // gives exe 1 second to start up, then another 2, then another 4.
            int wait = 1000; // 1 second
            bool success = false;
            for (int t = 0; t < 3; t++) // try a few times to connect, then give up
            {
                try
                {
                    // Create the process.
                    this.Process = new Process
                    {
                        StartInfo =
                        {
                            FileName = "fuego.exe",
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            LoadUserProfile = false
                        }
                    };

                    this.Process.Start();

                    Thread.Sleep(wait); // give exe a chance to start up

                    // This method is much more reliable than trying to read standard output.
                    this.Process.OutputDataReceived += Process_OutputDataReceived;
                    this.Process.BeginOutputReadLine();
                    this.Process.ErrorDataReceived += Process_ErrorDataReceived;
                    this.Process.BeginErrorReadLine();
                    this._writer = this.Process.StandardInput;

                    success = true;
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error connecting to Fuego on attempt #" + t + ". " + ex.Message);
                    this.Process?.Kill();
                    wait *= 2;
                    Thread.Sleep(wait);
                }
            }
            if (!success)
            {
                Debug.WriteLine("Giving up on connecting to Fuego.");
                throw new Exception("Couldn't connect to Fuego.");
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this._inputs.Enqueue(e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this._inputs.Enqueue(e.Data);
        }
    }
}