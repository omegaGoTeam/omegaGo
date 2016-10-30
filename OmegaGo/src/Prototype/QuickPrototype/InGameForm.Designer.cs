﻿namespace QuickPrototype
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
            this.bPASS = new System.Windows.Forms.Button();
            this.bRESIGN = new System.Windows.Forms.Button();
            this.panelEnd = new System.Windows.Forms.Panel();
            this.lblEndCaption = new System.Windows.Forms.Label();
            this.lblGameEndReason = new System.Windows.Forms.Label();
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
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
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
            this.bMakeMove.Location = new System.Drawing.Point(14, 86);
            this.bMakeMove.Name = "bMakeMove";
            this.bMakeMove.Size = new System.Drawing.Size(174, 55);
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
            // lblGameEndReason
            // 
            this.lblGameEndReason.AutoSize = true;
            this.lblGameEndReason.Location = new System.Drawing.Point(10, 36);
            this.lblGameEndReason.Name = "lblGameEndReason";
            this.lblGameEndReason.Size = new System.Drawing.Size(120, 13);
            this.lblGameEndReason.TabIndex = 13;
            this.lblGameEndReason.Text = "Why did the game end?";
            // 
            // InGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 581);
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
    }
}