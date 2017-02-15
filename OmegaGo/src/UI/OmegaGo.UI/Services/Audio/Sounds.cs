using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Audio
{
    /// <summary>
    /// Contains sound effects that can be played from either the viewmodels or the UI. 
    /// 
    /// To play a sound effect, use
    /// <code>
    /// Sounds.PlaceStone.Play()
    /// </code>.
    /// 
    /// The sound will be chosen randomly from the pack and will play at the appropriate volume level.
    /// </summary>
    public static class Sounds
    {
        public static AudioFilePack PlaceStone = new AudioFilePack(SfxId.SabakiPlace0, SfxId.SabakiPlace1,
            SfxId.SabakiPlace2, SfxId.SabakiPlace3, SfxId.SabakiPlace4);

        public static AudioFilePack Capture = new AudioFilePack(SfxId.SabakiCapture0, SfxId.SabakiCapture1,
            SfxId.SabakiCapture2, SfxId.SabakiCapture3, SfxId.SabakiCapture4);

        public static AudioFilePack NewGame = SfxId.SabakiNewGame;
        public static AudioFilePack Pass = SfxId.SabakiPass;
        public static AudioFilePack TestSfx = SfxId.SabakiPlace1;
    }
}
