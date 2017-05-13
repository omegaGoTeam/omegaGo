using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OmegaGo.Core.Annotations;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents any KGS channel. This can either be a <see cref="KgsGameChannel"/> (i.e. a challenge or a game) or
    /// a <see cref="KgsGameContainer"/> (i.e. a room or a global list). Instances of this class are stored in Data,
    /// and are not sent directly via downstream or upstream messages. 
    /// </summary>
    public class KgsChannel : INotifyPropertyChanged
    {
        private bool _joined;

        /// <summary>
        /// Gets or sets the number KGS assigned to this channel. Unlike with IGS, channel ID numbers are unique and don't repeat.
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the logged-in user is in this channel (whether this channel is joined).
        /// </summary>
        public bool Joined
        {
            get { return _joined; }
            set
            {
                _joined = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the users that are present in this channel. I don't think we're using this anywhere currently.
        /// </summary>
        public ObservableCollection<KgsUser> Users { get; } = new ObservableCollection<KgsUser>();

        public override string ToString()
        {
            return "Channel " + ChannelId;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}