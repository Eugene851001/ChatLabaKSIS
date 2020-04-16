using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientHttp;
using System.IO;

namespace Chat
{
    public partial class frmShowFiles : Form
    {
        Client client;
        int port;
        public frmShowFiles()
        {
            InitializeComponent();
            client = new Client();
        }

        void ShowFileNotFound(string fileName)
        {
            MessageBox.Show("The file " + fileName + " is not found", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public frmShowFiles(List<string> filesNames, int port)
        {
            InitializeComponent();
            this.port = port;
            client = new Client();
            foreach(string fileName in filesNames)
            {
                lbFiles.Items.Add(fileName);
            }
        }

        private async void btSaveFile_Click(object sender, EventArgs e)
        {
            if(lbFiles.SelectedIndex != -1)
            {
                string fileName = (string)lbFiles.SelectedItem;
                byte[] buffer = null;
                try
                {
                    buffer = await client.GetResource("http://localhost:" + port.ToString() + "/" + fileName);
                }
                catch(FileNotFoundException)
                {
                    ShowFileNotFound(fileName);
                    return;
                }
                if(buffer != null)
                {
                    if(SaveFile.ShowDialog() == DialogResult.OK)
                    {
                        fileName = SaveFile.FileName;
                        FileStream fout;
                        try
                        {
                            fout = new FileStream(fileName, FileMode.Create);
                        }
                        catch
                        {
                            return;
                        }
                        try
                        {
                            fout.Write(buffer, 0, buffer.Length);
                        }
                        finally
                        {
                            fout.Close();
                        }
                    }
                }
            }
        }

        private void frmShowFiles_Load(object sender, EventArgs e)
        {

        }

        async private void btFileInfo_Click(object sender, EventArgs e)
        {
            if(lbFiles.SelectedIndex != -1)
            {
                string fileName = (string)lbFiles.SelectedItem;
                Dictionary<string, string> result = null;
                try
                {
                    result = await client.GetResourceInf("http://localhost:" +
                        port.ToString() + "/" + (string)lbFiles.SelectedItem);
                }
                catch(FileNotFoundException)
                {
                    ShowFileNotFound(fileName);
                }
                if (result != null)
                {
                    MessageBox.Show("File name: " + result["name"] + "File size: " + result["size"], "File Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                } 
                else
                {
                    MessageBox.Show("No info", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
            }
        }

    }
}
 