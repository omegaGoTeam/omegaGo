using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
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
                case "ROOM_DESC":
                    HandleInterruptMessage<RoomDesc>(message);
                    return true;
                case "ROOM_JOIN":
                    HandleInterruptMessage<RoomJoin>(message);
                    return true;
                case "JOIN_COMPLETE":
                    HandleInterruptMessage<JoinComplete>(message);
                    return true;
                case "AUTOMATCH_PREFS":
                    HandleInterruptMessage<AutomatchPrefs>(message);
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
