using OmegaGo.Core;
using OmegaGo.Core.Online.Igs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OmegaGo.DesktopPrototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Game> _lbGamesItems;
        private ObservableCollection<string> _lbChatItems;
        private ObservableCollection<Game> _lbObservedGamesItems;
        private ObservableCollection<IgsUser> _lbUsersItems;

        public ObservableCollection<Game> lbGamesItems
        {
            get { return _lbGamesItems; }
        }

        public ObservableCollection<string> lbChatItems
        {
            get { return _lbChatItems; }
        }

        public ObservableCollection<Game> lbObservedGamesItems
        {
            get { return _lbObservedGamesItems; }
        }

        public ObservableCollection<IgsUser> lbUsersItems
        {
            get { return _lbUsersItems; }
        }

        public MainWindow()
        {
            _lbGamesItems = new ObservableCollection<Game>();
            _lbChatItems = new ObservableCollection<string>();
            _lbObservedGamesItems = new ObservableCollection<Game>();
            _lbUsersItems = new ObservableCollection<IgsUser>();

            InitializeComponent();
        }


        // Taken from Forms prototype
        private IgsConnection _igsConnection = new IgsConnection();
        private List<Game> _games;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _igsConnection = new IgsConnection();
            _igsConnection.LogEvent += Igs_LogEvent;
            _igsConnection.IncomingChatMessage += Igs_IncomingChatMessage;
            _igsConnection.Beep += Igs_Beep;
            _igsConnection.IncomingShoutMessage += Igs_IncomingShoutMessage;
            _igsConnection.Login("OmegaGo1", "123456789");
        }

        private void Igs_LogEvent(string obj)
        {
            this.tbConsole.AppendText(Environment.NewLine + obj);
        }

        private void Igs_IncomingShoutMessage(string obj)
        {
            this.lbChat.Items.Add("SOMEBODY SHOUTS: " + obj);
        }

        private void Igs_IncomingChatMessage(string obj)
        {
            this.lbChat.Items.Add("INCOMING: " + obj);
        }

        private void Igs_Beep()
        {
            System.Media.SystemSounds.Beep.Play();
        }

        private void ReportError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _games = await _igsConnection.ListGamesInProgress();
            lbGamesItems.Clear();

            _games.ForEach(x => lbGamesItems.Add(x));
        }

        private async void bSortGames_Click(object sender, RoutedEventArgs e)
        {
            List<IgsUser> users = await _igsConnection.ListOnlinePlayers();
            lbUsersItems.Clear();
            users.ForEach(x => lbUsersItems.Add(x));

            this.cbMatchRecipient.Items.Clear();
            users.ForEach(usr => this.cbMatchRecipient.Items.Add(usr.Name));
            this.cbMessageRecipient.Items.Clear();
            users.ForEach(usr => this.cbMessageRecipient.Items.Add(usr.Name));
        }

        private void Observe_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbGames.SelectedItem != null)
            {
                Game game = (Game)lbGames.SelectedItem;
                game.StartObserving();
                _igsConnection.RefreshBoard(game);
                InGameWindow observing = new InGameWindow(game, _igsConnection);
                observing.Show();
                this.lbObservedGamesItems.Add(game);
            }
        }

        private void StopObserving_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbObservedGames.SelectedItem != null)
            {

                Game game = (Game)lbGames.SelectedItem;
                game.StopObserving();
                this.lbObservedGames.Items.Remove(game);
            }
        }

        private void SendCommand_Click(object sender, RoutedEventArgs e)
        {
            _igsConnection.SendRawText(this.tbCommand.Text);
            this.tbCommand.Clear();
        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            bool success = await _igsConnection.Tell(this.cbMessageRecipient.Text, this.tbChatMessage.Text);
            if (success)
            {
                this.lbChat.Items.Add("OUTGOING to " + this.cbMessageRecipient.Text + ": " + this.tbChatMessage.Text);
            }
            else
            {
                ReportError("An outgoing Tell message was not sent (user '" + this.cbMessageRecipient.Text + "' not online?).");
            }
        }

        private async void RefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            List<IgsUser> users = await _igsConnection.ListOnlinePlayers();
            lbUsersItems.Clear();
            users.ForEach(x => lbUsersItems.Add(x));

            this.cbMatchRecipient.Items.Clear();
            users.ForEach(usr => this.cbMatchRecipient.Items.Add(usr.Name));
            this.cbMessageRecipient.Items.Clear();
            users.ForEach(usr => this.cbMessageRecipient.Items.Add(usr.Name));
        }
    }
}
