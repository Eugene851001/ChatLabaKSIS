namespace WindowsFormsApp1
{
    partial class frmMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbMessageContent = new System.Windows.Forms.RichTextBox();
            this.tbIPAdress = new System.Windows.Forms.TextBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btConnect = new System.Windows.Forms.Button();
            this.btSendMessage = new System.Windows.Forms.Button();
            this.cbIsConnected = new System.Windows.Forms.CheckBox();
            this.btDisconnect = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbParticipants = new System.Windows.Forms.ListBox();
            this.tbChatContent = new System.Windows.Forms.RichTextBox();
            this.btGetAddress = new System.Windows.Forms.Button();
            this.lbCurrentDialog = new System.Windows.Forms.Label();
            this.btGetHistory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbMessageContent
            // 
            this.tbMessageContent.Location = new System.Drawing.Point(95, 350);
            this.tbMessageContent.Name = "tbMessageContent";
            this.tbMessageContent.Size = new System.Drawing.Size(256, 24);
            this.tbMessageContent.TabIndex = 0;
            this.tbMessageContent.Text = "";
            // 
            // tbIPAdress
            // 
            this.tbIPAdress.Location = new System.Drawing.Point(95, 70);
            this.tbIPAdress.Name = "tbIPAdress";
            this.tbIPAdress.Size = new System.Drawing.Size(122, 22);
            this.tbIPAdress.TabIndex = 1;
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(95, 108);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(122, 22);
            this.tbPort.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "IPAdress";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // btConnect
            // 
            this.btConnect.Location = new System.Drawing.Point(263, 111);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(88, 23);
            this.btConnect.TabIndex = 5;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // btSendMessage
            // 
            this.btSendMessage.Location = new System.Drawing.Point(95, 380);
            this.btSendMessage.Name = "btSendMessage";
            this.btSendMessage.Size = new System.Drawing.Size(75, 23);
            this.btSendMessage.TabIndex = 6;
            this.btSendMessage.Text = "Send";
            this.btSendMessage.UseVisualStyleBackColor = true;
            this.btSendMessage.Click += new System.EventHandler(this.btSendMessage_Click);
            // 
            // cbIsConnected
            // 
            this.cbIsConnected.AutoSize = true;
            this.cbIsConnected.Location = new System.Drawing.Point(660, 403);
            this.cbIsConnected.Name = "cbIsConnected";
            this.cbIsConnected.Size = new System.Drawing.Size(110, 21);
            this.cbIsConnected.TabIndex = 7;
            this.cbIsConnected.Text = "Is connected";
            this.cbIsConnected.UseVisualStyleBackColor = true;
            // 
            // btDisconnect
            // 
            this.btDisconnect.Location = new System.Drawing.Point(263, 69);
            this.btDisconnect.Name = "btDisconnect";
            this.btDisconnect.Size = new System.Drawing.Size(88, 23);
            this.btDisconnect.TabIndex = 8;
            this.btDisconnect.Text = "Disconnect";
            this.btDisconnect.UseVisualStyleBackColor = true;
            this.btDisconnect.Click += new System.EventHandler(this.btDisconnect_Click);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(95, 28);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(122, 22);
            this.tbName.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(647, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Participants";
            // 
            // lbParticipants
            // 
            this.lbParticipants.FormattingEnabled = true;
            this.lbParticipants.ItemHeight = 16;
            this.lbParticipants.Location = new System.Drawing.Point(626, 87);
            this.lbParticipants.Name = "lbParticipants";
            this.lbParticipants.Size = new System.Drawing.Size(120, 132);
            this.lbParticipants.TabIndex = 13;
            this.lbParticipants.SelectedIndexChanged += new System.EventHandler(this.lbParticipants_SelectedIndexChanged);
            // 
            // tbChatContent
            // 
            this.tbChatContent.Location = new System.Drawing.Point(95, 201);
            this.tbChatContent.Name = "tbChatContent";
            this.tbChatContent.Size = new System.Drawing.Size(256, 132);
            this.tbChatContent.TabIndex = 14;
            this.tbChatContent.Text = "";
            // 
            // btGetAddress
            // 
            this.btGetAddress.Location = new System.Drawing.Point(263, 27);
            this.btGetAddress.Name = "btGetAddress";
            this.btGetAddress.Size = new System.Drawing.Size(88, 23);
            this.btGetAddress.TabIndex = 16;
            this.btGetAddress.Text = "Find";
            this.btGetAddress.UseVisualStyleBackColor = true;
            this.btGetAddress.Click += new System.EventHandler(this.btGetAddress_Click);
            // 
            // lbCurrentDialog
            // 
            this.lbCurrentDialog.AutoSize = true;
            this.lbCurrentDialog.Location = new System.Drawing.Point(171, 171);
            this.lbCurrentDialog.Name = "lbCurrentDialog";
            this.lbCurrentDialog.Size = new System.Drawing.Size(100, 17);
            this.lbCurrentDialog.TabIndex = 17;
            this.lbCurrentDialog.Text = "Not connected";
            // 
            // btGetHistory
            // 
            this.btGetHistory.Location = new System.Drawing.Point(650, 237);
            this.btGetHistory.Name = "btGetHistory";
            this.btGetHistory.Size = new System.Drawing.Size(75, 49);
            this.btGetHistory.TabIndex = 18;
            this.btGetHistory.Text = "Get History";
            this.btGetHistory.UseVisualStyleBackColor = true;
            this.btGetHistory.Click += new System.EventHandler(this.btGetHistory_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btGetHistory);
            this.Controls.Add(this.lbCurrentDialog);
            this.Controls.Add(this.btGetAddress);
            this.Controls.Add(this.tbChatContent);
            this.Controls.Add(this.lbParticipants);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.btDisconnect);
            this.Controls.Add(this.cbIsConnected);
            this.Controls.Add(this.btSendMessage);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.tbIPAdress);
            this.Controls.Add(this.tbMessageContent);
            this.Name = "frmMain";
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox tbMessageContent;
        private System.Windows.Forms.TextBox tbIPAdress;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.Button btSendMessage;
        private System.Windows.Forms.CheckBox cbIsConnected;
        private System.Windows.Forms.Button btDisconnect;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbParticipants;
        private System.Windows.Forms.RichTextBox tbChatContent;
        private System.Windows.Forms.Button btGetAddress;
        private System.Windows.Forms.Label lbCurrentDialog;
        private System.Windows.Forms.Button btGetHistory;
    }
}

