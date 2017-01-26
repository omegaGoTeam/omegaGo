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
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Rules;
using StoneColor = OmegaGo.Core.Game.StoneColor;

// ReSharper disable CoVariantArrayConversion

namespace FormsPrototype
{
    public partial class PrimaryForm : Form
    {
        private IgsConnection igs;
        private List<OnlineGameInfo> observableGames;

        public PrimaryForm()
        {
            InitializeComponent();

        }

        private void Igs_LogEvent(object sender, string obj)
        {
           this.tbConsole.AppendText(Environment.NewLine + obj);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.observableGames = await igs.ListGamesInProgressAsync();
            this.lbGames.Items.Clear();
            this.lbGames.Items.AddRange(this.observableGames.ToArray());
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
            this.cbWhoPlaysOnline.Items.Clear();
            this.cbWhoPlaysOnline.Items.Add("Human");
            this.cbWhoPlaysOnline.Items.AddRange(AISystems.AiPrograms.ToArray());
            this.cbWhite.SelectedIndex = 0;
            this.cbBlack.SelectedIndex = 0;
            this.cbWhoPlaysOnline.SelectedIndex = 0;

            igs = new IgsConnection();
            igs.LogEvent += Igs_LogEvent;
            igs.IncomingChatMessage += Igs_IncomingChatMessage;
            igs.Beep += Igs_Beep;
            igs.UnhandledLine += Igs_UnhandledLine;
            igs.IncomingMatchRequest += Igs_IncomingMatchRequest;
            igs.IncomingShoutMessage += Igs_IncomingShoutMessage;
            igs.OutgoingLine += Igs_OutgoingLine;
         // TODO
            //   igs.MatchRequestAccepted += Igs_MatchRequestAccepted;
            igs.MatchRequestDeclined += Igs_MatchRequestDeclined;
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

        /*
        private async void Igs_MatchRequestAccepted(object sender, ObsoleteGameInfo game)
        {
            //game.Ruleset.startGame(game.Players[1], game.Players[0], game.BoardSize);
            GamePlayer localPlayer = game.Players[0].Name == "OmegaGo1" ? game.Players[0] : game.Players[1]; // TODO hardcoded username
            GamePlayer networkPlayer = game.OpponentOf(localPlayer);
            await game.AbsorbAdditionalInformation(); // TODO this should maybe be more hidden
            InGameForm ingameForm = new InGameForm(game, igs);
            localPlayer.Agent = CreateAgentFromComboboxObject(ingameForm, this.cbWhoPlaysOnline.SelectedItem);
            networkPlayer.Agent = new ObsoleteOnlineAgent();
            ingameForm.Show();
        }*/

        private void Igs_MatchRequestDeclined(object sender, string e)
        {
            MessageBox.Show("Our match request was declined by '" + e + "'. Boo '" + e + "'.");
        }

        private void Igs_OutgoingLine(string obj)
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
        

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.lbGames.SelectedItem != null)
            {
                OnlineGameInfo game = (OnlineGameInfo)lbGames.SelectedItem;
                // game.Ruleset = new JapaneseRuleset(game.BoardSize);
                //game.Ruleset.startGame(game.White, game.Black, game.BoardSize);
               //  game.StartObserving();
                //igs.RefreshBoard(game);
                /*InGameForm observing = new InGameForm(game, igs);
                observing.Show();
                this.lbObservedGames.Items.Add(game);
                */
                // TODo
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // TODO
            /**
            if (this.lbObservedGames.SelectedItem != null)
            {

                ObsoleteGameInfo game = (ObsoleteGameInfo)lbGames.SelectedItem;
                game.StopObserving();
                this.lbObservedGames.Items.Remove(game);
            }*/
        }

