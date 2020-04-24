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
        string httpServerIP = "localhost";
        List<string> fileNames = new List<string>();
        List<int> filesID;
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

        public frmShowFiles(List<int> filesID, int port, string httpServerIP)
        {
            InitializeComponent();
            this.port = port;
            this.filesID = filesID;
            client = new Client();
        }

        private async void btSaveFile_Click(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndex != -1)
            {
                int fileID = filesID[lbFiles.SelectedIndex];
                byte[] buffer = null;
                try
                {
                    buffer = await client.GetResource("http://" + httpServerIP + ":" + port.ToString() 
                        + "/" + fileID.ToString());
                }
                catch (FileNotFoundException)
                {
                    ShowFileNotFound(fileID.ToString());
                    return;
                }
                string fileName;
                if (buffer != null)
                {
                    if (SaveFile.ShowDialog() == DialogResult.OK)
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

        private async void frmShowFiles_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> fileInfo = null;
            foreach (int fileID in filesID)
            {
                fileInfo = await client.GetResourceInf("http://" + httpServerIP + ":" +
                    port.ToString() + "/ " + fileID.ToString());
                fileNames.Add(fileInfo["name"]);
            }
            updateView();
        }

        void updateView()
        {
            foreach(string fileName in fileNames)
            {
                lbFiles.Items.Add(fileName);
            }
        }

    }
}
 