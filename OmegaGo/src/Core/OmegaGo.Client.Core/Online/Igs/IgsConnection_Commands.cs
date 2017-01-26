using OmegaGo.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Igs
{
    partial class IgsConnection
    {
        /*
        public async Task<ObsoleteGameInfo> GetGameByIdAsync(int gameId)
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
        */
        public async Task<List<OnlineGameInfo>> ListGamesInProgressAsync()
        {
            await EnsureConnectedAsync();
            this._gamesInProgressOnIgs = new List<OnlineGameInfo>();
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
                    this._gamesInProgressOnIgs.Add(CreateGameFromTelnetLine(line.EntireLine));
                }
            }
            return this._gamesInProgressOnIgs;
        }
        private OnlineGameInfo CreateGameFromTelnetLine(string line)
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
            OnlineGameInfo game = new OnlineGameInfo(
                new PlayerInfo(StoneColor.White, match.Groups[2].Value, match.Groups[3].Value),
                new PlayerInfo(StoneColor.Black, match.Groups[4].Value, match.Groups[5].Value),
                new GameBoardSize(match.Groups[7].Value.AsInteger()),
                RulesetType.Japanese,
                match.Groups[8].Value.AsInteger(),
                HandicapPlacementType.Fixed,
                match.Groups[9].Value.AsFloat(),
                CountingType.Territory,
                match.Groups[12].Value.AsInteger(),
                ServerID.Igs);

                // DO *NOT* DO this: the displayed number might be something different from what our client wants
                // NumberOfMovesPlayed = match.Groups[6].Value.AsInteger(),
                // Do not uncomment the preceding line. I will fix it in time. I hope.

                return game;

        }
        /*
        public async void StartObserving(ObsoleteGameInfo game)
        {
            if (this._gamesBeingObserved.Contains(game))
            {
                // We are already observing this game.
                return;
            }
            this._gamesBeingObserved.Add(game);
            this._gamesYouHaveOpened.Add(game);
            await MakeRequestAsync("observe " + game.ServerId);
        }
        public void EndObserving(ObsoleteGameInfo game)
        {
            if (!this._gamesBeingObserved.Contains(game))
            {
                throw new ArgumentException("The specified game is currently not being observed.", nameof(game));
            }
            this._gamesBeingObserved.Remove(game);
            this._gamesYouHaveOpened.Remove(game);
            this._streamWriter.WriteLine("observe " + game.ServerId);
        }
        */
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
        public async Task<List<IgsUser>> ListOnlinePlayersAsync()
        {
            await EnsureConnectedAsync();
            List<IgsLine> users = await MakeRequestAsync("user");
            var returnedUsers = new List<IgsUser>();
            foreach (var line in users)
            {
                if (line.Code != IgsCode.User) continue; // Comment
                if (line.EntireLine.EndsWith("Language")) continue; // Example
                returnedUsers.Add(CreateUserFromTelnetLine(line.EntireLine));
            }
            return returnedUsers;
        }

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
            // ReSharper disable once SimplifyLinqExpression ...that is not simplification, stupid ReSharper!
            return !lines.Any(line => line.Code == IgsCode.Error);
        }

        public async Task<bool> DeclineMatchRequestAsync(IgsMatchRequest matchRequest)
        {
            List<IgsLine> lines = await MakeRequestAsync(matchRequest.RejectCommand);
            // ReSharper disable once SimplifyLinqExpression ...that is not simplification, baka ReSharper!
            return !lines.Any(line => line.Code == IgsCode.Error);
        }
        public async Task<OnlineGameInfo> AcceptMatchRequestAsync(IgsMatchRequest matchRequest)
        {
            /*
            List<IgsLine> lines = await MakeRequestAsync(matchRequest.AcceptCommand);
            if (lines.Any(line => line.Code == IgsCode.Error)) return null;
            GameHeading heading = IgsRegex.ParseGameHeading(lines[0]);

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
            return game;
            */
            return null;
        }
        public void DEBUG_MakeUnattendedRequest(string command)
        {
            MakeUnattendedRequest(command);
        }

        /*
        public override void MakeMove(ObsoleteGameInfo game, Move move)
        {
            switch (move.Kind)
            {
                case MoveKind.PlaceStone:
                    MakeUnattendedRequest(move.Coordinates.ToIgsCoordinates() + " " + game.ServerId);
                    break;
                case MoveKind.Pass:
                    MakeUnattendedRequest("pass " + game.ServerId);
                    break;
            }
        }
        public override void Resign(ObsoleteGameInfo game)
        {
            MakeUnattendedRequest("resign " + game.ServerId);
        }
        */
        /*
        public async Task<bool> SayAsync(ObsoleteGameInfo game, string chat)
        {
            if (!this._gamesYouHaveOpened.Contains(game)) throw new ArgumentException("You don't have this game opened on IGS.");
            if (chat == null) throw new ArgumentNullException(nameof(chat));
            if (chat == "") throw new ArgumentException("Chat line must not be empty.");
            if (chat.Contains("\n")) throw new Exception("Chat lines on IGS must not contain line breaks.");
            IgsResponse response;
            if (this._gamesYouHaveOpened.Count > 1)
            {
                // More than one game is opened: we must give the game id.
                response = await MakeRequestAsync("say " + game.ServerId + " " + chat);
            }
            else
            {
                // We have only one game opened: game id MUST NOT be given
                response = await MakeRequestAsync("say " + chat);
            }
            return !response.IsError;
        }*/

            /*
        public async Task UndoPleaseAsync(ObsoleteGameInfo game)
        {
            await MakeRequestAsync("undoplease " + game.ServerId);
        }

        public async Task UndoAsync(ObsoleteGameInfo game)
        {
            await MakeRequestAsync("undo " + game.ServerId);
        }

        public void NoUndo(ObsoleteGameInfo game)
        {
            MakeUnattendedRequest("noundo " + game.ServerId);
        }

        public override async void LifeDeath_Done(ObsoleteGameInfo game)
        {
            await MakeRequestAsync("done " + game.ServerId);
        }
        public override async void LifeDeath_MarkDead(Position position, ObsoleteGameInfo game)
        {
            await MakeRequestAsync(position.ToIgsCoordinates() + " " + game.ServerId);
        }
        */
    }
}
