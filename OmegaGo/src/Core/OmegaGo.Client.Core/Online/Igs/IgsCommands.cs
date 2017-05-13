using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time;
using OmegaGo.Core.Time.Canadian;
using OmegaGo.Core.Time.None;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsCommands : ICommonCommands
    {
        private static Task CompletedTask = Task.FromResult(0);


        private IgsConnection igsConnection;

        public IgsCommands(IgsConnection igsConnection)
        {
            this.igsConnection = igsConnection;
        }

        public Task MakeMove(RemoteGameInfo remoteInfo, Move move)
        {
            var game = (IgsGameInfo) remoteInfo;
            switch (move.Kind)
            {
                case MoveKind.PlaceStone:
                    this.igsConnection.MakeUnattendedRequest(move.Coordinates.ToIgsCoordinates() + " " + game.IgsIndex);
                    break;
                case MoveKind.Pass:
                    this.igsConnection.MakeUnattendedRequest("pass " + game.IgsIndex);
                    break;
            }
            return IgsCommands.CompletedTask;
        }

        public Task AddTime(RemoteGameInfo remoteInfo, TimeSpan additionalTime)
        {
            if (additionalTime.Seconds != 0)
            {
                throw new ArgumentException("IGS only supports adding whole minutes", nameof(additionalTime));
            }
            var igsGameInfo = (IgsGameInfo) remoteInfo;
            this.igsConnection.MakeUnattendedRequest("addtime " + igsGameInfo.IgsIndex + " " + additionalTime.Minutes);
            return IgsCommands.CompletedTask;
        }

        public Task UndoLifeDeath(RemoteGameInfo remoteInfo)
        {
            var igsGameInfo = (IgsGameInfo) remoteInfo;
            this.igsConnection.MakeUnattendedRequest("undo " + igsGameInfo.IgsIndex);
            return IgsCommands.CompletedTask;
        }

        public async Task LifeDeathDone(RemoteGameInfo remoteInfo)
        {
            var igsGameInfo = (IgsGameInfo) remoteInfo;
            await this.igsConnection.MakeRequestAsync("done " + igsGameInfo.IgsIndex);
        }

        public async Task LifeDeathMarkDeath(Position position, RemoteGameInfo remoteInfo)
        {
            var igsGameInfo = (IgsGameInfo) remoteInfo;
            await this.igsConnection.MakeRequestAsync(position.ToIgsCoordinates() + " " + igsGameInfo.IgsIndex);
        }

        public async Task Resign(RemoteGameInfo remoteInfo)
        {
            var igsGameInfo = (IgsGameInfo) remoteInfo;
            var response = await this.igsConnection.MakeRequestAsync("resign " + igsGameInfo.IgsIndex);
            if (!response.IsError)
            {
                this.igsConnection.HandleIncomingResignation(igsGameInfo, this.igsConnection.Username);
            }
        }

        public async Task AllowUndoAsync(RemoteGameInfo remoteInfo)
        {
            var igsGameInfo = (IgsGameInfo)remoteInfo;
            await MakeRequestAsync("undo " + igsGameInfo.IgsIndex);

        }

        public Task RejectUndoAsync(RemoteGameInfo remoteInfo)
        {
            var igsGameInfo = (IgsGameInfo)remoteInfo;
            this.igsConnection.MakeUnattendedRequest("noundo " + igsGameInfo.IgsIndex);
            return CompletedTask;
        }

        public async Task UnobserveAsync(RemoteGameInfo remoteInfo)
        {
            var igsGameInfo = (IgsGameInfo) remoteInfo;
            var igsGame = igsConnection.GamesBeingObserved.FirstOrDefault(gm => gm.Info.IgsIndex == igsGameInfo.IgsIndex);
            if (igsGame != null)
            {
                await this.EndObserving(igsGame);
            }
        }

        public void AreYouThere()
        {
            if (this.igsConnection.LoggedIn)
            {
                this.igsConnection.MakeUnattendedRequest("ayt");
            }
        }

        public async Task<List<IgsGameInfo>> ListGamesInProgressAsync()
        {
            var games = new List<IgsGameInfo>();
            List<IgsLine> lines = await MakeRequestAsync("games");
            foreach (var line in lines)
            {
                if (line.Code == IgsCode.Games)
                {
                    if (line.EntireLine.Contains("[##]"))
                    {
                        // This is the example line.
                        continue;
                    }
                    games.Add(CreateGameFromTelnetLine(line.EntireLine));
                }
            }
            this.igsConnection.Data.GamesInProgress = games;
            return games;
        }

        public async Task<IgsGame> StartObserving(IgsGameInfo gameInfo)
        {
            if (this.igsConnection.GamesBeingObserved.Any(g => g.Info.IgsIndex == gameInfo.IgsIndex))
            {
                // We are already observing this game.
                return null;
            }
            var response = await MakeRequestAsync("observe " + gameInfo.IgsIndex);
            if (response.IsError)
            {
                // Observing failed.
                return null;
            }
            var heading = response.GetGameHeading();
            if (heading == null)
            {
                return null;
            }
            if (heading.BlackName != gameInfo.Black.Name || heading.WhiteName != gameInfo.White.Name)
            {
                // It's a different game now.
                return null;
            }
            TimeControl blackClock =
                new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(gameInfo.ByoyomiPeriod)).UpdateFrom(
                    heading.BlackTimeRemaining);
            TimeControl whiteClock =
                new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(gameInfo.ByoyomiPeriod)).UpdateFrom(
                    heading.WhiteTimeRemaining);
            if (heading.BlackTimeRemaining.PeriodStonesLeft == 0 &&
                heading.BlackTimeRemaining.PeriodTimeLeft == TimeSpan.Zero &&
                heading.BlackTimeRemaining.MainTimeLeft == TimeSpan.Zero)
            {
                blackClock = new NoTimeControl();
                whiteClock = new NoTimeControl();
            }

            var titleLine = response.LastOrDefault(line => line.Code == IgsCode.Info);
            string gameName = null;
            if (titleLine != null)
            {
                gameName = IgsRegex.ParseTitleInformation(titleLine);
            }
            var blackPlayer =
                new IgsPlayerBuilder(StoneColor.Black, this.igsConnection)
                    .Name(gameInfo.Black.Name)
                    .Rank(gameInfo.Black.Rank)
                    .Clock(blackClock)
                    .Build();
            var whitePlayer =
                new IgsPlayerBuilder(StoneColor.White, this.igsConnection)
                    .Name(gameInfo.White.Name)
                    .Rank(gameInfo.White.Rank)
                    .Clock(whiteClock)
                    .Build();
            var onlineGame = GameBuilder.CreateOnlineGame(gameInfo)
                .Connection(this.igsConnection)
                .BlackPlayer(blackPlayer)
                .WhitePlayer(whitePlayer)
                .Ruleset(RulesetType.Japanese)
                .Komi(gameInfo.Komi)
                .BoardSize(gameInfo.BoardSize)
                .Name(gameName)
                .Build();
            this.igsConnection.GamesBeingObserved.Add(onlineGame);
            this.igsConnection.GamesYouHaveOpened.Add(onlineGame);
            this.igsConnection.MakeUnattendedRequest("moves " + gameInfo.IgsIndex);
            return onlineGame;
        }

        /// <summary>
        ///     If we are observing the given game, the observation ends. If we're not, nothing happens.
        /// </summary>
        /// <param name="game">The game we're observing.</param>
        /// <returns>
        ///     True if we succeeded in ending observation, false if we were not observing that game or the game already
        ///     ended.
        /// </returns>
        public async Task<bool> EndObserving(IgsGame game)
        {
            if (!this.igsConnection.GamesBeingObserved.Contains(game))
            {
                // We're not observing this game.
                return false;
            }
            var response = await MakeRequestAsync("unobserve " + game.Info.IgsIndex);
            this.igsConnection.GamesBeingObserved.Remove(game);
            this.igsConnection.GamesYouHaveOpened.Remove(game);
            return !response.IsError;
        }

        /// <summary>
        ///     Sends a private message to the specified user using the 'tell' feature of IGS.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="message">The message.</param>
        /// <returns>True if the message was delivered.</returns>
        public async Task<bool> TellAsync(string recipient, string message)
        {
            List<IgsLine> result = await MakeRequestAsync("tell " + recipient + " " + message);
            return result.All(line => line.Code != IgsCode.Error);
        }

        public async Task RequestPersonalInformationUpdate(string username)
        {
            List<IgsLine> users = await MakeRequestAsync("user " + username);
            foreach (var line in users)
            {
                if (line.Code != IgsCode.User) continue; // Comment
                if (line.EntireLine.EndsWith("Language")) continue; // Example
                var createdUser = CreateUserFromTelnetLine(line.EntireLine);
                if (createdUser.Name == this.igsConnection.Username)
                {
                    this.igsConnection.Events.OnPersonalInformationUpdate(createdUser);
                }
            }
        }

        public async Task<List<IgsUser>> ListOnlinePlayersAsync()
        {
            List<IgsLine> users = await MakeRequestAsync("user");
            var returnedUsers = new List<IgsUser>();
            foreach (var line in users)
            {
                if (line.Code != IgsCode.User) continue; // Comment
                if (line.EntireLine.EndsWith("Language")) continue; // Example
                var createdUser = CreateUserFromTelnetLine(line.EntireLine);
                returnedUsers.Add(createdUser);
                if (createdUser.Name == this.igsConnection.Username)
                {
                    this.igsConnection.Events.OnPersonalInformationUpdate(createdUser);
                }
            }
            this.igsConnection.Data.OnlineUsers = returnedUsers;
            return returnedUsers;
        }

        /// <summary>
        ///     Uses the command "match" to challenge a player.
        /// </summary>
        /// <param name="opponent">The opponent to play against.</param>
        /// <param name="yourColor">Your color.</param>
        /// <param name="boardSize">Size of the square board.</param>
        /// <param name="mainTime">The main time, in minutes.</param>
        /// <param name="byoyomiMinutes">Time for each period of 25 moves, in minutes, for Canadian byoyomi.</param>
        /// <returns></returns>
        public async Task<bool> RequestBasicMatchAsync(
            string opponent,
            StoneColor yourColor,
            int boardSize,
            int mainTime,
            int byoyomiMinutes)
        {
            var lines = await
                MakeRequestAsync("match " + opponent + " " + yourColor.ToIgsCharacterString() + " " + boardSize +
                                 " " + mainTime + " " + byoyomiMinutes);
            return !lines.IsError;
        }

        public async Task<bool> DeclineMatchRequestAsync(IgsMatchRequest matchRequest)
        {
            var response = await MakeRequestAsync(matchRequest.RejectCommand);
            return !response.IsError;
        }

        public async Task<IgsGame> AcceptMatchRequestAsync(IgsMatchRequest matchRequest)
        {
            // We are accepting a match and it begins.
            var lines = await MakeRequestAsync(matchRequest.AcceptCommand);
            if (lines.IsError) return null;
            if (lines.Any(ln => ln.Code == IgsCode.Info && ln.PureLine.Contains("Requesting")))
            {
                this.igsConnection.Events.OnErrorMessageReceived("Requesting " + matchRequest.OpponentName +
                                                                 " to confirm match.");
                return null;
            }
            var heading = this.igsConnection.Data.LastReceivedGameHeading;
            var ogi = await GetGameByIdAsync(heading.GameNumber);
            var builder = GameBuilder.CreateOnlineGame(ogi).Connection(this.igsConnection);
            bool youAreBlack = heading.BlackName == this.igsConnection.Username;
            var humanPlayer =
                new HumanPlayerBuilder(youAreBlack ? StoneColor.Black : StoneColor.White).Name(youAreBlack
                    ? heading.BlackName
                    : heading.WhiteName)
                    .Rank(youAreBlack ? ogi.Black.Rank : ogi.White.Rank)
                    .Clock(new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(ogi.ByoyomiPeriod)))
                    .Build();
            var onlinePlayer =
                new IgsPlayerBuilder(youAreBlack ? StoneColor.White : StoneColor.Black, this.igsConnection).Name(
                    youAreBlack
                        ? heading.WhiteName
                        : heading.BlackName)
                    .Rank(youAreBlack ? ogi.White.Rank : ogi.Black.Rank)
                    .Clock(new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(ogi.ByoyomiPeriod)))
                    .Build();
            builder.BlackPlayer(youAreBlack ? humanPlayer : onlinePlayer)
                .WhitePlayer(youAreBlack ? onlinePlayer : humanPlayer);
            var game = builder.Build();
            this.igsConnection.GamesYouHaveOpened.Add(game);
            return game;
        }


        public async Task<bool> SayAsync(IgsGameInfo game, string chat)
        {            
            if (this.igsConnection.GamesYouHaveOpened.All(g => g.Info.IgsIndex != game.IgsIndex))
            {
                // Game is already over.
                return false;
            }
            if (chat == null) throw new ArgumentNullException(nameof(chat));
            if (chat == "") throw new ArgumentException("Chat line must not be empty.");
            if (chat.Contains("\n")) throw new Exception("Chat lines on IGS must not contain line breaks.");

            IgsResponse response;
            if (this.igsConnection.GamesYouHaveOpened.Count > 1)
            {
                // More than one game is opened: we must give the game id.
                response = await MakeRequestAsync("say " + game.IgsIndex + " " + chat);
            }
            else
            {
                // We have only one game opened: game id MUST NOT be given
                response = await MakeRequestAsync("say " + chat);
            }
            return !response.IsError;
        }

        public async Task<bool> KibitzAsync(IgsGameInfo game, string chat)
        {
            if (this.igsConnection.GamesYouHaveOpened.All(g => g.Info.IgsIndex != game.IgsIndex))
            {
                // Game is already over.
                return false;
            }
            if (chat == null) throw new ArgumentNullException(nameof(chat));
            if (chat == "") throw new ArgumentException("Chat line must not be empty.");
            if (chat.Contains("\n")) throw new Exception("Chat lines on IGS must not contain line breaks.");

            IgsResponse response;
            if (this.igsConnection.GamesYouHaveOpened.Count > 1)
            {
                // More than one game is opened: we must give the game id.
                response = await MakeRequestAsync("kibitz " + game.IgsIndex + " " + chat);
            }
            else
            {
                // We have only one game opened: game id MUST NOT be given
                response = await MakeRequestAsync("kibitz " + chat);
            }
            return !response.IsError;
        }

        public async Task UndoPleaseAsync(IgsGameInfo game)
        {
            await MakeRequestAsync("undoplease " + game.IgsIndex);
        }


        public async Task<bool> ToggleAsync(string toggleKey, bool newToggleValue)
        {
            var response = await MakeRequestAsync("toggle " + toggleKey + " " + (newToggleValue ? "on" : "off"));
            return !response.IsError;
        }

        internal async Task<IgsGameInfo> GetGameByIdAsync(int gameId)
        {
            var response = await MakeRequestAsync("games " + gameId);
            foreach (var line in response)
            {
                if (line.Code == IgsCode.Games)
                {
                    if (line.EntireLine.Contains("[##]"))
                    {
                        // This is the example line.
                        continue;
                    }
                    return CreateGameFromTelnetLine(line.EntireLine);
                }
            }
            throw new Exception("No game with this ID.");
        }

        private async Task<IgsResponse> MakeRequestAsync(string request)
        {
            return await this.igsConnection.MakeRequestAsync(request);
        }

        private IgsGameInfo CreateGameFromTelnetLine(string line)
        {
            var regex =
                new Regex(
                    @"7 \[ *([0-9]+)] *([^[]+) \[([^]]+)\] vs. *([^[]+) \[([^]]+)\] \( *([0-9]+) *([0-9]+) *([0-9]+) *([-0-9.]+) *([0-9]+) *([A-Z]*)\) *\( *([0-9]+)\)");
            // The regex means:
            /*
             * 1 - game id
             * 2 - white name
             * 3 - white rank
             * 4 - black name
             * 5 - black rank
             * 6 - number of moves played
             * 7 - board size
             * 8 - handicap stones
             * 9 - komi points
             * 10 - byoyomi period
             * 11 - flags
             * 12 - number of observers 
             */
            var match = regex.Match(line);
            var game = new IgsGameInfo(
                new PlayerInfo(StoneColor.White, match.Groups[2].Value, match.Groups[3].Value),
                new PlayerInfo(StoneColor.Black, match.Groups[4].Value, match.Groups[5].Value),
                new GameBoardSize(match.Groups[7].Value.AsInteger()),
                RulesetType.Japanese,
                match.Groups[8].Value.AsInteger(),
                HandicapPlacementType.Fixed,
                match.Groups[9].Value.AsFloat(),
                CountingType.Territory,
                match.Groups[1].Value.AsInteger(),
                match.Groups[12].Value.AsInteger());
            game.ByoyomiPeriod = match.Groups[10].Value.AsInteger();
            return game;
        }

        private readonly Regex _regexUser =
            new Regex(
                @"42 +([^ ]+) +.* ([A-Za-z-.].{6})  (...)(\*| ) [^/]+/ *[^ ]+ +[^ ]+ +[^ ]+ +[^ ]+ +([^ ]+) default",
                RegexOptions.None);
        private IgsUser CreateUserFromTelnetLine(string line)
        {
            var match = this._regexUser.Match(line);
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

            var user = new IgsUser
            {
                Name = match.Groups[1].Value,
                Country = match.Groups[2].Value,
                Rank =
                    match.Groups[3].Value.StartsWith(" ") ? match.Groups[3].Value.Substring(1) : match.Groups[3].Value,
                LookingForAGame = match.Groups[5].Value.Contains("!"),
                RejectsRequests = match.Groups[5].Value.Contains("X")
            };
            return user;
        }
    }
}