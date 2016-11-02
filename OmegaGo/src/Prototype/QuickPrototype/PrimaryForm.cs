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
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.Defeatist;
using OmegaGo.Core.AI.Random;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;
// ReSharper disable CoVariantArrayConversion

namespace QuickPrototype
{
    public partial class PrimaryForm : Form
    {
        private IgsConnection igs;
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
            this.igs.DEBUG_SendRawText(this.tbCommand.Text);
            this.tbCommand.Clear();
        }

        private async void PrimaryForm_Load(object sender, EventArgs e)
        {
            this.cbBlack.Items.Clear();
            this.cbBlack.Items.Add("Human");
            this.cbBlack.Items.AddRange(AISystems.AiPrograms.ToArray());
            this.cbWhite.Items.Clear();
            this.cbWhite.Items.Add("Human");
            this.cbWhite.Items.AddRange(AISystems.AiPrograms.ToArray());

            igs = new IgsConnection();
            igs.LogEvent += Igs_LogEvent;
            igs.IncomingChatMessage += Igs_IncomingChatMessage;
            igs.Beep += Igs_Beep;
            igs.IncomingShoutMessage += Igs_IncomingShoutMessage;
            this.cbWhite.SelectedIndex = 0;
            this.cbBlack.SelectedIndex = 0;
            if (!await igs.Connect())
            {
                MessageBox.Show("Connection to IGS failed.");
                return;
            }
            if (!await igs.Login("OmegaGo1", "123456789"))
            {
                MessageBox.Show("Login failed.");
            }
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
            igs.DEBUG_SendRawText("toggle client");
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

        private async void button7_Click(object sender, EventArgs e)
        {
            List<IgsUser> users = await igs.ListOnlinePlayers();
            this.lbUsers.Items.Clear();
            this.lbUsers.Items.AddRange(users.ToArray());

            this.cbMatchRecipient.Items.Clear();
            this.cbMatchRecipient.Items.AddRange(users.Select(usr => usr.Name).ToArray());
            this.cbMessageRecipient.Items.Clear();
            this.cbMessageRecipient.Items.AddRange(users.Select(usr => usr.Name).ToArray());
        }

        private void bPlayLocal_Click(object sender, EventArgs e)
        {
            Player playerBlack = new Player(this.cbBlack.Text + " (Black)", "NR");
            Player playerWhite = new Player(this.cbWhite.Text + " (White)", "NR");
            

            Game localGame = new Game
            {
                SquareBoardSize = (int) this.nBoardSize.Value,
                NumberOfMovesPlayed = 0
            };
            localGame.Ruleset = new ChineseRuleset();
            localGame.Ruleset.startGame(playerWhite, playerBlack, localGame.BoardSize);
            localGame.Players.Add(playerBlack);
            localGame.Players.Add(playerWhite);
            localGame.Server = null;
            InGameForm ingameForm = new InGameForm(localGame, null);
            playerBlack.Agent = CreateAgentFromComboboxObject(ingameForm, this.cbBlack.SelectedItem);
            playerWhite.Agent = CreateAgentFromComboboxObject(ingameForm, this.cbWhite.SelectedItem);
            ingameForm.ShowDialog();
        }

        private IAgent CreateAgentFromComboboxObject(InGameForm form, object text)
        {
            if (text is string && ((string)text) == "Human")
            {
                return new InGameFormGuiAgent(form);
            }
            if (text is IAIProgram)
            {
                IAIProgram newInstance = (IAIProgram)Activator.CreateInstance(text.GetType());
                return new AIAgent(newInstance);
            }
            throw new Exception("This agent cannot be handled yet.");
        }

        private async void button6_Click_1(object sender, EventArgs e)
        {
            Igs_LogEvent("CONNECT() RESULT: " + await this.igs.Connect());
            Igs_LogEvent("LOGIN() RESULT: " + await this.igs.Login("OmegaGo1", "123456789"));
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            await this.igs.Disconnect();
            Igs_LogEvent("DISCONNECTED.");
        }
    }
}
