using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OmegaGo.Core;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;

namespace QuickPrototype
{
    public partial class PrimaryForm : Form
    {
        private IgsConnection igs = new IgsConnection();
        private List<Game> games;

        public PrimaryForm()
        {
            InitializeComponent();

        }

        private void Igs_LogEvent(string obj)
        {
           this.tbConsole.AppendText(Environment.NewLine + obj);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
                    games = await igs.ListGamesInProgress();
                    this.lbGames.Items.Clear();
                    this.lbGames.Items.AddRange(games.ToArray());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.igs.SendRawText(this.tbCommand.Text);
            this.tbCommand.Clear();
        }

        private void PrimaryForm_Load(object sender, EventArgs e)
        {
            igs = new IgsConnection();
            igs.LogEvent += Igs_LogEvent;
            igs.IncomingChatMessage += Igs_IncomingChatMessage;
            igs.Beep += Igs_Beep;
            igs.IncomingShoutMessage += Igs_IncomingShoutMessage;
            igs.Login("OmegaGo1", "123456789");
        }

        private void Igs_IncomingShoutMessage(string obj)
        {
            this.lbChat.Items.Add("SOMEBODY SHOUTS: " + obj);
        }

        private void Igs_Beep()
        {
            System.Media.SystemSounds.Beep.Play();
        }

        private void Igs_IncomingChatMessage(string obj)
        {
            this.lbChat.Items.Add("INCOMING: " + obj);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            igs.SendRawText("toggle client");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.lbGames.SelectedItem != null)
            {
                Game game = (Game)lbGames.SelectedItem;
                game.StartObserving();
                igs.RefreshBoard(game);
                InGameForm observing = new InGameForm(game, igs);
                observing.Show();
                this.lbObservedGames.Items.Add(game);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.lbObservedGames.SelectedItem != null)
            {

                Game game = (Game)lbGames.SelectedItem;
                game.StopObserving();
                this.lbObservedGames.Items.Remove(game);
            }
        }

        private async void bSendMessage_Click(object sender, EventArgs e)
        {
            bool success = await igs.Tell(this.cbMessageRecipient.Text, this.tbChatMessage.Text);
            if (success)
            {
                this.lbChat.Items.Add("OUTGOING to " + this.cbMessageRecipient.Text + ": " + this.tbChatMessage.Text);
            } else { 
                ReportError("An outgoing Tell message was not sent (user '" + this.cbMessageRecipient.Text + "' not online?).");
            }
        }

        private void ReportError(string error)
        {
            MessageBox.Show( error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void tbCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button4_Click(sender, EventArgs.Empty);
            }
        }

        private void bSortGames_Click(object sender, EventArgs e)
        {
            games.Sort((g1, g2) => g1.NumberOfObservers.CompareTo(g2.NumberOfObservers));
            games.Reverse();

            this.lbGames.Items.Clear();
            this.lbGames.Items.AddRange(games.ToArray());
        }
    }
}
