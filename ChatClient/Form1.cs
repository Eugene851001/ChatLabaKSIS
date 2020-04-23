using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Chat;
using System.Threading;
using System.IO;
using System.Net.Http;

namespace ChatClient
{
    public partial class frmMain : Form
    {
        const int chatDialogId = 0;
        const int httpPort = 8009;
        const long MaxFileSize = 1024 * 1024 * 50;
        const long MaxFileTotalSize = 1024 * 1024 * 500;

        long  FileTotalSizeCounter = 0;

        ClientHttp.Client clientHttp;
        Client client;
        Dictionary<int, ChatDialogInfo> chatDialogsInfo;
        List<EndPointNamePair> clientsList;
      //  List<string> fileNames;
        int CurrentDialogId = chatDialogId;
        int selectedIndex = 0;

      
        bool IsLoadingFiles = false;
        List<string> unacceptableExtension = new List<string>() { ".exe", ".dll" };
        Dictionary<int, string> loadedFiles = new Dictionary<int, string>();

        public frmMain()
        {
            InitializeComponent();
            clientsList = new List<EndPointNamePair>();
            clientsList.Add(new EndPointNamePair() { Key = chatDialogId, Value = "Chat" });
            chatDialogsInfo = new Dictionary<int, ChatDialogInfo>();
            chatDialogsInfo.Add(chatDialogId, new ChatDialogInfo("Chat"));
            client = new Client();
            clientHttp = new ClientHttp.Client();
            client.ReceiveMessageHandler += HandleReceivedMessages;
        }

        public void HandleReceivedMessages(Chat.Message message)
        {
            switch(message.messageType)
            {
                case MessageType.Regular:
                    chatDialogsInfo[chatDialogId].AddMessage(message);
                    break;
                case MessageType.Private:
                    chatDialogsInfo[message.SenderID].AddMessage(message);
                    break;
                case MessageType.ClientsList:
                    {
                        Action action = delegate
                        {
                            clientsList.Clear();
                            clientsList.Add(new EndPointNamePair() { Key = chatDialogId, Value = "Chat" });
                            foreach (EndPointNamePair nameClient in message.clientsNames)
                            {
                                clientsList.Add(nameClient);
                                if (!chatDialogsInfo.ContainsKey(nameClient.Key))
                                {
                                    chatDialogsInfo.Add(nameClient.Key, new ChatDialogInfo(nameClient.Value));
                                }
                            }
                        };
                        if (InvokeRequired)
                            Invoke(action);
                        else
                            action();
                    }
                    break;
                case MessageType.SearchResponse:
                    {
                        Action action = delegate
                        {
                            tbIPAdress.Text = message.IPAdress;
                            tbPort.Text = message.Port.ToString();
                        };
                        if (InvokeRequired)
                            Invoke(action);
                        else
                            action();
                    }
                    break;
                case MessageType.History:
                    chatDialogsInfo[message.ReceiverID].Messages = message.MessageHistory;
                    break;
                default:
                    return;
            }
            UpdateView();
        }

        string ShowMessageContent(Chat.Message message)
        {
            string visualMessage = (message.Name + ": " + message.Content);
            if (message.FilesID != null)
            {
                foreach (int fileName in message.FilesID)
                {
                    visualMessage += "\n[FILE] " + fileName.ToString();
                }
            }
            return visualMessage;
        }

        public void UpdateView()
        {
            Action action = delegate
            {
                if (CurrentDialogId == chatDialogId)
                {
                    lbCurrentDialog.Text = "Main chat";
                }
                else
                {
                    lbCurrentDialog.Text = chatDialogsInfo[CurrentDialogId].Name;
                }
                lbChatContent.Items.Clear();
                if (chatDialogsInfo != null)
                {
                    List<Chat.Message> messages = new List<Chat.Message>(chatDialogsInfo[CurrentDialogId].Messages);
                    foreach (Chat.Message message in messages)
                    {
                        lbChatContent.Items.Add(ShowMessageContent(message));
                    }
                }
                lbParticipants.Items.Clear();
                foreach (EndPointNamePair clientName in clientsList)
                {
                    lbParticipants.Items.Add(clientName.Value + " " + ((chatDialogsInfo[clientName.Key].UnreadMessageCounter != 0) ?
                        chatDialogsInfo[clientName.Key].UnreadMessageCounter.ToString() : ""));
                }
                lbFiles.Items.Clear();
                foreach(KeyValuePair<int, string> fileName in loadedFiles)
                {
                    lbFiles.Items.Add(fileName);
                }
            };
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btSendMessage_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                Chat.Message message;
                if (CurrentDialogId == chatDialogId)
                {
                    message = new Chat.Message(tbMessageContent.Text);
                    message.FilesID = new List<int>(loadedFiles.Keys.AsEnumerable());
                }
                else
                {
                    message = new Chat.Message(clientsList[selectedIndex].Key, tbMessageContent.Text);
                    message.FilesID = new List<int>(loadedFiles.Keys.AsEnumerable());
                    message.Name = "Me";
                    chatDialogsInfo[message.ReceiverID].Messages.Add(message);
                }
                if (cbIsConnected.Checked)
                {
                    if (IsLoadingFiles)
                    {
                        MessageBox.Show("Please wait for file loading", "Not ready",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        client.SendMessage(message);
                        loadedFiles.Clear();
                        UpdateView();
                    }
                }
            }
        }

        private void lst_DrawItem(object sender, DrawItemEventArgs e)
        {
            
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(tbIPAdress.Text), int.Parse(tbPort.Text));
            if (client.ConnectToServer(endPoint, tbName.Text))
                cbIsConnected.Checked = true;
            else
                cbIsConnected.Checked = false;
        }

