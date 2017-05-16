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
    /// <para>
    /// While these methods are asynchronous, they return immediately after the command is successfully sent. They do not wait
    /// for a reply.
    /// </para>
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Common.ICommonCommands" />
    public class KgsCommands : ICommonCommands
    {
        private readonly KgsConnection kgsConnection;
        private Task CompletedTask = Task.FromResult(0);

        public KgsCommands(KgsConnection kgsConnection)
        {
            this.kgsConnection = kgsConnection;
        }

        public async Task UnobserveAsync(RemoteGameInfo remoteInfo)
        {
            var kgsInfo = remoteInfo as KgsGameInfo;
            await kgsConnection.MakeUnattendedRequestAsync("UNJOIN_REQUEST", new
            {
                ChannelId = kgsInfo.ChannelId
            });
        }

        /// <summary>
        /// Asks to join the room and requests its description.
        /// </summary>
        /// <param name="room">The room we want to join.</param>
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

        /// <summary>
        /// Asks to unjoin the room.
        /// </summary>
        /// <param name="room">The room we wish to exit.</param>
        public async Task UnjoinRoomAsync(KgsRoom room)
        {
            await kgsConnection.MakeUnattendedRequestAsync("UNJOIN_REQUEST", new
            {
                ChannelId = room.ChannelId
            });
        }

        /// <summary>
        /// Asks to join one of the three global lists: ACTIVES, CHALLENGES or FANS.
        /// </summary>
        /// <param name="listName">Name of the global list to join.</param>
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

        public async Task<bool> LogoutAsync()
        {
            return await kgsConnection.MakeUnattendedRequestAsync("LOGOUT", new object());
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
                        Y = KgsCoordinates.OurToTheirs(move.Coordinates.Y, kgsInfo.BoardSize)
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
                Y = KgsCoordinates.OurToTheirs(position.Y, kgsInfo.BoardSize)
            });
        }
        public async Task LifeDeathMarkLife(Position position, RemoteGameInfo remoteInfo)
        {
            KgsGameInfo kgsInfo = (KgsGameInfo)remoteInfo;
            await kgsConnection.MakeUnattendedRequestAsync("GAME_MARK_LIFE", new
            {
                ChannelId = kgsInfo.ChannelId,
                Alive = true,
                X = position.X,
                Y = KgsCoordinates.OurToTheirs(position.Y, kgsInfo.BoardSize)
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

        public async Task JoinAndSubmitSelfToChallengeAsync(KgsChallenge selectedItem)
        {
            // Join
            var simpleProposal = SubmitOurselvesIntoProposal(selectedItem);
            ChallengeWaiter waiter = new Kgs.KgsCommands.ChallengeWaiter(kgsConnection, selectedItem.ChannelId, simpleProposal);
            this.kgsConnection.Events.ChannelJoined += waiter.Joined;
            this.kgsConnection.Events.ChannelUnjoined += waiter.Unjoined;
            await kgsConnection.MakeUnattendedRequestAsync("JOIN_REQUEST", new
            {
                ChannelId = selectedItem.ChannelId
            });

        }
        
        class ChallengeWaiter
        {
            private readonly KgsConnection _connection;
            private readonly int _channelId;
            private readonly object _simpleProposal;

            public async void Joined(KgsChannel channel)
            {
                if (channel.ChannelId == _channelId)
                {
                    EndThis();
                    await _connection.MakeUnattendedRequestAsync("CHALLENGE_SUBMIT", _simpleProposal);
                }
            }
            public void Unjoined(KgsChannel channel)
            {
                if (channel.ChannelId == _channelId)
                {
                    EndThis();
                }
            }
            private void EndThis()
            {
                _connection.Events.ChannelJoined -= Joined;
                _connection.Events.ChannelUnjoined -= Unjoined;
            }
            public ChallengeWaiter(KgsConnection connection, int channelId, object simpleProposal)
            {
                _connection = connection;
                _channelId = channelId;
                _simpleProposal = simpleProposal;
            }
        }

        private object SubmitOurselvesIntoProposal(KgsChallenge selectedItem)
        {
            var originalProposal = selectedItem.Proposal;
            if (selectedItem.CreatorsNewProposal != null)
            {
                originalProposal = selectedItem.CreatorsNewProposal;
            }
            var ourName = this.kgsConnection.Username;
            var upstreamProposal = originalProposal.ToUpstream();
            var emptySeat = upstreamProposal.Players.FirstOrDefault(pl => pl.Name == null);
            if (emptySeat != null)
            {
                emptySeat.Name = ourName;
            }
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

        public async Task GenericUnjoinAsync(int channelId)
        {
            await kgsConnection.MakeUnattendedRequestAsync("UNJOIN_REQUEST", new
            {
                ChannelId = channelId
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
        public async Task CreateChallenge(KgsRoom room, bool ranked, bool global, RulesDescription rules, StoneColor yourColor)
        {
            await kgsConnection.MakeUnattendedRequestAsync("CHALLENGE_CREATE", new
            {
                ChannelId = room.ChannelId,
                CallbackKey = 0,
                Global = global,
                Text = "Game",
                Proposal = new 
                {
                    GameType = ranked ? GameType.Ranked : GameType.Free,
                    Rules = rules,
                    Nigiri = yourColor == StoneColor.None,
                    Players = new KgsPlayer []
                    {
                        new KgsPlayer()
                        {
                            Name = kgsConnection.Username,
                            Role = yourColor == StoneColor.Black ? "black" : "white"
                        },
                        new  KgsPlayer()
                        {
                            Role = yourColor == StoneColor.Black ? "white" : "black"
                        }
                    }
                }
            });
        }


        public async Task DeclineChallengeAsync(KgsChallenge challenge, Proposal incomingChallenge)
        {
            string targetName =
                incomingChallenge.Players.Select(pl => pl.User?.Name)
                    .First(name => name != null && name != kgsConnection.Username);
            await kgsConnection.MakeUnattendedRequestAsync("CHALLENGE_DECLINE", new
            {
                ChannelId = challenge.ChannelId,
                Name = targetName
            });
        }

        public async Task ChallengeProposalAsync(KgsChallenge challenge, Proposal incomingChallenge)
        {
            Proposal outgoing = incomingChallenge.ToUpstream();
            await kgsConnection.MakeUnattendedRequestAsync("CHALLENGE_PROPOSAL", new
            {
                ChannelId = challenge.ChannelId,
                GameType = outgoing.GameType,
                Rules = outgoing.Rules,
                Nigiri = outgoing.Nigiri,
                Players = outgoing.Players
            });
        }
    }
}
