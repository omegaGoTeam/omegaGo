namespace QuickPrototype
{
    partial class PrimaryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.lbGames = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tbConsole = new System.Windows.Forms.TextBox();
            this.lbObservedGames = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tbCommand = new System.Windows.Forms.TextBox();
            this.lbUsers = new System.Windows.Forms.ListBox();
            this.button7 = new System.Windows.Forms.Button();
            this.cbMessageRecipient = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbChat = new System.Windows.Forms.ListBox();
            this.bSendMessage = new System.Windows.Forms.Button();
            this.tbChatMessage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bSortGames = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bRejectRequest = new System.Windows.Forms.Button();
            this.bAcceptRequest = new System.Windows.Forms.Button();
            this.lbMatchRequests = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.nBoardSize = new System.Windows.Forms.NumericUpDown();
            this.cbMatchRecipient = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbBlack = new System.Windows.Forms.ComboBox();
            this.cbWhite = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.nLocalBoardSize = new System.Windows.Forms.NumericUpDown();
            this.bPlayLocal = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nBoardSize)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nLocalBoardSize)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(244, 33);
            this.button1.TabIndex = 0;
            this.button1.Text = "Refresh list of games";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbGames
            // 
            this.lbGames.FormattingEnabled = true;
            this.lbGames.Location = new System.Drawing.Point(12, 52);
            this.lbGames.Name = "lbGames";
            this.lbGames.Size = new System.Drawing.Size(244, 420);
            this.lbGames.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(22, 440);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(218, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Observe";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tbConsole
            // 
            this.tbConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbConsole.Location = new System.Drawing.Point(263, 205);
            this.tbConsole.Multiline = true;
            this.tbConsole.Name = "tbConsole";
            this.tbConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbConsole.Size = new System.Drawing.Size(580, 267);
            this.tbConsole.TabIndex = 3;
            this.tbConsole.WordWrap = false;
            // 
            // lbObservedGames
            // 
            this.lbObservedGames.FormattingEnabled = true;
            this.lbObservedGames.Location = new System.Drawing.Point(263, 12);
            this.lbObservedGames.Name = "lbObservedGames";
            this.lbObservedGames.Size = new System.Drawing.Size(244, 160);
            this.lbObservedGames.TabIndex = 4;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(270, 142);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(229, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Stop Observing";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(727, 178);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(116, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Send Command";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tbCommand
            // 
            this.tbCommand.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbCommand.Location = new System.Drawing.Point(350, 178);
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.Size = new System.Drawing.Size(371, 23);
            this.tbCommand.TabIndex = 7;
            this.tbCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCommand_KeyDown);
            // 
            // lbUsers
            // 
            this.lbUsers.FormattingEnabled = true;
            this.lbUsers.Location = new System.Drawing.Point(862, 52);
            this.lbUsers.Name = "lbUsers";
            this.lbUsers.Size = new System.Drawing.Size(244, 420);
            this.lbUsers.TabIndex = 10;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(862, 12);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(244, 33);
            this.button7.TabIndex = 11;
            this.button7.Text = "Refresh list of users";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // cbMessageRecipient
            // 
            this.cbMessageRecipient.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbMessageRecipient.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbMessageRecipient.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cbMessageRecipient.FormattingEnabled = true;
            this.cbMessageRecipient.Location = new System.Drawing.Point(20, 41);
            this.cbMessageRecipient.Name = "cbMessageRecipient";
            this.cbMessageRecipient.Size = new System.Drawing.Size(172, 23);
            this.cbMessageRecipient.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbChat);
            this.groupBox1.Controls.Add(this.bSendMessage);
            this.groupBox1.Controls.Add(this.tbChatMessage);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbMessageRecipient);
            this.groupBox1.Location = new System.Drawing.Point(12, 478);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(573, 212);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Chat";
            // 
            // lbChat
            // 
            this.lbChat.FormattingEnabled = true;
            this.lbChat.Location = new System.Drawing.Point(20, 94);
            this.lbChat.Name = "lbChat";
            this.lbChat.Size = new System.Drawing.Size(547, 108);
            this.lbChat.TabIndex = 17;
            // 
            // bSendMessage
            // 
            this.bSendMessage.Location = new System.Drawing.Point(465, 67);
            this.bSendMessage.Name = "bSendMessage";
            this.bSendMessage.Size = new System.Drawing.Size(102, 23);
            this.bSendMessage.TabIndex = 16;
            this.bSendMessage.Text = "Send message";
            this.bSendMessage.UseVisualStyleBackColor = true;
            this.bSendMessage.Click += new System.EventHandler(this.bSendMessage_Click);
            // 
            // tbChatMessage
            // 
            this.tbChatMessage.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbChatMessage.Location = new System.Drawing.Point(208, 42);
            this.tbChatMessage.Name = "tbChatMessage";
            this.tbChatMessage.Size = new System.Drawing.Size(359, 23);
            this.tbChatMessage.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(205, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Message:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Tell to:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Telnet console:";
            // 
            // bSortGames
            // 
            this.bSortGames.Location = new System.Drawing.Point(22, 408);
            this.bSortGames.Name = "bSortGames";
            this.bSortGames.Size = new System.Drawing.Size(218, 26);
            this.bSortGames.TabIndex = 16;
            this.bSortGames.Text = "Sort by observer count, descending.";
            this.bSortGames.UseVisualStyleBackColor = true;
            this.bSortGames.Click += new System.EventHandler(this.bSortGames_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bRejectRequest);
            this.groupBox2.Controls.Add(this.bAcceptRequest);
            this.groupBox2.Controls.Add(this.lbMatchRequests);
            this.groupBox2.Location = new System.Drawing.Point(591, 478);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 212);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Incoming match requests";
            // 
            // bRejectRequest
            // 
            this.bRejectRequest.Location = new System.Drawing.Point(149, 153);
            this.bRejectRequest.Name = "bRejectRequest";
            this.bRejectRequest.Size = new System.Drawing.Size(75, 33);
            this.bRejectRequest.TabIndex = 2;
            this.bRejectRequest.Text = "Reject";
            this.bRejectRequest.UseVisualStyleBackColor = true;
            // 
            // bAcceptRequest
            // 
            this.bAcceptRequest.BackColor = System.Drawing.Color.YellowGreen;
            this.bAcceptRequest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.bAcceptRequest.Location = new System.Drawing.Point(7, 153);
            this.bAcceptRequest.Name = "bAcceptRequest";
            this.bAcceptRequest.Size = new System.Drawing.Size(136, 33);
            this.bAcceptRequest.TabIndex = 1;
            this.bAcceptRequest.Text = "Accept";
            this.bAcceptRequest.UseVisualStyleBackColor = false;
            // 
            // lbMatchRequests
            // 
            this.lbMatchRequests.FormattingEnabled = true;
            this.lbMatchRequests.Location = new System.Drawing.Point(7, 25);
            this.lbMatchRequests.Name = "lbMatchRequests";
            this.lbMatchRequests.Size = new System.Drawing.Size(217, 121);
            this.lbMatchRequests.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.button5);
            this.groupBox3.Controls.Add(this.nBoardSize);
            this.groupBox3.Controls.Add(this.cbMatchRecipient);
            this.groupBox3.Location = new System.Drawing.Point(827, 478);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(268, 212);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Request a match";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(197, 26);
            this.label6.TabIndex = 23;
            this.label6.Text = "No handicaps. \r\nTiming will be 10/10 Canadian byo-yomi.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Board size:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Opponent:";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(43, 131);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(182, 40);
            this.button5.TabIndex = 20;
            this.button5.Text = "Request match (match)";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // nBoardSize
            // 
            this.nBoardSize.Location = new System.Drawing.Point(74, 55);
            this.nBoardSize.Maximum = new decimal(new int[] {
            19,
            0,
            0,
            0});
            this.nBoardSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nBoardSize.Name = "nBoardSize";
            this.nBoardSize.Size = new System.Drawing.Size(172, 20);
            this.nBoardSize.TabIndex = 19;
            this.nBoardSize.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // cbMatchRecipient
            // 
            this.cbMatchRecipient.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbMatchRecipient.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbMatchRecipient.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cbMatchRecipient.FormattingEnabled = true;
            this.cbMatchRecipient.Location = new System.Drawing.Point(74, 25);
            this.cbMatchRecipient.Name = "cbMatchRecipient";
            this.cbMatchRecipient.Size = new System.Drawing.Size(172, 23);
            this.cbMatchRecipient.TabIndex = 18;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.bPlayLocal);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.cbWhite);
            this.groupBox4.Controls.Add(this.nLocalBoardSize);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.cbBlack);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Location = new System.Drawing.Point(521, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(322, 138);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = " Local Play ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Black:";
            // 
            // cbBlack
            // 
            this.cbBlack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBlack.FormattingEnabled = true;
            this.cbBlack.Items.AddRange(new object[] {
            "Human",
            "Defeatist",
            "Random"});
            this.cbBlack.Location = new System.Drawing.Point(92, 26);
            this.cbBlack.Name = "cbBlack";
            this.cbBlack.Size = new System.Drawing.Size(121, 21);
            this.cbBlack.TabIndex = 1;
            // 
            // cbWhite
            // 
            this.cbWhite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWhite.FormattingEnabled = true;
            this.cbWhite.Items.AddRange(new object[] {
            "Human",
            "Defeatist",
            "Random"});
            this.cbWhite.Location = new System.Drawing.Point(92, 53);
            this.cbWhite.Name = "cbWhite";
            this.cbWhite.Size = new System.Drawing.Size(121, 21);
            this.cbWhite.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(23, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "White:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 82);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Board size:";
            // 
            // nLocalBoardSize
            // 
            this.nLocalBoardSize.Location = new System.Drawing.Point(92, 80);
            this.nLocalBoardSize.Maximum = new decimal(new int[] {
            19,
            0,
            0,
            0});
            this.nLocalBoardSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nLocalBoardSize.Name = "nLocalBoardSize";
            this.nLocalBoardSize.Size = new System.Drawing.Size(121, 20);
            this.nLocalBoardSize.TabIndex = 24;
            this.nLocalBoardSize.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // bPlayLocal
            // 
            this.bPlayLocal.Location = new System.Drawing.Point(70, 109);
            this.bPlayLocal.Name = "bPlayLocal";
            this.bPlayLocal.Size = new System.Drawing.Size(116, 23);
            this.bPlayLocal.TabIndex = 20;
            this.bPlayLocal.Text = "Play Go";
            this.bPlayLocal.UseVisualStyleBackColor = true;
            this.bPlayLocal.Click += new System.EventHandler(this.bPlayLocal_Click);
            // 
            // PrimaryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 702);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.bSortGames);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.lbUsers);
            this.Controls.Add(this.tbCommand);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.lbObservedGames);
            this.Controls.Add(this.tbConsole);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lbGames);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Name = "PrimaryForm";
            this.Text = "IGS Tester";
            this.Load += new System.EventHandler(this.PrimaryForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nBoardSize)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nLocalBoardSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox lbGames;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox tbConsole;
        private System.Windows.Forms.ListBox lbObservedGames;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox tbCommand;
        private System.Windows.Forms.ListBox lbUsers;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox cbMessageRecipient;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbChat;
        private System.Windows.Forms.Button bSendMessage;
        private System.Windows.Forms.TextBox tbChatMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bSortGames;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bRejectRequest;
        private System.Windows.Forms.Button bAcceptRequest;
        private System.Windows.Forms.ListBox lbMatchRequests;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.NumericUpDown nBoardSize;
        private System.Windows.Forms.ComboBox cbMatchRecipient;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button bPlayLocal;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbWhite;
        private System.Windows.Forms.NumericUpDown nLocalBoardSize;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbBlack;
        private System.Windows.Forms.Label label7;
    }
}

