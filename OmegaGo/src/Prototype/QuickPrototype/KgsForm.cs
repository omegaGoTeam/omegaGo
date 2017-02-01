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
        }

        private async void KgsForm_Load(object sender, EventArgs e)
        {
            if (!await kgs.Login("Soothie", "fdsfd"))
            {
                MessageBox.Show("Fail.");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            if (!await kgs.Login("OmegaGo1", "123456789"))
            {
                MessageBox.Show("Fail.");
            }
        }
    }
}
