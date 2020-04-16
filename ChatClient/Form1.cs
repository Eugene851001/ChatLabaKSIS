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
        const long MaxFileSize = long.MaxValue;

        ClientHttp.Client clientHttp;
        Client client;
        Dictionary<int, ChatDialogInfo> chatDialogsInfo;
        List<EndPointNamePair> clientsList;
        List<string> fileNames;
        int CurrentDialogId = chatDialogId;
        int selectedIndex = 0;

      
        bool IsLoadingFiles = false;
        List<string> unacceptableExtension = new List<string>() { ".exe", ".dll" };

        public frmMain()
        {
            InitializeComponent();
            clientsList = new List<EndPointNamePair>();
            clientsList.Add(new EndPointNamePair() { Key = chatDialogId, Value = "Chat" });
            fileNames = new List<string>();
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
            if (message.FileNames != null)
            {
                foreach (var fileName in message.FileNames)
                {
                    visualMessage += "\n[FILE] " + fileName + "\n";
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
                    // string[] messages = new string[chatDialogsInfo[CurrentDialogId].Messages.Count];
                    //chatDialogsInfo[CurrentDialogId].Messages.CopyTo(messages);
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
                foreach(string fileName in fileNames)
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
                    message.FileNames = new List<string>(fileNames);
                }
                else
                {
                    message = new Chat.Message(clientsList[selectedIndex].Key, tbMessageContent.Text);
                    message.FileNames = new List<string>(fileNames);
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
                        UpdateView();
                    }
                }
            }
        }

     //   void lst_MeasureItem(object sender, MeasureItemEventArgs e)
       /// {
          //  e.ItemHeight = (int)e.Graphics.MeasureString(listBox1.Items[e.Index].ToString(), listBox1.Font, listBox1.Width).Height;
       // }

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

        async Task<bool> LoadFileContent(string fileName, long MaxSize)
        {
            bool result = true;
            IsLoadingFiles = true;
            HttpContent fileContent = clientHttp.LoadFile(fileName);
            byte[] buffer = await fileContent.ReadAsByteArrayAsync();
            if (buffer.Length <= MaxSize)
            {
                await clientHttp.PostResource("http://localhost:" + httpPort.ToString(), clientHttp.LoadFile(fileName));
            }
            else
            {
                MessageBox.Show("The file size is " + buffer.Length + ", but should be less then" 
                    + MaxFileSize.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = false;
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
                    MessageBox.Show("The downloaded file cannot be executable", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }   
                bool result = true;
                try
                {
                    Task<bool> t = LoadFileContent(fileName, MaxFileSize);
                    result = await t;
                }
                catch
                {
                    MessageBox.Show("The file server is unavaible now", "Bad news", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = false;
                }
                if (result)
                {
                    fileNames.Add(Path.GetFileName(fileName));
                    UpdateView();
                }
            }
        }

        bool CheckMessageForFiles(int index)
        {
            bool result = false;
            Chat.Message message = chatDialogsInfo[CurrentDialogId].Messages[index];
            if (message.FileNames != null && message.FileNames.Count > 0)
                result = true;
            return result;
        }
        private void lbChatContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lbChatContent.SelectedIndex != - 1 && CheckMessageForFiles(lbChatContent.SelectedIndex))
            {
                new frmShowFiles(chatDialogsInfo[CurrentDialogId].Messages[lbChatContent.SelectedIndex].
                    FileNames, httpPort).ShowDialog();
            }
        }

        private void btShowFiles_Click(object sender, EventArgs e)
        {
            new frmShowFiles(chatDialogsInfo[CurrentDialogId].Messages[selectedIndex].
                FileNames, httpPort).ShowDialog();
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

        private void btRemove_Click(object sender, EventArgs e)
        {
            if(lbFiles.SelectedIndex != -1)
            {
                fileNames.RemoveAt(lbFiles.SelectedIndex);
                UpdateView();
            }
        }

        private async void btDelete_Click(object sender, EventArgs e)
        {
            if(lbFiles.SelectedIndex != -1)
            {
                string fileName = (string)lbFiles.SelectedItem;
                try
                {
                    await clientHttp.DeleteResource("http://localhost:" + httpPort.ToString() + "/" + fileName);
                    MessageBox.Show("The file " + fileName + " has been deleted", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(FileNotFoundException)
                {
                    MessageBox.Show("The file is not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
    }
}
