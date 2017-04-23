using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// Call methods of this class to send commands to KGS.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Common.ICommonCommands" />
    public class KgsCommands : ICommonCommands
    {
        private readonly KgsConnection kgsConnection;

        public KgsCommands(KgsConnection kgsConnection)
        {
            this.kgsConnection = kgsConnection;
        }

        public async Task JoinRoomAsync(KgsRoom room)
        {
            await kgsConnection.MakeUnattendedRequestAsync("JOIN_REQUEST", new
            {
                ChannelId = room.ChannelId
            });
            await kgsConnection.MakeUnattendedRequestAsync("ROOM_DESC_REQUEST", new
            {
                ChannelId = room.ChannelId
            });
        }

        public async Task UnjoinRoomAsync(KgsRoom room)
        {
            await kgsConnection.MakeUnattendedRequestAsync("UNJOIN_REQUEST", new
            {
                ChannelId = room.ChannelId
            });
        }

        public async Task GlobalListJoinRequestAsync(string listName)
        {
            await kgsConnection.MakeUnattendedRequestAsync("GLOBAL_LIST_JOIN_REQUEST", new
            {
                List = listName
            });
        }

        public async Task ObserveGameAsync(KgsGameInfo gameInfo)
        {
            await kgsConnection.MakeUnattendedRequestAsync("JOIN_REQUEST", new
            {
                ChannelId = gameInfo.ChannelId
            });
        }

        public async Task WakeUpAsync()
        {
            if (kgsConnection.LoggedIn)
            {
                await kgsConnection.MakeUnattendedRequestAsync("WAKE_UP", new object());
            }
        }

        public async Task LogoutAsync()
        {
            await kgsConnection.MakeUnattendedRequestAsync("LOGOUT", new object());
        }

        public async Task MakeMove(RemoteGameInfo remoteInfo, Move move)
        {
            KgsGameInfo kgsInfo = (KgsGameInfo) remoteInfo;
            if (move.Kind == MoveKind.Pass)
            {
                await kgsConnection.MakeUnattendedRequestAsync("GAME_MOVE", new
                {
                    ChannelId = kgsInfo.ChannelId,
                    Loc = "PASS"
                });
            }
            else
            {
                await kgsConnection.MakeUnattendedRequestAsync("GAME_MOVE", new
                {
                    ChannelId = kgsInfo.ChannelId,
                    Loc = new 
                    {
                        X = move.Coordinates.X,
                        Y = move.Coordinates.Y
                    }
                });
            }
        }

        public async Task AddTime(RemoteGameInfo remoteInfo, TimeSpan additionalTime)
        {
            KgsGameInfo kgsInfo = (KgsGameInfo)remoteInfo;
            string opponentsRole = "black";
            if (kgsInfo.Black.Name == kgsConnection.Username)
            {
                opponentsRole = "white";
            }
            await kgsConnection.MakeUnattendedRequestAsync("GAME_ADD_TIME", new
            {
                ChannelId = kgsInfo.ChannelId,
                Role = opponentsRole,
                Time = (float) additionalTime.TotalSeconds
            });
        }

        public Task UndoLifeDeath(RemoteGameInfo remoteInfo)
        {
            // Life/death works a little differently in KGS.
            return CompletedTask;
        }

        public async Task LifeDeathDone(RemoteGameInfo remoteInfo)
        {
            KgsGameInfo kgsInfo = (KgsGameInfo)remoteInfo;
            var game = kgsConnection.Data.GetGame(kgsInfo.ChannelId);
            await kgsConnection.MakeUnattendedRequestAsync("GAME_SCORING_DONE", new
            {
                ChannelId = kgsInfo.ChannelId,
                DoneId = game.Controller.DoneId
            });
        }

        public async Task LifeDeathMarkDeath(Position position, RemoteGameInfo remoteInfo)
        {
            KgsGameInfo kgsInfo = (KgsGameInfo)remoteInfo;
            await kgsConnection.MakeUnattendedRequestAsync("GAME_MARK_LIFE", new
            {
                ChannelId = kgsInfo.ChannelId,
                Alive = false,
                X = position.X,
                Y = position.Y
            });
        }

        public async Task AllowUndoAsync(RemoteGameInfo remoteInfo)
        {
            KgsGameInfo kgsInfo = (KgsGameInfo)remoteInfo;
            await kgsConnection.MakeUnattendedRequestAsync("GAME_UNDO_ACCEPT", new
            {
                ChannelId = kgsInfo.ChannelId
            });
        }

        public Task RejectUndoAsync(RemoteGameInfo remoteInfo)
        {
            // At KGS, to reject an undo, simply ignore it. 
            return CompletedTask;
        }

        public async Task Resign(RemoteGameInfo remoteInfo)
        {
            var kgsInfo = (KgsGameInfo) remoteInfo;
            await kgsConnection.MakeUnattendedRequestAsync("GAME_RESIGN", new
            {
                ChannelId = kgsInfo.ChannelId,
            });
        }

        public async Task<KgsChallenge> JoinAndSubmitSelfToChallengeAsync(KgsChallenge selectedItem)
        {
            // Join
            await kgsConnection.MakeUnattendedRequestAsync("JOIN_REQUEST", new
            {
                ChannelId = selectedItem.ChannelId
            });
            await kgsConnection.WaitUntilJoined(selectedItem.ChannelId);


            var simpleProposal = SubmitOurselvesIntoProposal(selectedItem);

            await kgsConnection.MakeUnattendedRequestAsync("CHALLENGE_SUBMIT", simpleProposal);
            return selectedItem;
        }

        private object SubmitOurselvesIntoProposal(KgsChallenge selectedItem)
        {
            var originalProposal = selectedItem.Proposal;
            var ourName = this.kgsConnection.Username;
            var upstreamProposal = originalProposal.ToUpstream();
            var emptySeat = upstreamProposal.Players.First(pl => pl.Name == null);
            emptySeat.Name = ourName;
            var simpleProposal = new
            {
                ChannelId = selectedItem.ChannelId,
                GameType = upstreamProposal.GameType,
                Rules = upstreamProposal.Rules,
                Nigiri = upstreamProposal.Nigiri,
                Players = new[]
                {
                    new
                    {
                        Role = upstreamProposal.Players[0].Role,
                        Name = upstreamProposal.Players[0].Name
                    },
                    new
                    {
                        Role = upstreamProposal.Players[1].Role,
                        Name = upstreamProposal.Players[1].Name
                    }
                }
            };
            return simpleProposal;
        }

        public async Task GenericUnjoinAsync(KgsChannel channel)
        {
            await kgsConnection.MakeUnattendedRequestAsync("UNJOIN_REQUEST", new
            {
                ChannelId = channel.ChannelId
            });
        }

        public async Task AcceptChallenge(KgsChallenge challenge)
        {
           var newProposal = SubmitOurselvesIntoProposal(challenge);
           await kgsConnection.MakeUnattendedRequestAsync("CHALLENGE_ACCEPT", newProposal);
        }

        public async Task GameTimeExpiredAsync(int channelId)
        {
            await kgsConnection.MakeUnattendedRequestAsync("GAME_TIME_EXPIRED", new
            {
                ChannelId = channelId
            });
        }

        public async Task ChatAsync(KgsGameInfo info, string text)
        {
            await kgsConnection.MakeUnattendedRequestAsync("CHAT", new
            {
                ChannelId = info.ChannelId,
                Text = text
            });
        }
        public async Task UndoPleaseAsync(KgsGameInfo info)
        {
            await kgsConnection.MakeUnattendedRequestAsync("GAME_UNDO_REQUEST", new
            {
                ChannelId = info.ChannelId
            });
        }

        private Task CompletedTask = Task.FromResult(0);

    }
}
