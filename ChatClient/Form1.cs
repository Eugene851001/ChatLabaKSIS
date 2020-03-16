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

namespace ChatClient
{
    public partial class frmMain : Form
    {
        const int chatDialogId = 0;

        Client client;
        Dictionary<int, ChatDialogInfo> chatDialogsInfo;
        List<EndPointNamePair> clientsList;
        int CurrentDialogId = chatDialogId;
        int selectedIndex = 0;
        public frmMain()
        {
            InitializeComponent();
            clientsList = new List<EndPointNamePair>();
            clientsList.Add(new EndPointNamePair() { Key = chatDialogId, Value = "Chat" });
            chatDialogsInfo = new Dictionary<int, ChatDialogInfo>();
            chatDialogsInfo.Add(chatDialogId, new ChatDialogInfo("Chat"));
            client = new Client();
            client.ReceiveMessageHandler += ShowReceivedMessages;
        }

        public void ShowReceivedMessages(Chat.Message message)
        {
            switch(message.messageType)
            {
                case MessageType.Regular:
                    chatDialogsInfo[chatDialogId].AddMessage(message.Name + ": " 
                        + message.Content + "\n" + message.Time + "\n" + message.IPAdress);
                    break;
                case MessageType.Private:
                    chatDialogsInfo[message.SenderID].AddMessage( message.Content + "\n" + 
                        message.Time + "\n" + message.IPAdress);
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
            }
            UpdateView();
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
                tbChatContent.Clear();
                if (chatDialogsInfo != null)
                {
                    string[] messages = new string[chatDialogsInfo[CurrentDialogId].Messages.Count];
                    chatDialogsInfo[CurrentDialogId].Messages.CopyTo(messages);
                    foreach (string messageContent in messages)
                    {
                        tbChatContent.Text += messageContent + "\r\n";
                    }
                }
                lbParticipants.Items.Clear();
                foreach (EndPointNamePair clientName in clientsList)
                {
                    lbParticipants.Items.Add(clientName.Value + " " + ((chatDialogsInfo[clientName.Key].UnreadMessageCounter != 0) ?
                        chatDialogsInfo[clientName.Key].UnreadMessageCounter.ToString() : ""));
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
                }
                else
                {
                    message = new Chat.Message(clientsList[selectedIndex].Key, tbMessageContent.Text);
                    chatDialogsInfo[message.ReceiverID].Messages.Add("Me: " + message.Content);
                }
                if (cbIsConnected.Checked)
                {
                    client.SendMessage(message);
                    UpdateView();
                }
            }
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
            Chat.Message message = new Chat.Message(new List<string>(), clientsList[selectedIndex].Key);
            client.SendMessage(message);
            UpdateView();
        }
    }
}
