namespace FormsPrototype
{
    partial class KgsForm
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
            this.tbUnhandledMessagesFull = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbUnhandledMessageTypes = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tbFirstUnhandledMessage = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tbLastOutgoingMessage = new System.Windows.Forms.TextBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.bUnjoinRoom = new System.Windows.Forms.Button();
            this.bJoinRoom = new System.Windows.Forms.Button();
            this.tbRoomDescription = new System.Windows.Forms.TextBox();
            this.bLocalRoomsRefresh = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lbRooms = new System.Windows.Forms.ListBox();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.lbAllIncomingMessages = new System.Windows.Forms.ListBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.bRefreshLocalContainers = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbContainers = new System.Windows.Forms.ListBox();
            this.lbContainerGames = new System.Windows.Forms.ListBox();
            this.bObserveGame = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbUnhandledMessagesFull
            // 
            this.tbUnhandledMessagesFull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbUnhandledMessagesFull.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbUnhandledMessagesFull.Location = new System.Drawing.Point(3, 3);
            this.tbUnhandledMessagesFull.Multiline = true;
            this.tbUnhandledMessagesFull.Name = "tbUnhandledMessagesFull";
            this.tbUnhandledMessagesFull.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbUnhandledMessagesFull.Size = new System.Drawing.Size(850, 480);
            this.tbUnhandledMessagesFull.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 197);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Incoming:";
            // 
            // tbUnhandledMessageTypes
            // 
            this.tbUnhandledMessageTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbUnhandledMessageTypes.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbUnhandledMessageTypes.Location = new System.Drawing.Point(3, 3);
            this.tbUnhandledMessageTypes.Multiline = true;
            this.tbUnhandledMessageTypes.Name = "tbUnhandledMessageTypes";
            this.tbUnhandledMessageTypes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbUnhandledMessageTypes.Size = new System.Drawing.Size(850, 480);
            this.tbUnhandledMessageTypes.TabIndex = 7;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1177, 579);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(856, 486);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "System log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbLog.Location = new System.Drawing.Point(3, 3);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(850, 480);
            this.tbLog.TabIndex = 8;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbUnhandledMessageTypes);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(856, 486);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Unhandled Messages";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tbUnhandledMessagesFull);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(856, 486);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Unhandled Messages (full)";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tbFirstUnhandledMessage);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(856, 486);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "First Unhandled Message";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tbFirstUnhandledMessage
            // 
            this.tbFirstUnhandledMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFirstUnhandledMessage.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbFirstUnhandledMessage.Location = new System.Drawing.Point(3, 3);
            this.tbFirstUnhandledMessage.Multiline = true;
            this.tbFirstUnhandledMessage.Name = "tbFirstUnhandledMessage";
            this.tbFirstUnhandledMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbFirstUnhandledMessage.Size = new System.Drawing.Size(850, 480);
            this.tbFirstUnhandledMessage.TabIndex = 2;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.tbLastOutgoingMessage);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(856, 486);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Last Outgoing Message";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tbLastOutgoingMessage
            // 
            this.tbLastOutgoingMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLastOutgoingMessage.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbLastOutgoingMessage.Location = new System.Drawing.Point(3, 3);
            this.tbLastOutgoingMessage.Multiline = true;
            this.tbLastOutgoingMessage.Name = "tbLastOutgoingMessage";
            this.tbLastOutgoingMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLastOutgoingMessage.Size = new System.Drawing.Size(850, 480);
            this.tbLastOutgoingMessage.TabIndex = 3;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.bUnjoinRoom);
            this.tabPage6.Controls.Add(this.bJoinRoom);
            this.tabPage6.Controls.Add(this.tbRoomDescription);
            this.tabPage6.Controls.Add(this.bLocalRoomsRefresh);
            this.tabPage6.Controls.Add(this.label3);
            this.tabPage6.Controls.Add(this.lbRooms);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1169, 553);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Rooms";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // bUnjoinRoom
            // 
            this.bUnjoinRoom.Location = new System.Drawing.Point(363, 445);
            this.bUnjoinRoom.Name = "bUnjoinRoom";
            this.bUnjoinRoom.Size = new System.Drawing.Size(75, 23);
            this.bUnjoinRoom.TabIndex = 5;
            this.bUnjoinRoom.Text = "Unjoin";
            this.bUnjoinRoom.UseVisualStyleBackColor = true;
            this.bUnjoinRoom.Click += new System.EventHandler(this.bUnjoinRoom_Click);
            // 
            // bJoinRoom
            // 
            this.bJoinRoom.Location = new System.Drawing.Point(363, 416);
            this.bJoinRoom.Name = "bJoinRoom";
            this.bJoinRoom.Size = new System.Drawing.Size(75, 23);
            this.bJoinRoom.TabIndex = 4;
            this.bJoinRoom.Text = "Join";
            this.bJoinRoom.UseVisualStyleBackColor = true;
            this.bJoinRoom.Click += new System.EventHandler(this.bJoinRoom_Click);
            // 
            // tbRoomDescription
            // 
            this.tbRoomDescription.Location = new System.Drawing.Point(405, 52);
            this.tbRoomDescription.Multiline = true;
            this.tbRoomDescription.Name = "tbRoomDescription";
            this.tbRoomDescription.ReadOnly = true;
            this.tbRoomDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbRoomDescription.Size = new System.Drawing.Size(445, 339);
            this.tbRoomDescription.TabIndex = 3;
            // 
            // bLocalRoomsRefresh
            // 
            this.bLocalRoomsRefresh.Location = new System.Drawing.Point(201, 23);
            this.bLocalRoomsRefresh.Name = "bLocalRoomsRefresh";
            this.bLocalRoomsRefresh.Size = new System.Drawing.Size(161, 23);
            this.bLocalRoomsRefresh.TabIndex = 2;
            this.bLocalRoomsRefresh.Text = "Refresh Local Rooms";
            this.bLocalRoomsRefresh.UseVisualStyleBackColor = true;
            this.bLocalRoomsRefresh.Click += new System.EventHandler(this.bLocalRoomsRefresh_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Rooms:";
            // 
            // lbRooms
            // 
            this.lbRooms.FormattingEnabled = true;
            this.lbRooms.Location = new System.Drawing.Point(6, 52);
            this.lbRooms.Name = "lbRooms";
            this.lbRooms.Size = new System.Drawing.Size(393, 420);
            this.lbRooms.TabIndex = 0;
            this.lbRooms.SelectedIndexChanged += new System.EventHandler(this.lbRooms_SelectedIndexChanged);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.lbAllIncomingMessages);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1169, 553);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Incoming Messages";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // lbAllIncomingMessages
            // 
            this.lbAllIncomingMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbAllIncomingMessages.FormattingEnabled = true;
            this.lbAllIncomingMessages.Location = new System.Drawing.Point(3, 3);
            this.lbAllIncomingMessages.Name = "lbAllIncomingMessages";
            this.lbAllIncomingMessages.Size = new System.Drawing.Size(1163, 547);
            this.lbAllIncomingMessages.TabIndex = 0;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.bObserveGame);
            this.tabPage8.Controls.Add(this.lbContainerGames);
            this.tabPage8.Controls.Add(this.bRefreshLocalContainers);
            this.tabPage8.Controls.Add(this.label2);
            this.tabPage8.Controls.Add(this.lbContainers);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(1169, 553);
            this.tabPage8.TabIndex = 7;
            this.tabPage8.Text = "Containers";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // bRefreshLocalContainers
            // 
            this.bRefreshLocalContainers.Location = new System.Drawing.Point(214, 26);
            this.bRefreshLocalContainers.Name = "bRefreshLocalContainers";
            this.bRefreshLocalContainers.Size = new System.Drawing.Size(161, 23);
            this.bRefreshLocalContainers.TabIndex = 5;
            this.bRefreshLocalContainers.Text = "Refresh Local Rooms";
            this.bRefreshLocalContainers.UseVisualStyleBackColor = true;
            this.bRefreshLocalContainers.Click += new System.EventHandler(this.bRefreshLocalContainers_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Containers:";
            // 
            // lbContainers
            // 
            this.lbContainers.FormattingEnabled = true;
            this.lbContainers.Location = new System.Drawing.Point(19, 55);
            this.lbContainers.Name = "lbContainers";
            this.lbContainers.Size = new System.Drawing.Size(393, 420);
            this.lbContainers.TabIndex = 3;
            this.lbContainers.SelectedIndexChanged += new System.EventHandler(this.lbContainers_SelectedIndexChanged);
            // 
            // lbContainerGames
            // 
            this.lbContainerGames.FormattingEnabled = true;
            this.lbContainerGames.Location = new System.Drawing.Point(418, 55);
            this.lbContainerGames.Name = "lbContainerGames";
            this.lbContainerGames.Size = new System.Drawing.Size(393, 420);
            this.lbContainerGames.TabIndex = 6;
            // 
            // bObserveGame
            // 
            this.bObserveGame.Location = new System.Drawing.Point(668, 481);
            this.bObserveGame.Name = "bObserveGame";
            this.bObserveGame.Size = new System.Drawing.Size(161, 23);
            this.bObserveGame.TabIndex = 7;
            this.bObserveGame.Text = "Observe";
            this.bObserveGame.UseVisualStyleBackColor = true;
            // 
            // KgsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1177, 579);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Name = "KgsForm";
            this.Text = "KgsForm";
            this.Load += new System.EventHandler(this.KgsForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbUnhandledMessagesFull;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbUnhandledMessageTypes;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox tbFirstUnhandledMessage;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TextBox tbLastOutgoingMessage;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TextBox tbRoomDescription;
        private System.Windows.Forms.Button bLocalRoomsRefresh;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbRooms;
        private System.Windows.Forms.Button bJoinRoom;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.ListBox lbAllIncomingMessages;
        private System.Windows.Forms.Button bUnjoinRoom;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.Button bRefreshLocalContainers;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbContainers;
        private System.Windows.Forms.ListBox lbContainerGames;
        private System.Windows.Forms.Button bObserveGame;
    }
}