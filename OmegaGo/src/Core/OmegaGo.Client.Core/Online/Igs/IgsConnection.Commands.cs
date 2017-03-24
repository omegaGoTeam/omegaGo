using OmegaGo.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time;
using OmegaGo.Core.Time.Canadian;
using OmegaGo.Core.Time.None;

namespace OmegaGo.Core.Online.Igs
{
    partial class IgsConnection
    {
        private async Task<IgsGameInfo> GetGameByIdAsync(int gameId)
        {
            IgsResponse response = await MakeRequestAsync("games " + gameId);
            foreach (IgsLine line in response)
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

        public async Task<List<IgsGameInfo>> ListGamesInProgressAsync()
        {
            await EnsureConnectedAsync();
            var games = new List<IgsGameInfo>();
            List<IgsLine> lines = await MakeRequestAsync("games");
            foreach (IgsLine line in lines)
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
            this.Data.GamesInProgress = games;
            return games; 
        }
        private IgsGameInfo CreateGameFromTelnetLine(string line)
        {
            Regex regex = new Regex(@"7 \[ *([0-9]+)] *([^[]+) \[([^]]+)\] vs. *([^[]+) \[([^]]+)\] \( *([0-9]+) *([0-9]+) *([0-9]+) *([-0-9.]+) *([0-9]+) *([A-Z]*)\) *\( *([0-9]+)\)");
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
            Match match = regex.Match(line);
            IgsGameInfo game = new IgsGameInfo(
                new PlayerInfo(StoneColor.White, match.Groups[2].Value, match.Groups[3].Value),
                new PlayerInfo(StoneColor.Black, match.Groups[4].Value, match.Groups[5].Value),
                new GameBoardSize(match.Groups[7].Value.AsInteger()),
                RulesetType.Japanese,
                match.Groups[8].Value.AsInteger(),
                HandicapPlacementType.Fixed,
                match.Groups[9].Value.AsFloat(),
                CountingType.Territory,
                match.Groups[1].Value.AsInteger(),
                match.Groups[12].Value.AsInteger(),
                this);
            game.ByoyomiPeriod = match.Groups[10].Value.AsInteger();
            game.PreplayedMoveCount = match.Groups[6].Value.AsInteger();
            return game;

        }

        public async Task<IgsGame> StartObserving(IgsGameInfo gameInfo)
        {
            if (this._gamesBeingObserved.Any(g => g.Info.IgsIndex == gameInfo.IgsIndex))
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
            GameHeading heading = response.GetGameHeading();
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

            IgsLine titleLine = response.LastOrDefault(line => line.Code == IgsCode.Info);
            string gameName = null;
            if (titleLine != null)
            {
                gameName = IgsRegex.ParseTitleInformation(titleLine);
            }
            GamePlayer blackPlayer =
                  new IgsPlayerBuilder(StoneColor.Black, this)
                      .Name(gameInfo.Black.Name)
                      .Rank(gameInfo.Black.Rank)
                      .Clock(blackClock)
                      .Build();
            GamePlayer whitePlayer =
                new IgsPlayerBuilder(StoneColor.White, this)
                    .Name(gameInfo.White.Name)
                    .Rank(gameInfo.White.Rank)
                    .Clock(whiteClock)
                    .Build();
            var onlineGame = GameBuilder.CreateOnlineGame(gameInfo)
                .Connection(this)
                .BlackPlayer(blackPlayer)
                .WhitePlayer(whitePlayer)
                .Ruleset(RulesetType.Japanese)
                .Komi(gameInfo.Komi)
                .BoardSize(gameInfo.BoardSize)
                .Name(gameName)
                .Build();
            _gamesBeingObserved.Add(onlineGame);
            _gamesYouHaveOpened.Add(onlineGame);
            MakeUnattendedRequest("moves " + gameInfo.IgsIndex);
            return onlineGame;
        }
        /// <summary>
        /// If we are observing the given game, the observation ends. If we're not, nothing happens.
        /// </summary>
        /// <param name="game">The game we're observing.</param>
        /// <returns>True if we succeeded in ending observation, false if we were not observing that game or the game already ended.</returns>
        public async Task<bool> EndObserving(IgsGame game)
        {

            if (!this._gamesBeingObserved.Contains(game))
            {
                // We're not observing this game.
                return false;
            }
            this.OneUnobserveExpected = false;
            var response = await MakeRequestAsync("unobserve " + game.Info.IgsIndex);
            this._gamesBeingObserved.Remove(game);
            _gamesYouHaveOpened.Remove(game);
            return !response.IsError;
        }

        /// <summary>
        /// Sends a private message to the specified user using the 'tell' feature of IGS.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="message">The message.</param>
        /// <returns>True if the message was delivered.</returns>
        public async Task<bool> TellAsync(string recipient, string message)
        {
            await EnsureConnectedAsync();
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
                IgsUser createdUser = CreateUserFromTelnetLine(line.EntireLine);
                if (createdUser.Name == this._username)
                {
                    OnPersonalInformationUpdate(createdUser);
                }
            }
        }

