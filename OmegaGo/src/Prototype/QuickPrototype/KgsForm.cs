using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Structures;

namespace FormsPrototype
{
    public partial class KgsForm : Form
    {
        private KgsConnection kgs = new KgsConnection();
        public KgsForm()
        {
            InitializeComponent();
            kgs.Events.PersonalInformationUpdate += Events_PersonalInformationUpdate;
            kgs.Events.UnhandledMessage += Kgs_IncomingMessage;
            kgs.Events.IncomingMessage += Events_IncomingMessage;
            kgs.Events.SystemMessage += Events_SystemMessage;
            kgs.Events.OutgoingRequest += Events_OutgoingRequest;
            kgs.Events.GameJoined += Events_GameJoined;
            kgs.Events.Disconnection += Events_Disconnection;
        }

        private void Events_PersonalInformationUpdate(object sender, OmegaGo.Core.Online.Kgs.Datatypes.User e)
        {
            this.lblYourRank.Text = "Your rank: " + e.Rank;
        }

        private void Events_Disconnection(object sender, string e)
        {
            MessageBox.Show(
                "You have been disconnected from KGS!" +
                (String.IsNullOrWhiteSpace(e) ? "\n\nNo reason given." : "\n\nReason: " + e), "Disconnection",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Events_GameJoined(object sender, KgsGame e)
        {
            InGameForm ingameForm = new FormsPrototype.InGameForm(e.Info, e.Controller, this.kgs);
            ingameForm.LoadGame(e);
            ingameForm.Show();

        }

        private void Events_IncomingMessage(object sender, JsonResponse e)
        {
            this.lbAllIncomingMessages.Items.Add(e);
        }

        private void Events_OutgoingRequest(object sender, string e)
        {
            if (e.Contains("WAKE_UP"))
            {
                return;
            }
            this.tbLastOutgoingMessage.Text = e;
        }

        private void Events_SystemMessage(object sender, string e)
        {
            this.tbLog.AppendText(e + Environment.NewLine);
        }

        private void Kgs_IncomingMessage(object sender, JsonResponse e)
        {
            if (this.tbFirstUnhandledMessage.Text == "")
            {
                this.tbFirstUnhandledMessage.Text = e.Fulltext;
            }
            this.tbUnhandledMessagesFull.Text += e.Fulltext + Environment.NewLine;
            this.tbUnhandledMessageTypes.Text += (e.Type + Environment.NewLine);
        }

        private async void KgsForm_Load(object sender, EventArgs e)
        {
            this.tbLog.Text += ("Logging in." + Environment.NewLine);
            if (await kgs.LoginAsync("OmegaGo1", "123456789"))
            {
                this.tbLog.Text += ("Logged in." + Environment.NewLine);
                
            }
            else
            {
                MessageBox.Show("Fail.");
            }
        }

       

        private void bLocalRoomsRefresh_Click(object sender, EventArgs e)
        {
            this.lbRooms.Items.Clear();
            var values = kgs.Data.Rooms.Values.ToList();
            values.Sort((r1, r2) =>
            {
                if (r1.Joined && !r2.Joined) return -1;
                else if (!r1.Joined && r2.Joined) return 1;
                else
                {
                    return String.Compare(r1.Name, r2.Name, StringComparison.Ordinal);
                }
            });
            foreach (KgsRoom room in values)
            {
                this.lbRooms.Items.Add(room);
            }
        }

        private void lbRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lbRooms.SelectedItem != null)
            {
                var room = ((KgsRoom) this.lbRooms.SelectedItem);
                this.tbRoomDescription.Text = room.Description;
            }
        }

        private async void bJoinRoom_Click(object sender, EventArgs e)
        {
            if (this.lbRooms.SelectedItem != null)
            {
                var room = ((KgsRoom)this.lbRooms.SelectedItem);
                await kgs.Commands.JoinRoomAsync(room);
            }
        }

        private async void bUnjoinRoom_Click(object sender, EventArgs e)
        {
            if (this.lbRooms.SelectedItem != null)
            {
                var room = ((KgsRoom)this.lbRooms.SelectedItem);
                await kgs.Commands.UnjoinRoomAsync(room);
            }
        }

        private void bRefreshLocalContainers_Click(object sender, EventArgs e)
        {
            this.lbContainers.Items.Clear();
            var values = kgs.Data.Containers.Values.ToList();
            values.Sort((r1, r2) =>
            {
                if (r1.Joined && !r2.Joined) return -1;
                else if (!r1.Joined && r2.Joined) return 1;
                else
                {
                    return String.Compare(r1.Name, r2.Name, StringComparison.Ordinal);
                }
            });
            foreach (KgsGameContainer room in values)
            {
                this.lbContainers.Items.Add(room);
            }
        }

        private void lbContainers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lbContainers.SelectedItem != null)
            {
                var container = ((KgsGameContainer)this.lbContainers.SelectedItem);
                this.lbContainerGames.Items.Clear();
                foreach (var game in container.GetGames())
                {
                    this.lbContainerGames.Items.Add(game);
                }
                this.lbContainerChallenges.Items.Clear();

                foreach (var game in container.GetChallenges())
                {
                    this.lbContainerChallenges.Items.Add(game);
                }
            }
        }

        private async void bObserveGame_Click(object sender, EventArgs e)
        {
            if (this.lbContainerGames.SelectedItem != null)
            {
                var game = (KgsGameInfo) this.lbContainerGames.SelectedItem;
                await kgs.Commands.ObserveGameAsync(game);
            }
        }

        private async void timerIdle_Tick(object sender, EventArgs e)
        {
            if (this.kgs.LoggedIn)
            {
                await kgs.Commands.WakeUpAsync();
            }
        }

        private async void bLogout_Click(object sender, EventArgs e)
        {
            await kgs.Commands.LogoutAsync();
        }

        private async void bLogin_Click(object sender, EventArgs e)
        {

            await kgs.LoginAsync("OmegaGo1", "123456789");
        }

        private void bRefreshJoinedChannels_Click(object sender, EventArgs e)
        {
            this.lbJoinedChannels.Items.Clear();
            foreach(var channel in kgs.Data.Channels)
            {
                if (channel.Value.Joined)
                {
                    this.lbJoinedChannels.Items.Add(channel.Value);
                }
            }
        }

        private async void bAccept_Click(object sender, EventArgs e)
        {
            if (this.lbContainerChallenges.SelectedItem != null)
            {
                await this.kgs.Commands.AcceptChallengeAsync((KgsChallenge)this.lbContainerChallenges.SelectedItem);
            }
        }

        private void bClearTrivialMessages_Click(object sender, EventArgs e)
        {
            for (int i = this.lbAllIncomingMessages.Items.Count - 1; i >= 0; i--)
            {
                var msg = (JsonResponse)this.lbAllIncomingMessages.Items[i];
                if (IsTrivial(msg))
                {
                    this.lbAllIncomingMessages.Items.RemoveAt(i);
                }
            }
        }

        private bool IsTrivial(JsonResponse msg)
        {
            switch (msg.Type)
            {
                case "USER_UPDATE":
                case "GAME_LIST":
                case "GAME_CONTAINER_REMOVE_GAME":
                    return true;
            }
            return false;
        }
    }
}
