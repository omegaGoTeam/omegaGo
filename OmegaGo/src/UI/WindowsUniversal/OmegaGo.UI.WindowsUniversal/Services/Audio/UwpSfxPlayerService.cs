using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Windows.UI.Core;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Settings;
// ReSharper disable ClassNeverInstantiated.Global

namespace OmegaGo.UI.WindowsUniversal.Services.Audio
{
    /// <summary>
    /// Converts <see cref="SfxId"/> values to filenames and uses the <see cref="AudioGraph"/> API 
    /// to play sounds. Multiple sounds may be played at the same time.  
    /// </summary>
    /// <seealso cref="OmegaGo.UI.Services.Audio.ISfxPlayerService" />
    internal class UwpSfxPlayerService : ISfxPlayerService
    {
        private readonly IGameSettings _gameSettings;
        private readonly TaskCompletionSource<bool> _initializationTask = new TaskCompletionSource<bool>();

        /// <summary>
        /// Sound filenames dictionary
        /// </summary>
        private readonly Dictionary<SfxId, string> _filenames = new Dictionary<SfxId, string>
        {
            [SfxId.SabakiCapture0] = "Sounds\\Sabaki\\capture0.mp3",
            [SfxId.SabakiCapture1] = "Sounds\\Sabaki\\capture1.mp3",
            [SfxId.SabakiCapture2] = "Sounds\\Sabaki\\capture2.mp3",
            [SfxId.SabakiCapture3] = "Sounds\\Sabaki\\capture3.mp3",
            [SfxId.SabakiCapture4] = "Sounds\\Sabaki\\capture4.mp3",
            [SfxId.SabakiPlace0] = "Sounds\\Sabaki\\place0.mp3",
            [SfxId.SabakiPlace1] = "Sounds\\Sabaki\\place1.mp3",
            [SfxId.SabakiPlace2] = "Sounds\\Sabaki\\place2.mp3",
            [SfxId.SabakiPlace3] = "Sounds\\Sabaki\\place3.mp3",
            [SfxId.SabakiPlace4] = "Sounds\\Sabaki\\place4.mp3",
            [SfxId.SabakiNewGame] = "Sounds\\Sabaki\\newgame.mp3",
            [SfxId.SabakiPass] = "Sounds\\Sabaki\\pass.mp3",
            [SfxId.PleasantJingle] = "Sounds\\Notification.mp3"
        };

        private bool _initialized = false;
        private bool _initializing = false;

        private AudioGraph _audioGraph;
        private AudioDeviceOutputNode _outputNode;

        /// <summary>
        /// Creates UWP Sound effects player service
        /// </summary>
        /// <param name="gameSettings">Game settings</param>
        public UwpSfxPlayerService(IGameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        /// <summary>
        /// See the interface method for documentation on use. Implementation-wise, this method calculates
        /// the volume as the product of the master and sfx volume (both of which are 0 to 1). There is also
        /// a somewhat complicated mechanism to ensure that the audio engine is initialized. This is necessary because we may want to play multiple sounds one shortly after another.
        /// </summary>
        /// <param name="id">The sound effect to play.</param>
        /// <seealso cref="ISfxPlayerService.PlaySoundEffectAsync(SfxId)"/>
        public async Task PlaySoundEffectAsync(SfxId id)
        {
            await EnsureInitializedAsync();

            // Play sound
            if (!_gameSettings.Audio.Mute)
            {
                string file = _filenames[id];
                double gain = _gameSettings.Audio.MasterVolume*(_gameSettings.Audio.SfxVolume/10000.0);
                await PlaySound(file, gain);
            }
        }

        /// <summary>
        /// Ensures the service is initialized
        /// </summary>
        private async Task EnsureInitializedAsync()
        {
            if (!_initialized)
            {
                // This method is always run from the same thread, so there is no race condition here.
                if (_initializing)
                {
                    await _initializationTask.Task;
                }
                else
                {
                    _initializing = true;
                    await InitializeAsync();
                    _initializationTask.SetResult(true);
                    _initialized = true;
                }
            }
        }
        
        /// <summary>
        /// Initializes the player service
        /// </summary>
        private async Task InitializeAsync()
        {
            var result = await AudioGraph.CreateAsync(new AudioGraphSettings(AudioRenderCategory.Media));
            if (result.Status != AudioGraphCreationStatus.Success) return;
            _audioGraph = result.Graph;
            var outputResult = await _audioGraph.CreateDeviceOutputNodeAsync();
            if (outputResult.Status != AudioDeviceNodeCreationStatus.Success) return;
            _outputNode = outputResult.DeviceOutputNode;
            _audioGraph.Start();
        }
        
        /// <summary>
        /// Plays a sound file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="gain">Gain</param>
        /// <returns></returns>
        private async Task PlaySound(string file, double gain)
        {
            var soundFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{file}"));
            var fileInputNodeResult = await _audioGraph.CreateFileInputNodeAsync(soundFile);
            if (fileInputNodeResult.Status != AudioFileNodeCreationStatus.Success) return;
            var fileInputNode = fileInputNodeResult.FileInputNode;
            fileInputNode.FileCompleted += FileInputNodeOnFileCompleted;
            fileInputNode.AddOutgoingConnection(_outputNode, gain);
        }

        /// <summary>
        /// Closes and disposes sound file node
        /// </summary>
        /// <param name="sender">Audio file input node</param>
        /// <param name="args">Arguments</param>
        private async void FileInputNodeOnFileCompleted(AudioFileInputNode sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                sender.RemoveOutgoingConnection(_outputNode);
                sender.FileCompleted -= FileInputNodeOnFileCompleted;
                sender.Dispose();
            });
        }
    }
}
