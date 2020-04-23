namespace Chat
{
    partial class frmShowFiles
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
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.SaveFile = new System.Windows.Forms.SaveFileDialog();
            this.btSaveFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbFiles
            // 
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.ItemHeight = 16;
            this.lbFiles.Location = new System.Drawing.Point(22, 33);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.Size = new System.Drawing.Size(260, 276);
            this.lbFiles.TabIndex = 0;
            // 
            // btSaveFile
            // 
            this.btSaveFile.Location = new System.Drawing.Point(22, 334);
            this.btSaveFile.Name = "btSaveFile";
            this.btSaveFile.Size = new System.Drawing.Size(95, 39);
            this.btSaveFile.TabIndex = 1;
            this.btSaveFile.Text = "Save file";
            this.btSaveFile.UseVisualStyleBackColor = true;
            this.btSaveFile.Click += new System.EventHandler(this.btSaveFile_Click);
            // 
            // frmShowFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 395);
            this.Controls.Add(this.btSaveFile);
            this.Controls.Add(this.lbFiles);
            this.Name = "frmShowFiles";
            this.Text = "frmShowFiles";
            this.Load += new System.EventHandler(this.frmShowFiles_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.SaveFileDialog SaveFile;
        private System.Windows.Forms.Button btSaveFile;
    }
}