        private void btDisconnect_Click(object sender, EventArgs e)
        {
            client.Disconnect();
            cbIsConnected.Checked = client.IsConnected;
        }

        private void btGetAddress_Click(object sender, EventArgs e)
        {
            client.UdpBroadcastRequest();        
        }

        private void lbParticipants_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbParticipants.SelectedIndex != -1)
            {
                selectedIndex = lbParticipants.SelectedIndex;
                CurrentDialogId = clientsList[selectedIndex].Key;
                chatDialogsInfo[CurrentDialogId].UnreadMessageCounter = 0;
                UpdateView();
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.CloseAllThreads();
        }

        private void btGetHistory_Click(object sender, EventArgs e)
        {
            lbParticipants.SelectedIndex = 0;
            Chat.Message message = new Chat.Message() {messageType = MessageType.History, ReceiverID = clientsList[selectedIndex].Key};
            client.SendMessage(message);
            UpdateView();
        }

        byte[] getFileContent(string fileName)
        {
            FileStream fin;
            try
            {
                fin = new FileStream(fileName, FileMode.Open);
            }
            catch
            {
                return null;
            }
            int length = (int)fin.Length;
            byte[] buffer = new byte[length];
            try
            {
                fin.Read(buffer, 0, length);
            }
            catch
            {
                buffer = null;
            }
            finally
            {
                fin.Close();
            }
            return buffer;
        }

        async Task<int> LoadFileContent(string fileName, long MaxSize)
        {
            int result;
            IsLoadingFiles = true;
            byte[] bufferFileContent = getFileContent(fileName);
            long length = bufferFileContent.Length;
            long totalSize = FileTotalSizeCounter + length;
            if (length > MaxSize)
            {
                MessageBox.Show("The file size is " + length + ", but it should be less then"
                  + MaxFileSize.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = -1;
            }
            else if (totalSize > MaxFileTotalSize)
            {
                MessageBox.Show("The tota files size size is " + totalSize + ", but it should be less then"
                  + MaxFileSize.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = -1;
            }
            else
            {
                FileTotalSizeCounter = totalSize;
                HttpContent fileContent = new ByteArrayContent(bufferFileContent);
                int tryCounter = 0;
                string fileExtension = Path.GetExtension(fileName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                result = -1;
                while (result == -1)
                {
                    result = await clientHttp.PostResource("http://localhost:" + httpPort.ToString()
                        + "/" + fileName, fileContent);
                    fileName = fileNameWithoutExtension + "(" + tryCounter.ToString() + ")" + fileExtension;
                    tryCounter++;
                }
            }
            IsLoadingFiles = false;
            return result;
        }

        bool isAcceptableExtension(string fileName)
        {
            string extesnion = Path.GetExtension(fileName);
            return !unacceptableExtension.Contains(extesnion);
        }

        private async void btAdd_Click(object sender, EventArgs e)
        {
            if (LoadFile.ShowDialog() == DialogResult.OK)
            {
                string fileName = LoadFile.FileName;
                if(!isAcceptableExtension(fileName))
                {
                    MessageBox.Show("The downloaded file can not be executable", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }   
                int result;
                try
                {
                    Task<int> t = LoadFileContent(fileName, MaxFileSize);
                    result = await t;
                    if(result == -1)
                    {
                        MessageBox.Show("File upload failed", "Error", MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("The file server is unavaible now", "Bad news", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = -1;
                }
                if (result != - 1)
                {
                    loadedFiles.Add(result, Path.GetFileName(fileName));
                    UpdateView();
                }
            }
        }

        bool CheckMessageForFiles(int index)
        {
            bool result = false;
            Chat.Message message = chatDialogsInfo[CurrentDialogId].Messages[index];
            if (message.FilesID != null && message.FilesID.Count > 0)
                result = true;
            return result;
        }

        private void lbChatContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lbChatContent.SelectedIndex != - 1 && CheckMessageForFiles(lbChatContent.SelectedIndex))
            {
                new frmShowFiles(chatDialogsInfo[CurrentDialogId].Messages[lbChatContent.SelectedIndex].
                    FilesID, httpPort).ShowDialog();
            }
        }

        private void btShowFiles_Click(object sender, EventArgs e)
        {
            new frmShowFiles(chatDialogsInfo[CurrentDialogId].Messages[selectedIndex].
                FilesID, httpPort).ShowDialog();
        }

        private void lbChatContent_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)e.Graphics.MeasureString(((ListBox)sender).Items[e.Index].ToString(), ((ListBox)sender).Font).Height;
        }

        private void lbChatContent_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (((ListBox)sender).Items.Count > 0)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font,
                    new SolidBrush(e.ForeColor), e.Bounds);
            }
        }

        private async void btDelete_Click(object sender, EventArgs e)
        {
            if(lbFiles.SelectedIndex != -1)
            {
                int fileID = ((KeyValuePair<int, string>)lbFiles.SelectedItem).Key;
                string fileName = ((KeyValuePair<int, string>)lbFiles.SelectedItem).Value;
                try
                {
                    await clientHttp.DeleteResource("http://localhost:" + 
                        httpPort.ToString() + "/" + fileID.ToString());
                    MessageBox.Show("The file " + fileName + " has been deleted", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadedFiles.Remove(((KeyValuePair<int, string>)lbFiles.SelectedItem).Key);
                    UpdateView();

                }
                catch(FileNotFoundException)
                {
                    MessageBox.Show("The file is not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
    }
}
