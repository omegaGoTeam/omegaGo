namespace FormsPrototype
{
    partial class InGameForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTurnPlayer = new System.Windows.Forms.Label();
            this.bMakeMove = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbInputMove = new System.Windows.Forms.TextBox();
            this.groupboxMoveMaker = new System.Windows.Forms.GroupBox();
            this.bRESIGN = new System.Windows.Forms.Button();
            this.bPASS = new System.Windows.Forms.Button();
            this.panelEnd = new System.Windows.Forms.Panel();
            this.lblGameEndReason = new System.Windows.Forms.Label();
            this.lblEndCaption = new System.Windows.Forms.Label();
            this.tbSystemMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.nAiStrength = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.bSay = new System.Windows.Forms.Button();
            this.tbSayWhat = new System.Windows.Forms.TextBox();
            this.lbPlayerChat = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbAiLog = new System.Windows.Forms.TextBox();
            this.grpLifeDeath = new System.Windows.Forms.GroupBox();
            this.bResumeAsBlack = new System.Windows.Forms.Button();
            this.bUndoLifeDeath = new System.Windows.Forms.Button();
            this.bDoneWithLifeDeathDetermination = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bLocalUndo = new System.Windows.Forms.Button();
            this.bUndoNo = new System.Windows.Forms.Button();
            this.bUndoYes = new System.Windows.Forms.Button();
            this.bUndoPlease = new System.Windows.Forms.Button();
            this.trackTimeline = new System.Windows.Forms.TrackBar();
            this.lblTimeline = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupboxMoveMaker.SuspendLayout();
            this.panelEnd.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nAiStrength)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grpLifeDeath.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTimeline)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Go board:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.PeachPuff;
            this.pictureBox1.Location = new System.Drawing.Point(12, 76);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(474, 411);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(493, 215);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Turn player:";
            // 
            // lblTurnPlayer
            // 
            this.lblTurnPlayer.AutoSize = true;
            this.lblTurnPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTurnPlayer.Location = new System.Drawing.Point(508, 240);
            this.lblTurnPlayer.Name = "lblTurnPlayer";
            this.lblTurnPlayer.Size = new System.Drawing.Size(128, 31);
            this.lblTurnPlayer.TabIndex = 7;
            this.lblTurnPlayer.Text = "Unknown";
            // 
            // bMakeMove
            // 
            this.bMakeMove.Location = new System.Drawing.Point(14, 79);
            this.bMakeMove.Name = "bMakeMove";
            this.bMakeMove.Size = new System.Drawing.Size(174, 62);
            this.bMakeMove.TabIndex = 8;
            this.bMakeMove.Text = "Make move";
            this.bMakeMove.UseVisualStyleBackColor = true;
            this.bMakeMove.Click += new System.EventHandler(this.bMakeMove_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "You are requested to make a move!";
            // 
            // tbInputMove
            // 
            this.tbInputMove.Location = new System.Drawing.Point(14, 53);
            this.tbInputMove.Name = "tbInputMove";
            this.tbInputMove.Size = new System.Drawing.Size(174, 20);
            this.tbInputMove.TabIndex = 10;
            // 
            // groupboxMoveMaker
            // 
            this.groupboxMoveMaker.Controls.Add(this.bRESIGN);
            this.groupboxMoveMaker.Controls.Add(this.bPASS);
            this.groupboxMoveMaker.Controls.Add(this.label3);
            this.groupboxMoveMaker.Controls.Add(this.tbInputMove);
            this.groupboxMoveMaker.Controls.Add(this.bMakeMove);
            this.groupboxMoveMaker.Location = new System.Drawing.Point(493, 299);
            this.groupboxMoveMaker.Name = "groupboxMoveMaker";
            this.groupboxMoveMaker.Size = new System.Drawing.Size(200, 188);
            this.groupboxMoveMaker.TabIndex = 11;
            this.groupboxMoveMaker.TabStop = false;
            this.groupboxMoveMaker.Text = "GuiAgent";
            this.groupboxMoveMaker.Visible = false;
            // 
            // bRESIGN
            // 
            this.bRESIGN.BackColor = System.Drawing.Color.Coral;
            this.bRESIGN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.bRESIGN.Location = new System.Drawing.Point(104, 147);
            this.bRESIGN.Name = "bRESIGN";
            this.bRESIGN.Size = new System.Drawing.Size(83, 35);
            this.bRESIGN.TabIndex = 12;
            this.bRESIGN.Text = "Resign";
            this.bRESIGN.UseVisualStyleBackColor = false;
            this.bRESIGN.Click += new System.EventHandler(this.bRESIGN_Click);
            // 
            // bPASS
            // 
            this.bPASS.Location = new System.Drawing.Point(15, 147);
            this.bPASS.Name = "bPASS";
            this.bPASS.Size = new System.Drawing.Size(83, 35);
            this.bPASS.TabIndex = 11;
            this.bPASS.Text = "PASS";
            this.bPASS.UseVisualStyleBackColor = true;
            this.bPASS.Click += new System.EventHandler(this.bPASS_Click);
            // 
            // panelEnd
            // 
            this.panelEnd.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panelEnd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelEnd.Controls.Add(this.lblGameEndReason);
            this.panelEnd.Controls.Add(this.lblEndCaption);
            this.panelEnd.Location = new System.Drawing.Point(492, 524);
            this.panelEnd.Name = "panelEnd";
            this.panelEnd.Size = new System.Drawing.Size(201, 65);
            this.panelEnd.TabIndex = 12;
            this.panelEnd.Visible = false;
            // 
            // lblGameEndReason
            // 
            this.lblGameEndReason.AutoSize = true;
            this.lblGameEndReason.Location = new System.Drawing.Point(10, 36);
            this.lblGameEndReason.Name = "lblGameEndReason";
            this.lblGameEndReason.Size = new System.Drawing.Size(120, 13);
            this.lblGameEndReason.TabIndex = 13;
            this.lblGameEndReason.Text = "Why did the game end?";
            // 
            // lblEndCaption
            // 
            this.lblEndCaption.AutoSize = true;
            this.lblEndCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblEndCaption.Location = new System.Drawing.Point(10, 9);
            this.lblEndCaption.Name = "lblEndCaption";
            this.lblEndCaption.Size = new System.Drawing.Size(101, 16);
            this.lblEndCaption.TabIndex = 0;
            this.lblEndCaption.Text = "Game ended.";
            // 
            // tbSystemMessage
            // 
            this.tbSystemMessage.Location = new System.Drawing.Point(187, 16);
            this.tbSystemMessage.Name = "tbSystemMessage";
            this.tbSystemMessage.ReadOnly = true;
            this.tbSystemMessage.Size = new System.Drawing.Size(449, 20);
            this.tbSystemMessage.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Last System Message:";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(699, 361);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(322, 228);
            this.tbLog.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(711, 336);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "System Log:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(699, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(322, 216);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.nAiStrength);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(314, 190);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // nAiStrength
            // 
            this.nAiStrength.Location = new System.Drawing.Point(77, 13);
            this.nAiStrength.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nAiStrength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nAiStrength.Name = "nAiStrength";
            this.nAiStrength.Size = new System.Drawing.Size(144, 20);
            this.nAiStrength.TabIndex = 1;
            this.nAiStrength.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nAiStrength.ValueChanged += new System.EventHandler(this.nAiStrength_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "AI Strength:";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.bSay);
            this.tabPage1.Controls.Add(this.tbSayWhat);
            this.tabPage1.Controls.Add(this.lbPlayerChat);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(314, 190);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Player Chat";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // bSay
            // 
            this.bSay.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.bSay.Location = new System.Drawing.Point(257, 164);
            this.bSay.Name = "bSay";
            this.bSay.Size = new System.Drawing.Size(51, 20);
            this.bSay.TabIndex = 2;
            this.bSay.Text = "say";
            this.bSay.UseVisualStyleBackColor = true;
            this.bSay.Click += new System.EventHandler(this.bSay_Click);
            // 
            // tbSayWhat
            // 
            this.tbSayWhat.Location = new System.Drawing.Point(3, 164);
            this.tbSayWhat.Name = "tbSayWhat";
            this.tbSayWhat.Size = new System.Drawing.Size(248, 20);
            this.tbSayWhat.TabIndex = 1;
            this.tbSayWhat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // lbPlayerChat
            // 
            this.lbPlayerChat.FormattingEnabled = true;
            this.lbPlayerChat.Location = new System.Drawing.Point(4, 11);
            this.lbPlayerChat.Name = "lbPlayerChat";
            this.lbPlayerChat.Size = new System.Drawing.Size(304, 147);
            this.lbPlayerChat.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(314, 190);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Player Advisory";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(47, 25);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(217, 56);
            this.button4.TabIndex = 0;
            this.button4.Text = "Get Hint from Joker23 Heuristic AI";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbAiLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(314, 190);
            this.tabPage2.TabIndex = 4;
            this.tabPage2.Text = "AI Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbAiLog
            // 
            this.tbAiLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAiLog.Location = new System.Drawing.Point(3, 3);
            this.tbAiLog.Multiline = true;
            this.tbAiLog.Name = "tbAiLog";
            this.tbAiLog.ReadOnly = true;
            this.tbAiLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbAiLog.Size = new System.Drawing.Size(308, 184);
            this.tbAiLog.TabIndex = 24;
            // 
            // grpLifeDeath
            // 
            this.grpLifeDeath.Controls.Add(this.bResumeAsBlack);
            this.grpLifeDeath.Controls.Add(this.bUndoLifeDeath);
            this.grpLifeDeath.Controls.Add(this.bDoneWithLifeDeathDetermination);
            this.grpLifeDeath.Location = new System.Drawing.Point(492, 76);
            this.grpLifeDeath.Name = "grpLifeDeath";
            this.grpLifeDeath.Size = new System.Drawing.Size(200, 126);
            this.grpLifeDeath.TabIndex = 6;
            this.grpLifeDeath.TabStop = false;
            this.grpLifeDeath.Text = "Life/Death Determination";
            this.grpLifeDeath.Visible = false;
            // 
            // bResumeAsBlack
            // 
            this.bResumeAsBlack.Location = new System.Drawing.Point(16, 86);
            this.bResumeAsBlack.Name = "bResumeAsBlack";
            this.bResumeAsBlack.Size = new System.Drawing.Size(166, 27);
            this.bResumeAsBlack.TabIndex = 15;
            this.bResumeAsBlack.Text = "Resume game (Black first)";
            this.bResumeAsBlack.UseVisualStyleBackColor = true;
            this.bResumeAsBlack.Click += new System.EventHandler(this.bResumeAsBlack_Click);
            // 
            // bUndoLifeDeath
            // 
            this.bUndoLifeDeath.Location = new System.Drawing.Point(16, 52);
            this.bUndoLifeDeath.Name = "bUndoLifeDeath";
            this.bUndoLifeDeath.Size = new System.Drawing.Size(166, 27);
            this.bUndoLifeDeath.TabIndex = 14;
            this.bUndoLifeDeath.Text = "Undo Death Marks";
            this.bUndoLifeDeath.UseVisualStyleBackColor = true;
            this.bUndoLifeDeath.Click += new System.EventHandler(this.bUndoLifeDeath_Click);
            // 
            // bDoneWithLifeDeathDetermination
            // 
            this.bDoneWithLifeDeathDetermination.Location = new System.Drawing.Point(16, 19);
            this.bDoneWithLifeDeathDetermination.Name = "bDoneWithLifeDeathDetermination";
            this.bDoneWithLifeDeathDetermination.Size = new System.Drawing.Size(166, 27);
            this.bDoneWithLifeDeathDetermination.TabIndex = 13;
            this.bDoneWithLifeDeathDetermination.Text = "Done";
            this.bDoneWithLifeDeathDetermination.UseVisualStyleBackColor = true;
            this.bDoneWithLifeDeathDetermination.Click += new System.EventHandler(this.bDoneWithLifeDeathDetermination_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bLocalUndo);
            this.groupBox1.Controls.Add(this.bUndoNo);
            this.groupBox1.Controls.Add(this.bUndoYes);
            this.groupBox1.Controls.Add(this.bUndoPlease);
            this.groupBox1.Location = new System.Drawing.Point(699, 234);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 79);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Undo";
            // 
            // bLocalUndo
            // 
            this.bLocalUndo.Location = new System.Drawing.Point(10, 50);
            this.bLocalUndo.Name = "bLocalUndo";
            this.bLocalUndo.Size = new System.Drawing.Size(291, 23);
            this.bLocalUndo.TabIndex = 3;
            this.bLocalUndo.Text = "undo locally";
            this.bLocalUndo.UseVisualStyleBackColor = true;
            this.bLocalUndo.Click += new System.EventHandler(this.bLocalUndo_Click);
            // 
            // bUndoNo
            // 
            this.bUndoNo.Location = new System.Drawing.Point(208, 21);
            this.bUndoNo.Name = "bUndoNo";
            this.bUndoNo.Size = new System.Drawing.Size(93, 23);
            this.bUndoNo.TabIndex = 2;
            this.bUndoNo.Text = "noundo";
            this.bUndoNo.UseVisualStyleBackColor = true;
            this.bUndoNo.Click += new System.EventHandler(this.bUndoNo_Click);
            // 
            // bUndoYes
            // 
            this.bUndoYes.Location = new System.Drawing.Point(109, 21);
            this.bUndoYes.Name = "bUndoYes";
            this.bUndoYes.Size = new System.Drawing.Size(93, 23);
            this.bUndoYes.TabIndex = 1;
            this.bUndoYes.Text = "undo";
            this.bUndoYes.UseVisualStyleBackColor = true;
            this.bUndoYes.Click += new System.EventHandler(this.bUndoYes_Click);
            // 
            // bUndoPlease
            // 
            this.bUndoPlease.Location = new System.Drawing.Point(10, 21);
            this.bUndoPlease.Name = "bUndoPlease";
            this.bUndoPlease.Size = new System.Drawing.Size(93, 23);
            this.bUndoPlease.TabIndex = 0;
            this.bUndoPlease.Text = "undoplease";
            this.bUndoPlease.UseVisualStyleBackColor = true;
            this.bUndoPlease.Click += new System.EventHandler(this.bUndoPlease_Click);
            // 
            // trackTimeline
            // 
            this.trackTimeline.Location = new System.Drawing.Point(12, 524);
            this.trackTimeline.Maximum = 5;
            this.trackTimeline.Name = "trackTimeline";
            this.trackTimeline.Size = new System.Drawing.Size(474, 45);
            this.trackTimeline.TabIndex = 22;
            this.trackTimeline.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackTimeline.Value = 4;
            this.trackTimeline.ValueChanged += new System.EventHandler(this.trackTimeline_ValueChanged);
            // 
            // lblTimeline
            // 
            this.lblTimeline.AutoSize = true;
            this.lblTimeline.Location = new System.Drawing.Point(22, 500);
            this.lblTimeline.Name = "lblTimeline";
            this.lblTimeline.Size = new System.Drawing.Size(49, 13);
            this.lblTimeline.TabIndex = 23;
            this.lblTimeline.Text = "Timeline:";
            // 
            // InGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1033, 601);
            this.Controls.Add(this.lblTimeline);
            this.Controls.Add(this.trackTimeline);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpLifeDeath);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbSystemMessage);
            this.Controls.Add(this.panelEnd);
            this.Controls.Add(this.groupboxMoveMaker);
            this.Controls.Add(this.lblTurnPlayer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Name = "InGameForm";
            this.Text = "InGameForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InGameForm_FormClosing);
            this.Load += new System.EventHandler(this.InGameForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupboxMoveMaker.ResumeLayout(false);
            this.groupboxMoveMaker.PerformLayout();
            this.panelEnd.ResumeLayout(false);
            this.panelEnd.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nAiStrength)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.grpLifeDeath.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackTimeline)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTurnPlayer;
        private System.Windows.Forms.Button bMakeMove;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbInputMove;
        private System.Windows.Forms.Button bRESIGN;
        private System.Windows.Forms.Button bPASS;
        private System.Windows.Forms.Panel panelEnd;
        private System.Windows.Forms.Label lblGameEndReason;
        private System.Windows.Forms.Label lblEndCaption;
        public System.Windows.Forms.GroupBox groupboxMoveMaker;
        private System.Windows.Forms.TextBox tbSystemMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.NumericUpDown nAiStrength;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button bSay;
        private System.Windows.Forms.TextBox tbSayWhat;
        private System.Windows.Forms.ListBox lbPlayerChat;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox grpLifeDeath;
        private System.Windows.Forms.Button bDoneWithLifeDeathDetermination;
        private System.Windows.Forms.Button bResumeAsBlack;
        private System.Windows.Forms.Button bUndoLifeDeath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bUndoNo;
        private System.Windows.Forms.Button bUndoYes;
        private System.Windows.Forms.Button bUndoPlease;
        private System.Windows.Forms.Button bLocalUndo;
        private System.Windows.Forms.TrackBar trackTimeline;
        private System.Windows.Forms.Label lblTimeline;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox tbAiLog;
    }
}