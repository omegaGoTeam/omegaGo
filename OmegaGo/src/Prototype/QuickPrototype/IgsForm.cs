﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OmegaGo.Core;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.Defeatist;
using OmegaGo.Core.AI.Random;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time;
using OmegaGo.Core.Time.Absolute;
using OmegaGo.Core.Time.Canadian;
using OmegaGo.Core.Time.Japanese;
using OmegaGo.Core.Time.None;
using StoneColor = OmegaGo.Core.Game.StoneColor;

// ReSharper disable CoVariantArrayConversion

namespace FormsPrototype
{
    public partial class IgsForm : Form
    {
        private IgsConnection igs;
        private List<IgsGameInfo> observableGames;

        public IgsForm()
        {
            InitializeComponent();

        }

        private void IgsIncomingLine(object sender, string obj)
        {
           this.tbConsole.AppendText(Environment.NewLine + obj);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.observableGames = await igs.Commands.ListGamesInProgressAsync();
            this.lbGames.Items.Clear();
            this.lbGames.Items.AddRange(this.observableGames.ToArray());
        }
        

        private async void PrimaryForm_Load(object sender, EventArgs e)
        {
            this.cbBlack.Items.Clear();
            this.cbBlack.Items.Add("Human");
            this.cbBlack.Items.AddRange(AISystems.AIPrograms.ToArray());
            this.cbWhite.Items.Clear();
            this.cbWhite.Items.Add("Human");
            this.cbWhite.Items.AddRange(AISystems.AIPrograms.ToArray());
            this.cbWhoPlaysOnline.Items.Clear();
            this.cbWhoPlaysOnline.Items.Add("Human");
            this.cbWhoPlaysOnline.Items.AddRange(AISystems.AIPrograms.ToArray());
            this.cbWhite.SelectedIndex = 0;
            this.cbBlack.SelectedIndex = 0;
            this.cbWhoPlaysOnline.SelectedIndex = 0;

            igs = new IgsConnection();
            igs.Events.IncomingLine += IgsIncomingLine;
            igs.Events.IncomingChatMessage += Igs_IncomingChatMessage;
            igs.Events.Beep += Igs_Beep;
            igs.Events.UnhandledLine += Igs_UnhandledLine;
            igs.Events.IncomingMatchRequest += Igs_IncomingMatchRequest;
            igs.Events.IncomingShoutMessage += Igs_IncomingShoutMessage;
            igs.Events.OutgoingLine += Igs_OutgoingLine;
            igs.Events.MatchRequestAccepted += Igs_MatchRequestAccepted;
            igs.Events.MatchRequestDeclined += Igs_MatchRequestDeclined;
            if (!await igs.ConnectAsync())
            {
                MessageBox.Show("Connection to IGS failed.");
                return;
            }
            if (!await igs.LoginAsync("OmegaGo1", "123456789"))
            {
                MessageBox.Show("Login failed.");
            }
        }

        
        private void Igs_MatchRequestAccepted(object sender, IgsGame game)
        {
            InGameForm ingameForm = new FormsPrototype.InGameForm(game.Info, game.Controller, igs);
            ingameForm.LoadGame(game);
            ingameForm.Show();
        }
        

        private void Igs_MatchRequestDeclined(object sender, string e)
        {
            MessageBox.Show("Our match request was declined by '" + e + "'. Boo '" + e + "'.");
        }

        private void Igs_OutgoingLine(object sender, string obj)
        {
            this.tbConsole.AppendText("> " + obj);
        }

        private void Igs_IncomingMatchRequest(OmegaGo.Core.Online.Igs.Structures.IgsMatchRequest obj)
        {
            this.lbMatchRequests.Items.Add(obj);
        }

