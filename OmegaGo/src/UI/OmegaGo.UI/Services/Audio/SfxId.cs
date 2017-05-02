using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Audio
{
    /// <summary>
    /// Represents a single sound effect file. It is the responsibility of the front-end project
    /// to convert a value of this type into a sound file to play on the speakers.
    /// </summary>
    public enum SfxId
    {
        //place stone sounds
        SabakiPlace0,
        SabakiPlace1,
        SabakiPlace2,
        SabakiPlace3,
        SabakiPlace4,

        //pass sounds
        SabakiPass,

        //new game sounds
        SabakiNewGame,

        //capture sounds
        SabakiCapture0,
        SabakiCapture1,
        SabakiCapture2,
        SabakiCapture3,
        SabakiCapture4,


        // used for incoming match request
        // https://www.freesound.org/people/GabrielAraujo/sounds/242502/
        PleasantJingle
    }
}
