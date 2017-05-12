using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Connectors.Igs;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;
using Sockets.Plugin;

namespace OmegaGo.Core.Online.Igs
{


    /// <summary>
    /// Represents a connection established with the IGS server. This may not necessarily be a persistent TCP connection, but it retains information
    /// about which user is logged in.
    /// </summary>
    public partial class IgsConnection : IServerConnection
    {
        /*
         * Synchronization
         */

        /// <summary>
        /// Synchronization mutex
        /// </summary>
        private readonly object _mutex = new object();

        /// <summary>
        /// Composure regained mutex
        /// </summary>
        private readonly object _mutexComposureRegained = new object();

        /// <summary>
        /// Composure
        /// </summary>
        private IgsComposure _composureBackingField = IgsComposure.Disconnected;

        /*
         * Status    
         */

        private readonly Dictionary<int, IgsConnector> _availableConnectors = new Dictionary<int, IgsConnector>();

        /// <summary>
        /// List of games that are being observed
        /// </summary>
        internal readonly List<IgsGame> GamesBeingObserved = new List<IgsGame>();
        /// <summary>
        /// List of games opened
        /// </summary>
        internal readonly List<IgsGame> GamesYouHaveOpened = new List<IgsGame>();

        /// <summary>
        /// Logger
        /// </summary>
        internal readonly StringBuilder LogBuilder = new StringBuilder();

        /// <summary>
        /// Outgoing IGS requests
        /// </summary>
        private readonly System.Collections.Concurrent.ConcurrentQueue<IgsRequest> _outgoingRequests =
            new System.Collections.Concurrent.ConcurrentQueue<IgsRequest>();

        /// <summary>
        /// List of incoming match requests
        /// </summary>
        private readonly List<IgsMatchRequest> _incomingMatchRequests = new List<IgsMatchRequest>();

        /*
         * Game state info
         */

        /// <summary>
        /// Game for which are the incoming moves relevant
        /// </summary>
        private IgsGame _incomingMovesAreForThisGame;

        /// <summary>
        /// Indicates the request currently in progress
        /// </summary>
        private IgsRequest _requestInProgress;

        /// <summary>
        /// Host name
        /// </summary>
        private string _hostname;

        /// <summary>
        /// Port
        /// </summary>
        private int _port;

        /// <summary>
        /// Username
        /// </summary>
        private string _username;

        /// <summary>
        /// Password
        /// </summary>
        private string _password;
        
        /// <summary>
        /// Contains the login error info
        /// </summary>
        private string _loginError;

        /*
         * Internal TCP connection objects  
         */

        /// <summary>
        /// The currently established Telnet connection. If it fails, it should automatically restart.
        /// </summary>
        private TcpSocketClient _client;

        /// <summary>
        /// Auto-flushing stream writer that sends commands to the server.
        /// </summary>
        private StreamWriter _streamWriter;

        /// <summary>
        /// This reader receives text lines from the server.
        /// </summary>
        private StreamReader _streamReader;

        /// <summary>
        /// Default IGS connection constructor
        /// </summary>
        public IgsConnection()
        {
            Commands = new IgsCommands(this);
            Data = new IgsData();
            Events = new IgsEvents(this);
        }

    /// <summary>
        /// Checks if  the connection has been established
        /// </summary>
        public bool ConnectionEstablished { get; private set; }

        /// <summary>
        /// Checks if the user has been logged in
        /// </summary>
        public bool LoggedIn { get; set; }

        public ServerId Name => ServerId.Igs;
        
        public IgsCommands Commands { get; }

        public IgsEvents Events { get; }

        public IgsData Data { get; set; }

        /// <summary>
        /// Gets our username, if we're logged in.
        /// </summary>
        public string Username => _username;

        /// <summary>
        /// Gets the active step of the IGS login process. If we're not currently logging in, this property has no meaning.
        /// </summary>
        public IgsLoginPhase CurrentLoginPhase { get; internal set; } = IgsLoginPhase.Connecting;
        
        /// <summary>
        /// Log of Igs
        /// </summary>
        public string Log => this.LogBuilder.ToString();
        
        ICommonCommands IServerConnection.Commands => Commands;

        ICommonEvents IServerConnection.Events => Events;

        /// <summary>
        /// Provides access to IGS composure, ensures monitor pulsing
        /// </summary>
        public IgsComposure Composure
        {
            get { return _composureBackingField; }
            private set
            {
                _composureBackingField = value;
                lock (_mutexComposureRegained)
                {
                    Monitor.PulseAll(_mutexComposureRegained);
                }
            }
        }

