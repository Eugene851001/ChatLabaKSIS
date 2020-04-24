using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Chat
{
    public enum MessageType { Regular, Registration, ClientsList, Private, SearchRequest,
        SearchResponse, History, CheckConnection};

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
        public string Content { get; set;}
        public string Name { get; set; }
        public MessageType messageType { get; set;}
        public List<EndPointNamePair> clientsNames { get; set;}
        public int ReceiverID { get; set; }
        public int SenderID { get; set; }
        public string IPAdress { get; set; }
        public int Port { get; set; }
        public int Hash { get; set; }
        public List<Message> MessageHistory { get; set; }
        public DateTime Time { get; set; }
        public List<int> FilesID { get; set; }

        public Message(int receiver, string content)
        {
            ReceiverID = receiver;
            Content = content;
            messageType = MessageType.Private;
        }

        public Message(List<Message> messageHistory, int receiverID)
        {
            ReceiverID = receiverID;
            MessageHistory = messageHistory;
            messageType = MessageType.History;
        }

        public Message(List<EndPointNamePair> clientsNames)
        {
            this.clientsNames = clientsNames;
            messageType = MessageType.ClientsList;
        }

        public Message()
        {
        }
    }
}