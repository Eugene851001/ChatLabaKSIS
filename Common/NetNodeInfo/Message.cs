using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Chat
{
    public enum MessageType { Regular, Registration, ClientsList, Private, SearchRequest, SearchResponse, History, CheckConnection};

    public struct EndPointNamePair
    {
        public int Key;
        public string Value;

        public override string ToString()
        {
            return Value;
        }
    }

    public class Message
    {
        public string Content;
        public string Name;
        public MessageType messageType;
        public List<EndPointNamePair> clientsNames;
        public int ReceiverID;
        public int SenderID;
        public string IPAdress;
        public int Port;
        public int Hash;
        public List<string> MessageHistory;
        public DateTime Time;
        public List<string> FileNames;

        public Message(int receiver, string content)
        {
            ReceiverID = receiver;
            Content = content;
            messageType = MessageType.Private;
        }

        public Message(List<string> messageHistory, int receiverID)
        {
            ReceiverID = receiverID;
            MessageHistory = messageHistory;
            messageType = MessageType.History;
        }
        public Message(MessageType messageType)
        {
            this.messageType = messageType;
        }

        public Message(List<EndPointNamePair> clientsNames)
        {
            this.clientsNames = clientsNames;
            messageType = MessageType.ClientsList;
        }

        public Message(string IPAdress, int port, MessageType messageType)
        {
            this.messageType = messageType;
            this.Port = port;
            this.IPAdress = IPAdress;
        }

        public Message(string content)
        {
            messageType = MessageType.Regular;
            Content = content;
            Name = "Unknown";
        }

        public Message()
        {
        }
    }
}