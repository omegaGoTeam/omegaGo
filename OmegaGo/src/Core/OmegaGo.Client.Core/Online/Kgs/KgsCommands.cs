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
                    ChannelId = kgsInfo.ChannelId
                });
            }
            else
            {
                await kgsConnection.MakeUnattendedRequestAsync("GAME_MOVE", new
                {
                    ChannelId = kgsInfo.ChannelId,
                    Loc = new XY()
                    {
                        X = move.Coordinates.X,
                        Y = move.Coordinates.Y
                    }
                });
            }
        }

        public Task AddTime(RemoteGameInfo remoteInfo, TimeSpan additionalTime)
        {
            throw new NotImplementedException();
        }

        public Task UndoLifeDeath(RemoteGameInfo remoteInfo)
        {
            throw new NotImplementedException();
        }

        public Task LifeDeathDone(RemoteGameInfo remoteInfo)
        {
            throw new NotImplementedException();
        }

        public Task LifeDeathMarkDeath(Position position, RemoteGameInfo remoteInfo)
        {
            throw new NotImplementedException();
        }

        public Task Resign(RemoteGameInfo remoteInfo)
        {
            throw new NotImplementedException();
        }

        public async Task AcceptChallengeAsync(KgsChallenge selectedItem)
        {
            var originalProposal = selectedItem.Proposal;
            var ourName = kgsConnection.Username;
            var upstreamProposal = originalProposal.ToUpstream();
            var emptySeat = upstreamProposal.Players.First(pl => pl.Name == null);
            emptySeat.Name = ourName;
            await kgsConnection.MakeUnattendedRequestAsync("JOIN_REQUEST", new
            {
                ChannelId = selectedItem.ChannelId
            });
            await kgsConnection.WaitUntilJoined(selectedItem.ChannelId);
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
            await kgsConnection.MakeUnattendedRequestAsync("CHALLENGE_SUBMIT", simpleProposal);
            await kgsConnection.MakeUnattendedRequestAsync("CHALLENGE_ACCEPT", simpleProposal);
        }
    }
}