        /// <summary>
        /// Connects to IGS at the specified host and port and logs in as a guest account. If we are already connected,
        /// then this method does nothing.
        /// </summary>
        /// <param name="hostname">The hostname to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(string hostname = ServerLocations.IgsPrimary, int port = ServerLocations.IgsPortPrimary)
        {
            _hostname = hostname;
            _port = port;
            try
            {
                if (_client != null && ConnectionEstablished)
                {
                    // We are likely to still be connected.
                    return true;
                }
                _client = new TcpSocketClient();
                try
                {
                    Composure = IgsComposure.InitialHandshake;
                    await _client.ConnectAsync(_hostname, _port);

                    _streamWriter = new StreamWriter(_client.WriteStream);
                    _streamReader = new StreamReader(_client.ReadStream);
                    _streamWriter.AutoFlush = true;
#pragma warning disable 4014
                    HandleIncomingData(_streamReader).ContinueWith(t =>
                    {
                        // Cancel everything.
                        ConnectionLost();
                    }, TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
                    _streamWriter.WriteLine("guest");
                    _streamWriter.WriteLine("toggle client on");
                    await WaitUntilComposureChangesAsync();
                }
                catch
                {
                    Composure = IgsComposure.Disconnected;
                    return false;
                }
                ConnectionEstablished = true;
            }
            catch
            {
                Composure = IgsComposure.Disconnected;
                return false;
            }
            return true;
        }

        private void ConnectionLost()
        {
            LoggedIn = false;
            if (Composure == IgsComposure.Disconnected)
            {
                return;
                // Don't do this twice.
                // Thread safety problems may occur. Oh well, it's networking. Hopefully they won't.
                // If yes, we should eliminate the possibilities for connection loss elsewehere and maybe ensure that a single 
                // point of connection failure exists, possibly in OnFaulted. We'll see.
            }
            _client = null;
            Composure = IgsComposure.Disconnected;
            ConnectionEstablished = false;
            foreach (var game in _availableConnectors)
            {
                game.Value.Disconnect();
            }
            Data.GamesInProgress.Clear();
            Data.OnlineUsers.Clear();
            this.GamesYouHaveOpened.Clear();
            this.GamesBeingObserved.Clear();
            _availableConnectors.Clear();
            IgsRequest notYetHandledRequest;
            while (_outgoingRequests.TryDequeue(out notYetHandledRequest))
            {
                notYetHandledRequest.Disconnect();
            }
            lock (_mutex)
            {
                _requestInProgress?.Disconnect();
                _requestInProgress = null;
            }

            Events.RaiseDisconnected();
        }

        /// <summary>
        /// Disconnects us from the IGS server and closes all IGS games.
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            try
            {
                await _client.DisconnectAsync();
            }
            catch
            {
                // Ignore all TCP errors.
            }
            finally
            {
                ConnectionLost();
            }
        }

        /// <summary>
        /// Unlike other methods, this is not always thread-safe. 
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public async Task<bool> LoginAsync(string username, string password)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            if (password == null) throw new ArgumentNullException(nameof(password));
            try
            {
                Events.RaiseLoginPhaseChanged(IgsLoginPhase.Connecting);
                if (!ConnectionEstablished)
                {
                    if (!await ConnectAsync())
                    {
                        return false;
                    }
                }
                Events.RaiseLoginPhaseChanged(IgsLoginPhase.LoggingIn);
                Composure = IgsComposure.LoggingIn;
                _username = username;
                _password = password;
                _loginError = null;
                ClearConnectionInformation();
                _streamWriter.WriteLine("login");
                _streamWriter.WriteLine(_username);
                _streamWriter.WriteLine(_password);
                await WaitUntilComposureChangesAsync();
                if (Composure == IgsComposure.Confused)
                {
                    await _client.DisconnectAsync();
                    _client = null;
                    Events.RaiseLoginComplete(false);
                    return false;
                }
                if (_loginError != null)
                {
                    Events.OnIncomingLine("LOGIN ERROR: " + _loginError);
                    Events.RaiseLoginComplete(false);
                    return false;
                }
                Events.RaiseLoginPhaseChanged(IgsLoginPhase.SendingInitialBurst);
                await MakeRequestAsync("toggle quiet true");
                await MakeRequestAsync("id omegaGo");
                await MakeRequestAsync("toggle newundo true");
                await MakeRequestAsync("toggle verbose false");
                Events.RaiseLoginPhaseChanged(IgsLoginPhase.RefreshingGames);
                await Commands.ListGamesInProgressAsync();
                Events.RaiseLoginPhaseChanged(IgsLoginPhase.RefreshingUsers);
                await Commands.ListOnlinePlayersAsync();
                Events.RaiseLoginPhaseChanged(IgsLoginPhase.Done);
                LoggedIn = true;
                Events.RaiseLoginComplete(true);
                return true;
            }
            catch
            {
                await DisconnectAsync();
                return false;
            }
        }

