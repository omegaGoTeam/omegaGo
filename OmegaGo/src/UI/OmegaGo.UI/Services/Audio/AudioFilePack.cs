using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Helpers;

namespace OmegaGo.UI.Services.Audio
{
    /// <summary>
    /// Represents a group of sound effects that may be played in response to an event. Only one
    /// sound effect from the pack (chosen randomly) would be played. This helps the sound be
    /// more diverse.
    /// </summary>
    public class AudioFilePack
    {
        private static ISfxPlayerService player;

        private List<SfxId> _sounds = new List<SfxId>();


        /// <summary>
        /// Initializes a new <see cref="AudioFilePack"/> that, when played, plays one of the
        /// given sound effects, chosen at random. 
        /// </summary>
        /// <param name="sounds">The possible sound effects to play.</param>
        public AudioFilePack(params SfxId[] sounds)
        {
            this._sounds.AddRange(sounds);
        }

        /// <summary>
        /// Selects a sound effect from this audio file pack at random, and causes it to start playing. 
        /// </summary>
        public async Task PlayAsync()
        {
            if (player == null)
            {
                player = Mvx.Resolve<ISfxPlayerService>();
            }
            var selectedSound = this._sounds[Randomizer.Next(this._sounds.Count)];
            await player.PlaySoundEffectAsync(selectedSound);
        }

        public static implicit operator AudioFilePack(SfxId sound)
        {
            return new AudioFilePack(sound);
        }
    }
}
