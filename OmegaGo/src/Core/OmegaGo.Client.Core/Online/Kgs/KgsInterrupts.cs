using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmegaGo.Core.Online.Kgs.Downstream;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// This is a router class that invokes various interrupt message handlers based on the 'type' field of
    /// messages from the KGS server.
    /// </summary>
    class KgsInterrupts
    {
        private KgsConnection kgsConnection;

        public KgsInterrupts(KgsConnection kgsConnection)
        {
            this.kgsConnection = kgsConnection;
        }

        public bool RouteAndHandle(string type, JObject message)
        {
            switch (type)
            {
                case "HELLO":
                    // Ignore, because we don't need this information.
                    return true;
                case "LOGIN_FAILED_BAD_PASSWORD":
                    HandleInterruptMessage<LoginFailedBadPassword>(message);
                    return true;
                case "LOGIN_FAILED_NO_SUCH_USER":
                    HandleInterruptMessage<LoginFailedNoSuchUser>(message);
                    return true;
                case "LOGIN_SUCCESS":
                    HandleInterruptMessage<LoginSuccess>(message);
                    return true;
                case "SUBSCRIPTION_UPDATE":
                    // Ignore, because we don't handle KGS+ account privileges.
                    return true;
                case "PLAYBACK_ADD":
                case "PLAYBACK_SETUP":
                case "CHANNEL_AUDIO":
                case "PLAYBACK_DATA":
                    // Ignore, because we don't handle audio channels.
                    return true;
                case "ROOM_NAMES":
                    HandleInterruptMessage<RoomNames>(message);
                    return true;
                case "GAME_UNDO_REQUEST":
                    HandleInterruptMessage<GameUndoRequest>(message);
                    return true;
                case "ROOM_DESC":
                    HandleInterruptMessage<RoomDesc>(message);
                    return true;
                case "ROOM_JOIN":
                    HandleInterruptMessage<KgsRoomJoin>(message);
                    return true;
                case "JOIN_COMPLETE":
                    HandleInterruptMessage<JoinComplete>(message);
                    return true;
                case "AUTOMATCH_PREFS":
                    HandleInterruptMessage<AutomatchPrefs>(message);
                    return true;
                case "USER_ADDED":
                    HandleInterruptMessage<UserAdded>(message);
                    return true;
                case "USER_REMOVED":
                    HandleInterruptMessage<UserRemoved>(message);
                    return true;
                case "USER_UPDATE":
                    HandleInterruptMessage<UserUpdate>(message);
                    return true;
                case "UNJOIN":
                    HandleInterruptMessage<Unjoin>(message);
                    return true;
                case "GAME_CONTAINER_REMOVE_GAME":
                    HandleInterruptMessage<GameContainerRemoveGame>(message);
                    return true;
                case "GLOBAL_GAMES_JOIN":
                    HandleInterruptMessage<GlobalGamesJoin>(message);
                    return true;
                case "GAME_LIST":
                    HandleInterruptMessage<GameList>(message);
                    return true;
                case "GAME_STATE":
                    HandleInterruptMessage<KgsGameState>(message);
                    return true;
                case "GAME_UPDATE":
                    HandleInterruptMessage<GameUpdate>(message);
                    return true;
                case "GAME_JOIN":
                    HandleInterruptMessage<KgsGameJoin>(message);
                    return true;
                case "LOGOUT":
                    HandleInterruptMessage<Logout>(message);
                    return true;
                case "CHALLENGE_JOIN":
                    HandleInterruptMessage<ChallengeJoin>(message);
                    return true;
                case "GAME_OVER":
                    HandleInterruptMessage<GameOver>(message);
                    return true;
                case "CHALLENGE_PROPOSAL":
                    HandleInterruptMessage<ChallengeProposal>(message);
                    return true;
                case "CHALLENGE_CREATED":
                    HandleInterruptMessage<ChallengeCreated>(message);
                    return true;
                case "CHALLENGE_SUBMIT":
                    HandleInterruptMessage<ChallengeSubmit>(message);
                    return true;
                case "CHALLENGE_FINAL":
                case "GAME_NOTIFY":
                    HandleInterruptMessage<ChallengeDownstreamEvent>(message);
                    return true;
                case "CLOSE":
                    HandleInterruptMessage<Close>(message);
                    return true;
                case "GAME_TIME_EXPIRED":
                    HandleInterruptMessage<GameTimeExpired>(message);
                    return true;
                case "CHALLENGE_DECLINE":
                    HandleInterruptMessage<ChallengeDecline>(message);
                    return true;
                case "CHANNEL_ALREADY_JOINED":
                case "PRIVATE_KEEP_OUT":
                case "CHALLENGE_NOT_CREATED":
                case "CHALLENGE_CANT_PLAY_RANKED":
                case "IDLE_WARNING":
                case "CHANNEL_SUBSCRIBERS_ONLY":
                case "CHANNEL_NO_TALKING":
                case "CANT_PLAY_TWICE":
                case "RECONNECT":
                    HandleInterruptMessage<NotificationMessageDownstreamEvent>(message);
                    return true;

            }
            // not handled
            return false;
        }

        private void HandleInterruptMessage<T>(JObject message)
            where T: KgsInterruptResponse
        {
            T response = message.ToObject<T>(kgsConnection.Serializer);
            response.Process(this.kgsConnection);
        }
    }
}