        private void Igs_UnhandledLine(string obj)
        {
            this.lbChat.Items.Add("UNHANDLED LINE: " + obj);
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
        

        private async void button2_Click(object sender, EventArgs e)
        {
            if (this.lbGames.SelectedItem != null)
            {

                IgsGameInfo gameInfo = (IgsGameInfo)lbGames.SelectedItem;
                var obs = await igs.Commands.StartObserving(gameInfo);
                if (obs != null)
                {
                    this.lbObservedGames.Items.Add(obs);
                    InGameForm ingameForm = new FormsPrototype.InGameForm(obs.Info, obs.Controller, igs);
                    ingameForm.LoadGame(obs);
                    ingameForm.Show();
                }
                else
                {
                    MessageBox.Show("Observing failed.");
                }
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            
            if (this.lbObservedGames.SelectedItem != null)
            {

                IgsGame game = (IgsGame)lbObservedGames.SelectedItem;
                
                this.lbObservedGames.Items.Remove(game);

                if (!await igs.Commands.EndObserving(game))
                {
                    MessageBox.Show("End observation failed.");
                }
            }
        }

        private async void bSendMessage_Click(object sender, EventArgs e)
        {
            bool success = await igs.Commands.TellAsync(this.cbMessageRecipient.Text, this.tbChatMessage.Text);
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

    
        private void bSortGames_Click(object sender, EventArgs e)
        {
            this.observableGames.Sort((g1, g2) => g1.NumberOfObservers.CompareTo(g2.NumberOfObservers));
            this.observableGames.Reverse();

            this.lbGames.Items.Clear();
            this.lbGames.Items.AddRange(this.observableGames.ToArray());
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            List<IgsUser> users = await igs.Commands.ListOnlinePlayersAsync();
            this.lbUsers.Items.Clear();
            this.lbUsers.Items.AddRange(users.ToArray());

            this.cbMatchRecipient.Items.Clear();
            this.cbMatchRecipient.Items.AddRange(users.Select(usr => usr.Name).ToArray());
            this.cbMessageRecipient.Items.Clear();
            this.cbMessageRecipient.Items.AddRange(users.Select(usr => usr.Name).ToArray());
        }

        private void bPlayLocal_Click(object sender, EventArgs e)
        {
            InGameForm ingameForm = new FormsPrototype.InGameForm(null, null, null);
            LocalGame game = GameBuilder.CreateLocalGame()
                .BlackPlayer(CreateAgentFromComboboxObject(ingameForm, this.cbBlack.SelectedItem, StoneColor.Black))
                .WhitePlayer(CreateAgentFromComboboxObject(ingameForm, this.cbWhite.SelectedItem, StoneColor.White))
                .Ruleset(RulesetType.Chinese)
                .Komi(7.5f)
                .BoardSize(new GameBoardSize((int) this.nLocalBoardSize.Value))
                .Build();
            ingameForm.LoadGame(game);
            ingameForm.Show();
        }

        
        private GamePlayer CreateAgentFromComboboxObject(InGameForm form, object text, StoneColor color)
        {
            TimeControl timeControl = null;
            if (rbNoTimeControl.Checked)
            {
                timeControl = new NoTimeControl();
            }
            else if (rbAbsoluteTiming.Checked)
            {
                timeControl = new AbsoluteTimeControl(1);
            }
            else if (rbCanadianTiming.Checked)
            {
                timeControl = new CanadianTimeControl(TimeSpan.FromSeconds(10), 5, TimeSpan.FromSeconds(10));
            }
            else if (rbJapaneseTiming.Checked)
            {
                timeControl = new JapaneseTimeControl(20, 30, 3);
            }
            if (text is string && ((string)text) == "Human")
            {
                GamePlayer human = new HumanPlayerBuilder(color)
                    .Name(color.ToString())
                    .Rank("NR")
                    .Clock(timeControl)
                    .Build();
                (human.Agent as HumanAgent).MoveRequested += (e,e2) =>
                {
                    form.GuiAgent_PleaseMakeAMove(null, null);
                };
                return human;
            }
            if (text is IAIProgram)
            {
                IAIProgram newInstance = (IAIProgram)Activator.CreateInstance(text.GetType());

                GamePlayer aiPlayer = new AiPlayerBuilder(color)
                    .Name(text.ToString() + "(" + color.ToIgsCharacterString() +")")
                    .Rank("NR")
                    .Clock(timeControl)
                    .AiProgram(newInstance)
                    .Build();
                return aiPlayer;
            }
            throw new Exception("This agent cannot be handled yet.");
        }

 
        private async void button6_Click_1(object sender, EventArgs e)
        {
            IgsIncomingLine(this, "CONNECT() RESULT: " + await this.igs.ConnectAsync());
            IgsIncomingLine(this, "LOGIN() RESULT: " + await this.igs.LoginAsync("OmegaGo1", "123456789"));
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            await this.igs.DisconnectAsync();
            IgsIncomingLine(this, "DISCONNECTED.");
        }

        private async void button5_Click(object sender, EventArgs e)
        {
           bool result = await igs.Commands.RequestBasicMatchAsync(
                this.cbMatchRecipient.Text,
                StoneColor.Black,
                (int) this.nBoardSize.Value,
                10,
                10
                );
            if (result)
            {
                MessageBox.Show("Match successfully requested.");
            }
            else
            {
                MessageBox.Show("Failed to request a match.", "Match not requested", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void bRejectRequest_Click(object sender, EventArgs e)
        {
            IgsMatchRequest selectedItem = this.lbMatchRequests.SelectedItem as IgsMatchRequest;
            if (selectedItem != null) {
                if (await igs.Commands.DeclineMatchRequestAsync(selectedItem))
                {
                    this.lbMatchRequests.Items.Remove(selectedItem);
                }
                else
                {
                    Fail("Match request cannot be declined.");
                }
            }
        }

        private void Fail(string v)
        {
            MessageBox.Show(v, "Error", MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.igs.MakeUnattendedRequest(this.tbCommand.Text);
        }

        private async void bAcceptRequest_Click(object sender, EventArgs e)
        {
            
            IgsMatchRequest selectedItem = this.lbMatchRequests.SelectedItem as IgsMatchRequest;
            if (selectedItem != null)
            {
                IgsGame game = await igs.Commands.AcceptMatchRequestAsync(selectedItem);
                if (game != null)
                {
                    InGameForm ingameForm = new FormsPrototype.InGameForm(game.Info, game.Controller, igs);
                    ingameForm.LoadGame(game);
                    ingameForm.Show();
                }
                else
                {
                    Fail("Match request cannot be accepted.");
                }
            }
        }
    }
}
