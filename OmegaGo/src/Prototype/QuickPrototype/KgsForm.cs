using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OmegaGo.Core.Online.Kgs;

namespace FormsPrototype
{
    public partial class KgsForm : Form
    {
        private KgsConnection kgs = new KgsConnection();
        public KgsForm()
        {
            InitializeComponent();
            kgs.Events.UnhandledMessage += Kgs_IncomingMessage;
            kgs.Events.IncomingMessage += Events_IncomingMessage;
            kgs.Events.SystemMessage += Events_SystemMessage;
            kgs.Events.OutgoingRequest += Events_OutgoingRequest; 
        }

        private void Events_IncomingMessage(object sender, JsonResponse e)
        {
            this.lbAllIncomingMessages.Items.Add(e.Type);
        }

        private void Events_OutgoingRequest(object sender, string e)
        {
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

        private void button1_Click(object sender, EventArgs e)
        { 

           
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private async void button4_Click(object sender, EventArgs e)
        {
            var challenges = await kgs.JoinGlobalChallengesList();
            if (challenges == null)
            {
                MessageBox.Show("Join failed.");
            }
            foreach (var c in challenges)
            {
                this.lbChallenges.Items.Add(c);
            }
            this.tbLog.Text += ("Challenges joined." + Environment.NewLine);

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
    }
}
