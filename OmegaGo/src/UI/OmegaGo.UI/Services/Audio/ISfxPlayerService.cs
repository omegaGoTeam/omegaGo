using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Audio
{
    /// <summary>
    /// The sound effect player service plays audio files.
    /// </summary>
    public interface ISfxPlayerService
    {
        /// <summary>
        /// Begins the playback of the given sound effect, at the volume level determined by the master volume and sound effect volume settings. Call this from the UI thread only. Multiple
        /// sounds may play simultaneously. The fact that the task terminates does not mean that the
        /// sound has finished playing, merely that it has started playing.
        /// </summary>
        /// <param name="id">The sound effect to play.</param>
        /// <returns></returns>
        Task PlaySoundEffectAsync(SfxId id);
    }
}
