using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// A downstream message by the metatranslator which has some meaning for a challenge that we have opened. These events may alter
    /// the status of UI controls on the challenge form, but so far, they are merely displayed in a listbox.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Downstream.Abstract.KgsInterruptChannelMessage" />
    class ChallengeDownstreamEvent : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
            var channel = connection.Data.GetChannel(this.ChannelId);
            KgsChallenge challenge = channel as KgsChallenge;
            if (challenge != null)
            {
                challenge.Events.Add(this.Type);
            }
            else
            {
                Debug.WriteLine("That was supposed to be a challenge.");
            }
        }
    }
}