        /// <summary>
        /// Enqueues a command to be send to IGS.
        /// </summary>
        /// <param name="command">The single-line command.</param>
        public void MakeUnattendedRequest(string command)
        {
            IgsRequest request = new IgsRequest(command) { Unattended = true };
            _outgoingRequests.Enqueue(request);
            ExecuteRequestFromQueue();
        }

        /// <summary>
        /// Handles incoming resignation
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="whoResigned">Name of the player who resigned</param>
        internal void HandleIncomingResignation(IgsGameInfo gameInfo, string whoResigned)
        {
            var stoneColor = GetStoneColorForPlayerName(gameInfo.IgsIndex, whoResigned);
            if (stoneColor == StoneColor.None) throw new InvalidOperationException("The player resignation is invalid for this game");
            _availableConnectors[gameInfo.IgsIndex].ResignationFromServer(stoneColor);
            DestroyGame(gameInfo);
        }


        /// <summary>
        /// Registers a IGS game connector
        /// </summary>
        /// <param name="connector">Connector</param>
        internal void RegisterConnector(IgsConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            if (_availableConnectors.ContainsKey(connector.GameId))
            {
                // (Petr) Right, so, the way it works is this:
                // At a single moment, there can be only one game with an ID on the server. However, as soon as
                // that game ends (for any reason), the server is free to reassign its ID to a newly created game.
                // This does happen in practice, often immediately, because new games are always being created.
                // The IgsConnection class IS catching most of the messages that cause a game to be deleted and
                // if that happens, it is removed from _gamesYouHaveOpened. A game-deletion message should arrive for 
                // all games that we have opened before we receive any information about a new game with the same ID,
                // BUT I'm certainly not sure that I handle all these messages correctly or that I catch all of them.
                // This part of the protocol (and my implementation in this area) is rather messy.

                // In any case, however, when this method is called, it means that the old connector will
                // never receive another message from the server so we can just overwrite it.
            }
            _availableConnectors[connector.GameId] = connector;
        }

        /// <summary>
        /// Enqueues the <paramref name="command"/> to be sent over Telnet to the IGS SERVER,
        /// then asynchronously receives the entirety of the server's response to this command.
        /// </summary>
        /// <param name="command">The command to send over Telnet.</param>
        /// <returns></returns>
        internal async Task<IgsResponse> MakeRequestAsync(string command)
        {
            IgsRequest request = new IgsRequest(command);
            _outgoingRequests.Enqueue(request);
            ExecuteRequestFromQueue();
            IgsResponse lines = await request.GetAllLines();
            lock (_mutex)
            {
                _requestInProgress = null;
            }
            ExecuteRequestFromQueue();
            return lines;

        }

        internal void DestroyGame(IgsGameInfo gameInfo)
        {

            this.GamesYouHaveOpened.Remove(this.GamesYouHaveOpened.FirstOrDefault(g => g.Info.IgsIndex == gameInfo.IgsIndex));
            // We will not delete it from _availableConnectors yet, I think it will be safer this way.
            // If we encounter problems, we'll deal with them then.
        }

        /// <summary>
        /// This method is called whenever a new command request is enqueued to be sent to
        ///  the IGS SERVER and also whenever
        /// a command request becomes completed. The method will determine whether the channel
        ///  is currently free (i.e. no other command is being executed) and
        ///  if so, if there are any command requests in the queue,
        ///  the earliest one is dequeued and executed.
        /// </summary>
        private void ExecuteRequestFromQueue()
        {

            lock (_mutex)
            {
                if (Composure != IgsComposure.Ok)
                {
                    return; // Cannot yet send requests.
                }
                if (_requestInProgress == null)
                {
                    IgsRequest dequeuedItem;
                    if (_outgoingRequests.TryDequeue(out dequeuedItem))
                    {
                        if (!dequeuedItem.Unattended)
                        {
                            _requestInProgress = dequeuedItem;
                        }
                        Events.OnOutgoingLine(dequeuedItem.Command);
                        LogBuilder.AppendLine("> " + dequeuedItem.Command);
                        _streamWriter.WriteLine(dequeuedItem.Command);
                        if (dequeuedItem.Unattended)
                        {
                            ExecuteRequestFromQueue();
                        }
                    }
                }
            }
        }


