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
                byte[] buffer = await client.GetResource("http://localhost:" + port.ToString() + "/" + fileName);
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
    }
}
