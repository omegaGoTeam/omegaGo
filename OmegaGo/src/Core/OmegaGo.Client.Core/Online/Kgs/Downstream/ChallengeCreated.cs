using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs
{
    internal class ChallengeCreated : KgsInterruptResponse
    {
        public int CallbackKey { get; set; }
        public GameChannel Game { get; set; }
        public override void Process(KgsConnection connection)
        {
            // TODO Petr KGS OVERHAUL
            connection.Data.JoinChallenge(Game.ChannelId);
            KgsChallenge createdChallenge = new KgsChallenge(Game.InitialProposal, Game.ChannelId);
            connection.Data.OpenChallenges.Add(createdChallenge);
            createdChallenge.OwnedByUs = true;
            connection.Events.RaiseChallengeJoined(createdChallenge);
            createdChallenge.RaiseStatusChanged();
        }
    }
}