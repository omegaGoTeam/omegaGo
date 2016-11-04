using OmegaGo.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                    Players = new List<Player>()
                {
                    new Player(match.Groups[4].Value, match.Groups[5].Value),
                    new Player(match.Groups[2].Value, match.Groups[3].Value)
                },
                    NumberOfMovesPlayed = match.Groups[6].Value.AsInteger(),
                    SquareBoardSize = match.Groups[7].Value.AsInteger(),
                    NumberOfHandicapStones = match.Groups[8].Value.AsInteger(),
                    KomiValue = match.Groups[9].Value.AsFloat(),
                    NumberOfObservers = match.Groups[12].Value.AsInteger()
                };
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
    }
}
