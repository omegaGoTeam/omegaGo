namespace QuickPrototype
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
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            this.chEnforceRules = new System.Windows.Forms.CheckBox();
            this.tbSystemMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bRefreshPicture = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupboxMoveMaker.SuspendLayout();
            this.panelEnd.SuspendLayout();
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(21, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "IGS Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 76);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(474, 411);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(21, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(160, 22);
            this.button2.TabIndex = 4;
            this.button2.Text = "IGS Raw Refresh";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(493, 105);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 90);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " IGS ";
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
            this.panelEnd.Location = new System.Drawing.Point(12, 502);
            this.panelEnd.Name = "panelEnd";
            this.panelEnd.Size = new System.Drawing.Size(681, 67);
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
            // chEnforceRules
            // 
            this.chEnforceRules.AutoSize = true;
            this.chEnforceRules.Checked = true;
            this.chEnforceRules.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chEnforceRules.Location = new System.Drawing.Point(496, 76);
            this.chEnforceRules.Name = "chEnforceRules";
            this.chEnforceRules.Size = new System.Drawing.Size(93, 17);
            this.chEnforceRules.TabIndex = 13;
            this.chEnforceRules.Text = "Enforce Rules";
            this.chEnforceRules.UseVisualStyleBackColor = true;
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
            this.tbLog.Location = new System.Drawing.Point(699, 320);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(322, 249);
            this.tbLog.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(711, 299);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "System Log:";
            // 
            // bRefreshPicture
            // 
            this.bRefreshPicture.Location = new System.Drawing.Point(370, 47);
            this.bRefreshPicture.Name = "bRefreshPicture";
            this.bRefreshPicture.Size = new System.Drawing.Size(116, 23);
            this.bRefreshPicture.TabIndex = 18;
            this.bRefreshPicture.Text = "Refresh Local Board";
            this.bRefreshPicture.UseVisualStyleBackColor = true;
            this.bRefreshPicture.Click += new System.EventHandler(this.bRefreshPicture_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(248, 47);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(116, 23);
            this.button3.TabIndex = 19;
            this.button3.Text = "Timelapse";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // InGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1033, 581);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.bRefreshPicture);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbSystemMessage);
            this.Controls.Add(this.chEnforceRules);
            this.Controls.Add(this.panelEnd);
            this.Controls.Add(this.groupboxMoveMaker);
            this.Controls.Add(this.lblTurnPlayer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Name = "InGameForm";
            this.Text = "InGameForm";
            this.Load += new System.EventHandler(this.InGameForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupboxMoveMaker.ResumeLayout(false);
            this.groupboxMoveMaker.PerformLayout();
            this.panelEnd.ResumeLayout(false);
            this.panelEnd.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.CheckBox chEnforceRules;
        private System.Windows.Forms.TextBox tbSystemMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button bRefreshPicture;
        private System.Windows.Forms.Button button3;
    }
}