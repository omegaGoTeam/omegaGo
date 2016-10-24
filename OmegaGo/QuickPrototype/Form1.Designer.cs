﻿namespace QuickPrototype
{
    partial class Form1
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
            this.button6 = new System.Windows.Forms.Button();
            this.lbUsers = new System.Windows.Forms.ListBox();
            this.button7 = new System.Windows.Forms.Button();
            this.cbMessageRecipient = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.bSendMessage = new System.Windows.Forms.Button();
            this.tbChatMessage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.button4.Location = new System.Drawing.Point(727, 184);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(116, 20);
            this.button4.TabIndex = 6;
            this.button4.Text = "Send Command";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tbCommand
            // 
            this.tbCommand.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbCommand.Location = new System.Drawing.Point(349, 184);
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.Size = new System.Drawing.Size(371, 20);
            this.tbCommand.TabIndex = 7;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(17, 19);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(159, 31);
            this.button6.TabIndex = 9;
            this.button6.Text = "Toggle Client Mode";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
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
            // 
            // cbMessageRecipient
            // 
            this.cbMessageRecipient.FormattingEnabled = true;
            this.cbMessageRecipient.Location = new System.Drawing.Point(20, 41);
            this.cbMessageRecipient.Name = "cbMessageRecipient";
            this.cbMessageRecipient.Size = new System.Drawing.Size(172, 21);
            this.cbMessageRecipient.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox1);
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
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(20, 94);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(547, 108);
            this.listBox1.TabIndex = 17;
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
            this.tbChatMessage.Location = new System.Drawing.Point(208, 42);
            this.tbChatMessage.Name = "tbChatMessage";
            this.tbChatMessage.Size = new System.Drawing.Size(359, 20);
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Location = new System.Drawing.Point(555, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 114);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Debug/Technical";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 702);
            this.Controls.Add(this.groupBox2);
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
            this.Name = "Form1";
            this.Text = "IGS Tester";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ListBox lbUsers;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox cbMessageRecipient;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button bSendMessage;
        private System.Windows.Forms.TextBox tbChatMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