        private void ClearConnectionInformation()
        {
            IgsRequest throwaway;
            while (_outgoingRequests.TryDequeue(out throwaway))
            {
            }
            _incomingMovesAreForThisGame = null;
            this.Data = new IgsData();
            this.GamesBeingObserved.Clear();
            this.GamesYouHaveOpened.Clear();
            // _gamesInProgressOnIgs.Clear();
        }

      

        private Task WaitUntilComposureChangesAsync()
        {
            IgsComposure originalComposure = Composure;
            return Task.Run(() =>
            {
                lock (_mutexComposureRegained)
                {
                    while (Composure == originalComposure)
                    {
                        Monitor.Wait(_mutexComposureRegained);
                    }
                }
            });
        }

        private IgsCode ExtractCodeFromLine(string line)
        {
            if (line != "")
            {
                string[] split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string firstWord = split[0];
                int firstWordAsInteger;
                if (int.TryParse(firstWord, out firstWordAsInteger))
                {
                    IgsCode code = (IgsCode)firstWordAsInteger;
                    return code;
                }
            }
            return IgsCode.Unknown;
        }

        private StoneColor GetStoneColorForPlayerName(int igsGameIndex, string playerName)
        {
            var game = this.GamesYouHaveOpened.Find(og => og.Info.IgsIndex == igsGameIndex);
            var player = game.Controller.Players.FirstOrDefault(p => p.Info.Name == playerName);
            return player?.Info.Color ?? StoneColor.None;
        }

        private void HandleIncomingShoutMessage(string line)
        {
            Events.OnIncomingShoutMessage(line);
        }

        private void HandleIncomingChatMessage(string line)
        {
            Events.OnIncomingChatMessage(line);
        }





        private void HandleIncomingMove(IgsGame game, int moveIndex, Move theMove)
        {
            _availableConnectors[game.Info.IgsIndex].MoveFromServer(moveIndex, theMove);
        }

        private IgsConnector GetConnector(IgsGameInfo gameinfo)
        {
            return _availableConnectors[gameinfo.IgsIndex];
        }

        private void OnIncomingHandicapInformation(IgsGame game, int stoneCount)
        {
            if (game != null)
            {
                _availableConnectors[game.Info.IgsIndex].HandicapFromServer(stoneCount);
            }
        }


        private void ScoreGame(IgsGame gameInfo, float blackScore, float whiteScore)
        {
            Scores scores = new Scores(blackScore, whiteScore);
            GamePlayer winner = null;
            if (scores.BlackScore > scores.WhiteScore)
            {
                winner = gameInfo.Controller.Players.Black;
            }
            else if (scores.BlackScore < scores.WhiteScore)
            {
                winner = gameInfo.Controller.Players.White;
            }
            else
            {
                winner = null;
            }
            if (winner != null)
            {
                GetConnector(gameInfo.Info).EndTheGame(
                    GameEndInformation.CreateScoredGame(winner, gameInfo.Controller.Players.GetOpponentOf(winner),
                        scores));

            }
            else
            {
                GetConnector(gameInfo.Info).EndTheGame(
                    GameEndInformation.CreateDraw(gameInfo.Controller.Players, scores));
            }
        }
        private void OnIncomingStoneRemoval(int gameNumber, Position deadPosition)
        {
            var game = this.GamesYouHaveOpened.Find(og => og.Info.IgsIndex == gameNumber);
            GetConnector(game.Info).ForceLifeDeathKillGroup(deadPosition);
        }

        private void ResignObservedGame(int gameInWhichSomebodyResigned, StoneColor whoResigned)
        {
            var game = GamesYouHaveOpened.FirstOrDefault(gm => gm.Info.IgsIndex == gameInWhichSomebodyResigned);
            if (game != null)
            {
                game.Controller.EndGame(GameEndInformation.CreateResignation(game.Controller.Players[whoResigned],
                    game.Controller.Players));
                DestroyGame(game.Info);
            }
        }
    }
}
