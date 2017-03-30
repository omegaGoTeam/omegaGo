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
using OmegaGo.Core.Online.Kgs.Structures;

namespace FormsPrototype
{
    public partial class ChallengeForm : Form
    {
        private KgsChallenge challenge;
        private readonly KgsConnection _connection;

        public ChallengeForm()
        {
        }

        public ChallengeForm(KgsChallenge challenge, KgsConnection connection)
        {
            InitializeComponent();
            this.challenge = challenge;
            this._connection = connection;
            this.lblInfo.Text = challenge.ToString();
            connection.Events.Unjoin += Events_Unjoin;
            RefreshEvents();
        }

        private void Events_Unjoin(object sender, KgsChannel e)
        {
            this.Close();
        }

        private void RefreshEvents()
        {
            this.lbEvents.Items.Clear();
            foreach(var e in challenge.Events)
            {
                this.lbEvents.Items.Add(e);
            }
        }

        private void bRefreshEvents_Click(object sender, EventArgs e)
        {
            RefreshEvents();
        }

        private async void bUnjoin_Click(object sender, EventArgs e)
        {
            this.bUnjoin.Enabled = false;
            await _connection.Commands.GenericUnjoinAsync(challenge);
            
        }

        private async void bAccept_Click(object sender, EventArgs e)
        {
            await _connection.Commands.AcceptChallenge(challenge);
        }
    }
}
