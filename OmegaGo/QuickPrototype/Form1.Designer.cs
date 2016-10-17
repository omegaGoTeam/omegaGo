namespace QuickPrototype
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
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(244, 33);
            this.button1.TabIndex = 0;
            this.button1.Text = "games";
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
            this.lbObservedGames.Size = new System.Drawing.Size(244, 121);
            this.lbObservedGames.TabIndex = 4;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(273, 99);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(223, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Stop Observing";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(699, 45);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(144, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Send Command";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tbCommand
            // 
            this.tbCommand.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbCommand.Location = new System.Drawing.Point(540, 19);
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.Size = new System.Drawing.Size(303, 20);
            this.tbCommand.TabIndex = 7;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(540, 99);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(180, 74);
            this.button5.TabIndex = 8;
            this.button5.Text = "Connect! (do this first)";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(364, 148);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(170, 36);
            this.button6.TabIndex = 9;
            this.button6.Text = "Toggle Client";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 495);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
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
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}