        private async void bSendMessage_Click(object sender, EventArgs e)
        {
            bool success = await igs.TellAsync(this.cbMessageRecipient.Text, this.tbChatMessage.Text);
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
            this.observableGames.Sort((g1, g2) => g1.NumberOfObservers.CompareTo(g2.NumberOfObservers));
            this.observableGames.Reverse();

            this.lbGames.Items.Clear();
            this.lbGames.Items.AddRange(this.observableGames.ToArray());
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            List<IgsUser> users = await igs.ListOnlinePlayersAsync();
            this.lbUsers.Items.Clear();
            this.lbUsers.Items.AddRange(users.ToArray());

            this.cbMatchRecipient.Items.Clear();
            this.cbMatchRecipient.Items.AddRange(users.Select(usr => usr.Name).ToArray());
            this.cbMessageRecipient.Items.Clear();
            this.cbMessageRecipient.Items.AddRange(users.Select(usr => usr.Name).ToArray());
        }

        private void bPlayLocal_Click(object sender, EventArgs e)
        {
            // TODO
            /*
            OnlineGameInfo localGame = new ObsoleteGameInfo
            {
                SquareBoardSize = (int) this.nLocalBoardSize.Value,
                NumberOfMovesPlayed = 0
            };
            GamePlayer playerBlack = new GamePlayer(this.cbBlack.Text + " (Black)", "NR", localGame);
            GamePlayer playerWhite = new GamePlayer(this.cbWhite.Text + " (White)", "NR", localGame);
            localGame.Ruleset = new ChineseRuleset(localGame.BoardSize);
            localGame.Players.Add(playerBlack);
            localGame.Players.Add(playerWhite);
            localGame.Server = null;
            InGameForm ingameForm = new InGameForm(localGame, null);
            playerBlack.Agent = CreateAgentFromComboboxObject(ingameForm, this.cbBlack.SelectedItem);
            playerWhite.Agent = CreateAgentFromComboboxObject(ingameForm, this.cbWhite.SelectedItem);
            ingameForm.Show();
            */
        }

        /*
        private IObsoleteAgent CreateAgentFromComboboxObject(InGameForm form, object text)
        {
            if (text is string && ((string)text) == "Human")
            {
                ObsoleteLocalAgent localAgent =  new ObsoleteLocalAgent();
                localAgent.OnPleaseMakeAMove += form.GuiAgent_PleaseMakeAMove;
                return localAgent;
            }
            if (text is IAIProgram)
            {
                IAIProgram newInstance = (IAIProgram)Activator.CreateInstance(text.GetType());
                return new ObsoleteAIAgent(newInstance);
            }
            throw new Exception("This agent cannot be handled yet.");
        }
        */
        private async void button6_Click_1(object sender, EventArgs e)
        {
            Igs_LogEvent(this, "CONNECT() RESULT: " + await this.igs.ConnectAsync());
            Igs_LogEvent(this, "LOGIN() RESULT: " + await this.igs.LoginAsync("OmegaGo1", "123456789"));
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            await this.igs.DisconnectAsync();
            Igs_LogEvent(this, "DISCONNECTED.");
        }

        private async void button5_Click(object sender, EventArgs e)
        {
           bool result = await igs.RequestBasicMatchAsync(
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
                if (await igs.DeclineMatchRequestAsync(selectedItem))
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
            igs.DEBUG_MakeUnattendedRequest(this.tbCommand.Text);
        }

        private async void bAcceptRequest_Click(object sender, EventArgs e)
        {
            // TODO
            /*
            IgsMatchRequest selectedItem = this.lbMatchRequests.SelectedItem as IgsMatchRequest;
            if (selectedItem != null)
            {
                ObsoleteGameInfo game = await igs.AcceptMatchRequestAsync(selectedItem);
                await game.AbsorbAdditionalInformation();
                if (game != null)
                {
                    this.lbMatchRequests.Items.Remove(selectedItem);
                    //game.Ruleset.startGame(game.Players[1], game.Players[0], game.BoardSize);
                    GamePlayer localPlayer = game.Players[0].Name == "OmegaGo1" ? game.Players[0] : game.Players[1]; // TODO hardcoded username
                    GamePlayer networkPlayer = game.OpponentOf(localPlayer);
                    InGameForm ingameForm = new InGameForm(game, igs);
                    localPlayer.Agent = CreateAgentFromComboboxObject(ingameForm, this.cbWhoPlaysOnline.SelectedItem);
                    networkPlayer.Agent = new ObsoleteOnlineAgent();
                    ingameForm.Show();
                }
                else
                {
                    Fail("Match request cannot be accepted.");
                }
            }
            */
        }
    }
}
