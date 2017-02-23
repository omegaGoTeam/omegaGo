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
        /// <summary>
        /// Randomizer
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// SFX effect player
        /// </summary>
        private static ISfxPlayerService _sfxPlayer;

        /// <summary>
        /// Sounds inside this audio file pack
        /// </summary>
        private readonly SfxId[] _sounds;

        /// <summary>
        /// Initializes a new <see cref="AudioFilePack"/> that, when played, plays one of the
        /// given sound effects, chosen at random. 
        /// </summary>
        /// <param name="sounds">The possible sound effects to play.</param>
        public AudioFilePack(params SfxId[] sounds)
        {
            _sounds = sounds;
        }

        /// <summary>
        /// Creates a new Audio file pack from a single sound
        /// </summary>
        /// <param name="sound"></param>
        public static implicit operator AudioFilePack(SfxId sound)
        {
            return new AudioFilePack(sound);
        }

        /// <summary>
        /// Selects a sound effect from this audio file pack at random, and causes it to start playing. 
        /// </summary>
        public async Task PlayAsync()
        {
            ResolveSfxPlayer();
            var selectedSound = _sounds[Random.Next(_sounds.Length)];
            await _sfxPlayer.PlaySoundEffectAsync(selectedSound);
        }

        /// <summary>
        /// Resolves SFX player service
        /// </summary>
        private void ResolveSfxPlayer()
        {
            if (_sfxPlayer == null)
            {
                _sfxPlayer = Mvx.Resolve<ISfxPlayerService>();
            }
        }
    }
}
