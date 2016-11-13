using OmegaGo.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.Online.Igs.Structures;

namespace OmegaGo.Core.Online.Igs
{
    partial class IgsConnection
    {
        public override async Task<List<Game>> ListGamesInProgress()
        {
            await EnsureConnected();
            _gamesInProgressOnIgs = new List<Game>();
            List<IgsLine> lines = await MakeRequest("games");
            foreach (IgsLine line in lines)
            {
                if (line.Code == IgsCode.Games)
                {
                    if (line.EntireLine.Contains("[##]"))
                    {
                        // This is the example line.
                        continue;
                    }
                    _gamesInProgressOnIgs.Add(CreateGameFromTelnetLine(line.EntireLine));
                }
            }
            return _gamesInProgressOnIgs;
        }
        private Game CreateGameFromTelnetLine(string line)
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
            try
            {
                Game game = new Game()
                {
                    ServerId = match.Groups[1].Value.AsInteger(),
                    Server = this,
                    Players = new List<Player>(),
                    SquareBoardSize = match.Groups[7].Value.AsInteger(),
                    NumberOfHandicapStones = match.Groups[8].Value.AsInteger(),
                    KomiValue = match.Groups[9].Value.AsFloat(),
                    NumberOfObservers = match.Groups[12].Value.AsInteger()
                };
                game.Players.Add(

                    new Player(match.Groups[4].Value, match.Groups[5].Value, game)
                    {
                        Agent = new OnlineAgent()
                    });
                game.Players.Add(
                    new Player(match.Groups[2].Value, match.Groups[3].Value, game)
                    {
                        Agent = new OnlineAgent()
                    });
                    // DO *NOT* DO this: the displayed number might be something different from what our client wants
                    // NumberOfMovesPlayed = match.Groups[6].Value.AsInteger(),
                    // Do not uncomment the preceding line. I will fix it in time. I hope.
                
                return game;
            }
            catch (FormatException)
            {
                Debug.WriteLine(line);
                return new Game();
            }

        }
        public override async void StartObserving(Game game)
        {
            if (_gamesBeingObserved.Contains(game))
            {
                // We are already observing this game.
                return;
            }
            _gamesBeingObserved.Add(game);
            await MakeRequest("observe " + game.ServerId);
        }
        public override void EndObserving(Game game)
        {
            if (!_gamesBeingObserved.Contains(game))
            {
                throw new ArgumentException("The specified game is currently not being observed.", nameof(game));
            }
            _gamesBeingObserved.Remove(game);
            _streamWriter.WriteLine("observe " + game.ServerId);
        }
        /// <summary>
        /// Sends a private message to the specified user using the 'tell' feature of IGS.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="message">The message.</param>
        /// <returns>True if the message was delivered.</returns>
        public async Task<bool> Tell(string recipient, string message)
        {
            await EnsureConnected();
            List<IgsLine> result = await MakeRequest("tell " + recipient + " " + message);
            return result.All(line => line.Code != IgsCode.Error);
        }
        public async Task<List<IgsUser>> ListOnlinePlayers()
        {
            await EnsureConnected();
            List<IgsLine> users = await MakeRequest("user");
            var returnedUsers = new List<IgsUser>();
            foreach (var line in users)
            {
                if (line.Code != IgsCode.User) continue; // Comment
                if (line.EntireLine.EndsWith("Language")) continue; // Example
                returnedUsers.Add(CreateUserFromTelnetLine(line.EntireLine));
            }
            return returnedUsers;
        }

        public async Task<bool> RequestBasicMatch(
            string opponent, 
            StoneColor yourColor, 
            int boardSize, 
            int mainTime,
            int byoyomiMinutes)
        {
            var lines = await
                MakeRequest("match " + opponent + " " + yourColor.ToIgsCharacterString() + " " + boardSize.ToString() +
                            " " + mainTime.ToString() + " " + byoyomiMinutes.ToString());
            // ReSharper disable once SimplifyLinqExpression ...that is not simplification, stupid ReSharper!
            return !lines.Any(line => line.Code == IgsCode.Error);
        }

        public async Task<bool> DeclineMatchRequest(IgsMatchRequest matchRequest)
        {
            List<IgsLine> lines = await MakeRequest(matchRequest.RejectCommand);
            // ReSharper disable once SimplifyLinqExpression ...that is not simplification, baka ReSharper!
            return !lines.Any(line => line.Code == IgsCode.Error);
        }
        public async Task<Game> AcceptMatchRequest(IgsMatchRequest matchRequest)
        { 
            /*  
            15 Game 10 I: Soothie (0 4500 -1) vs OmegaGo1 (0 4500 -1)
            9 Handicap and komi are disable.
            9 Creating match [10] with Soothie.
            9 Please use say to talk to your opponent -- help say.
            1 6
            */
            List<IgsLine> lines = await MakeRequest(matchRequest.AcceptCommand);
            if (lines.Any(line => line.Code == IgsCode.Error)) return null;
            GameHeading heading = IgsRegex.ParseGameHeading(lines[0]);

            Game game = new Core.Game()
            {
                BoardSize = new Core.GameBoardSize(19), // TODO
                Server = this,
                ServerId = heading.GameNumber,
                Ruleset = new Rules.JapaneseRuleset()
            };
            game.Players.Add(new Core.Player(heading.BlackName, "?", game));
            game.Players.Add(new Core.Player(heading.WhiteName, "?", game));
            this._gamesInProgressOnIgs.RemoveAll(gm => gm.ServerId == heading.GameNumber);
            this._gamesInProgressOnIgs.Add(game);
            return game;
        }
        public void DEBUG_MakeUnattendedRequest(string command)
        {
            MakeUnattendedRequest(command);
        }

        public void MakeMove(Game game, Move move)
        {
            MakeUnattendedRequest(move.Coordinates.ToIgsCoordinates() + " " + game.ServerId);
            // TODO many different things to handle here
        }
    }
}
