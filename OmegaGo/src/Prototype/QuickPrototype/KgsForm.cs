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
            kgs.IncomingMessage += Kgs_IncomingMessage;
        }

        private void Kgs_IncomingMessage(object sender, JsonResponse e)
        {
            this.textBox1.Text += e.Fulltext + Environment.NewLine;
            this.textBox2.Text += (e.Type + Environment.NewLine);
        }

        private async void KgsForm_Load(object sender, EventArgs e)
        {
            this.textBox3.Text += ("Logging in." + Environment.NewLine);
            if (await kgs.LoginAsync("OmegaGo1", "123456789"))
            {
                this.textBox3.Text += ("Logged in." + Environment.NewLine);
                var challenges = await kgs.JoinGlobalChallengesList();
                foreach(var c in challenges)
                {
                    this.lbChallenges.Items.Add(c);
                }
                this.textBox3.Text += ("Challenges joined." + Environment.NewLine);

            }
            else
            {
                MessageBox.Show("Fail.");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {

           
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            GameChannel sel = (GameChannel) this.lbChallenges.SelectedItem;

            await kgs.SubmitChallenge(sel.channelId, sel.initialProposal, "OmegaGo1");
        }
    }
}