        public async Task<List<IgsUser>> ListOnlinePlayersAsync(string specificUser = null)
        {
            await EnsureConnectedAsync();
            List<IgsLine> users = await MakeRequestAsync("user");
            var returnedUsers = new List<IgsUser>();
            foreach (var line in users)
            {
                if (line.Code != IgsCode.User) continue; // Comment
                if (line.EntireLine.EndsWith("Language")) continue; // Example
                IgsUser createdUser = CreateUserFromTelnetLine(line.EntireLine);
                returnedUsers.Add(createdUser);
                if (createdUser.Name == this._username)
                {
                    OnPersonalInformationUpdate(createdUser);
                }
            }
            this.Data.OnlineUsers = returnedUsers;
            return returnedUsers;
        }

        /// <summary>
        /// Uses the command "match" to challenge a player.
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
                MakeRequestAsync("match " + opponent + " " + yourColor.ToIgsCharacterString() + " " + boardSize.ToString() +
                            " " + mainTime.ToString() + " " + byoyomiMinutes.ToString());
            return !lines.IsError;
        }

        public async Task<bool> DeclineMatchRequestAsync(IgsMatchRequest matchRequest)
        {
            var response = await MakeRequestAsync(matchRequest.RejectCommand);
            return !response.IsError;
        }
        public async Task<IgsGame> AcceptMatchRequestAsync(IgsMatchRequest matchRequest)
        {
            var lines = await MakeRequestAsync(matchRequest.AcceptCommand);
            if (lines.IsError) return null;
            GameHeading heading = IgsRegex.ParseGameHeading(lines[0]);
            var ogi = await GetGameByIdAsync(heading.GameNumber);
            var builder = GameBuilder.CreateOnlineGame(ogi).Connection(this);
            bool youAreBlack = heading.BlackName == this._username;
            var humanPlayer =
                new HumanPlayerBuilder(youAreBlack ? StoneColor.Black : StoneColor.White).Name(youAreBlack
                    ? heading.BlackName
                    : heading.WhiteName)
                    .Rank(youAreBlack ? ogi.Black.Rank : ogi.White.Rank)
                    .Clock(new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(ogi.ByoyomiPeriod)))
                    .Build();
            var onlinePlayer =
                new IgsPlayerBuilder(youAreBlack ? StoneColor.White : StoneColor.Black, this).Name(youAreBlack
                    ? heading.WhiteName
                    : heading.BlackName)
                    .Rank(youAreBlack ? ogi.White.Rank : ogi.Black.Rank)
                    .Clock(new CanadianTimeControl(TimeSpan.Zero, 25, TimeSpan.FromMinutes(ogi.ByoyomiPeriod)))
                    .Build();
            builder.BlackPlayer(youAreBlack ? humanPlayer : onlinePlayer)
                .WhitePlayer(youAreBlack ? onlinePlayer : humanPlayer);
            var game = builder.Build();
            _gamesYouHaveOpened.Add(game);
            return game;
        }


        public void MadeMove(IgsGameInfo game, Move move)
        {
            switch (move.Kind)
            {
                case MoveKind.PlaceStone:
                    MakeUnattendedRequest(move.Coordinates.ToIgsCoordinates() + " " + game.IgsIndex);
                    break;
                case MoveKind.Pass:
                    MakeUnattendedRequest("pass " + game.IgsIndex);
                    break;
            }
        }



        public async Task<bool> SayAsync(IgsGame game, string chat)
        {
            if (!_gamesYouHaveOpened.Contains(game))
                throw new ArgumentException("You don't have this game opened on IGS.");
            if (chat == null) throw new ArgumentNullException(nameof(chat));
            if (chat == "") throw new ArgumentException("Chat line must not be empty.");
            if (chat.Contains("\n")) throw new Exception("Chat lines on IGS must not contain line breaks.");

            IgsResponse response;
            if (_gamesYouHaveOpened.Count > 1)
            {
                // More than one game is opened: we must give the game id.
                response = await MakeRequestAsync("say " + game.Info.IgsIndex + " " + chat);
            }
            else
            {
                // We have only one game opened: game id MUST NOT be given
                response = await MakeRequestAsync("say " + chat);
            }
            return !response.IsError;
        }


        public async Task UndoPleaseAsync(IgsGameInfo game)
        {
            await MakeRequestAsync("undoplease " + game.IgsIndex);
        }

        public async Task UndoAsync(IgsGameInfo game)
        {
            await MakeRequestAsync("undo " + game.IgsIndex);
        }

        public void NoUndo(IgsGameInfo game)
        {
            MakeUnattendedRequest("noundo " + game.IgsIndex);
        }


        public async Task<bool> ToggleAsync(string toggleKey, bool newToggleValue)
        {
            var response = await MakeRequestAsync("toggle " + toggleKey + " " + (newToggleValue ? "on" : "off"));
            return !response.IsError;
        }
    }
}
