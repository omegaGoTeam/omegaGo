namespace FormsPrototype
{
    partial class ChallengeForm
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.bAccept = new System.Windows.Forms.Button();
            this.bUnjoin = new System.Windows.Forms.Button();
            this.bCancelOffer = new System.Windows.Forms.Button();
            this.lbEvents = new System.Windows.Forms.ListBox();
            this.bRefreshEvents = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Info:";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblInfo.Location = new System.Drawing.Point(62, 67);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(64, 18);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "InfoHere";
            // 
            // bAccept
            // 
            this.bAccept.Enabled = false;
            this.bAccept.Location = new System.Drawing.Point(26, 239);
            this.bAccept.Name = "bAccept";
            this.bAccept.Size = new System.Drawing.Size(171, 47);
            this.bAccept.TabIndex = 2;
            this.bAccept.Text = "Accept";
            this.bAccept.UseVisualStyleBackColor = true;
            this.bAccept.Click += new System.EventHandler(this.bAccept_Click);
            // 
            // bUnjoin
            // 
            this.bUnjoin.Location = new System.Drawing.Point(203, 239);
            this.bUnjoin.Name = "bUnjoin";
            this.bUnjoin.Size = new System.Drawing.Size(171, 47);
            this.bUnjoin.TabIndex = 3;
            this.bUnjoin.Text = "Unjoin";
            this.bUnjoin.UseVisualStyleBackColor = true;
            this.bUnjoin.Click += new System.EventHandler(this.bUnjoin_Click);
            // 
            // bCancelOffer
            // 
            this.bCancelOffer.Enabled = false;
            this.bCancelOffer.Location = new System.Drawing.Point(380, 239);
            this.bCancelOffer.Name = "bCancelOffer";
            this.bCancelOffer.Size = new System.Drawing.Size(171, 47);
            this.bCancelOffer.TabIndex = 4;
            this.bCancelOffer.Text = "Cancel offer";
            this.bCancelOffer.UseVisualStyleBackColor = true;
            // 
            // lbEvents
            // 
            this.lbEvents.FormattingEnabled = true;
            this.lbEvents.Location = new System.Drawing.Point(26, 138);
            this.lbEvents.Name = "lbEvents";
            this.lbEvents.Size = new System.Drawing.Size(525, 95);
            this.lbEvents.TabIndex = 5;
            // 
            // bRefreshEvents
            // 
            this.bRefreshEvents.Location = new System.Drawing.Point(465, 148);
            this.bRefreshEvents.Name = "bRefreshEvents";
            this.bRefreshEvents.Size = new System.Drawing.Size(75, 23);
            this.bRefreshEvents.TabIndex = 6;
            this.bRefreshEvents.Text = "Refresh";
            this.bRefreshEvents.UseVisualStyleBackColor = true;
            this.bRefreshEvents.Click += new System.EventHandler(this.bRefreshEvents_Click);
            // 
            // ChallengeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 313);
            this.Controls.Add(this.bRefreshEvents);
            this.Controls.Add(this.lbEvents);
            this.Controls.Add(this.bCancelOffer);
            this.Controls.Add(this.bUnjoin);
            this.Controls.Add(this.bAccept);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.label1);
            this.Name = "ChallengeForm";
            this.Text = "KGS Challenge";
            this.Load += new System.EventHandler(this.ChallengeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button bAccept;
        private System.Windows.Forms.Button bUnjoin;
        private System.Windows.Forms.Button bCancelOffer;
        private System.Windows.Forms.ListBox lbEvents;
        private System.Windows.Forms.Button bRefreshEvents;
    }
}