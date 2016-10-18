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
    public partial class Form1 : Form
    {
        private IgsConnection igs = new IgsConnection();
        public Form1()
        {
            InitializeComponent();

        }

        private void Igs_LogEvent(string obj)
        {
           this.tbConsole.AppendText(Environment.NewLine + obj);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            List<Game> games = await igs.ListGamesInProgress();
                    this.lbGames.Items.Clear();
                    this.lbGames.Items.AddRange(games.ToArray());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.igs.SendRawText(this.tbCommand.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            igs = new IgsConnection();
            igs.LogEvent += Igs_LogEvent;
            igs.EnsureConnected();
            //igs.Login("OmegaGoBot", "123456789");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            igs.SendRawText("toggle client");
        }
    }
}
