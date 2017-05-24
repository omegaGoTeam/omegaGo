using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    internal class ChallengeCreated : KgsInterruptResponse
    {
        public int CallbackKey { get; set; }
        public GameChannel Game { get; set; }
        public override void Process(KgsConnection connection)
        {
            KgsChallenge createdChallenge = KgsChallenge.FromChannel(Game);
            if (createdChallenge != null)
            {
                createdChallenge.OwnedByUs = true;
                connection.Data.JoinChallenge(createdChallenge);
            }
            createdChallenge.RaiseStatusChanged();
        }
    }
}