using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Chat;
using System.Xml.Serialization;
using System.Collections;
using System.Threading;
using NetNodeInfo;

namespace Server
{
    static class Server
    {
        public const int chatDialogId = 0;
        const int port = 8005;
        const string address = "127.0.0.1";
        const int MaxUsersAmount = 10;

        public static Dictionary<int, Socket> clients = new Dictionary<int, Socket>();
        public static Dictionary<int, string> clientNames = new Dictionary<int, string>();
        public static List<string> MessageHistory = new List<string>();

       public static void HandleSearchMessage(Message message, Socket socketListener)
        {
            Message messageResponse = new Message(StandartInfo.GetCurrentIP().ToString(), port, MessageType.SearchResponse);
            Socket socketSetAdress = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(message.IPAdress), message.Port);
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            MemoryStream messageContainer = new MemoryStream();
            serializer.Serialize(messageContainer, messageResponse);
            byte[] data = messageContainer.GetBuffer();
            socketSetAdress.SendTo(data, endPoint); 
       }
        public static void ListenUdpBroadcast()
        {
            Socket socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socketListener.Bind(localEndPoint);
            byte[] data = new byte[1024];
            EndPoint endPoint = localEndPoint;
            Console.WriteLine("The server avaible (UDP)");
            while (true)
            {
                int amount = socketListener.ReceiveFrom(data, ref endPoint);
                int testPort = ((IPEndPoint)(socketListener.LocalEndPoint)).Port;
                MemoryStream messageContainer = new MemoryStream();
                messageContainer.Write(data, 0, amount);
                XmlSerializer serializer = new XmlSerializer(typeof(Message));
                messageContainer.Position = 0;
                Message message = (Message)serializer.Deserialize(messageContainer);
                if (message.messageType == MessageType.SearchRequest)
                    HandleSearchMessage(message, socketListener);
            }
        }

        public static void ListenTcp()
        {
            Socket socketListener;
            IPEndPoint endPoint = new IPEndPoint(StandartInfo.GetCurrentIP(), port);
            socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketListener.Bind(endPoint);
            socketListener.Listen(MaxUsersAmount);
            Console.WriteLine("The server is avaible (TCP)");
            while (true)
            {
                Socket socketClientHandler = socketListener.Accept();
                ConnectionHandler connection = new ConnectionHandler(socketClientHandler);
            }
        }

        public static void StartListen()
        {
            Thread handleTcp = new Thread(ListenTcp);
            Thread handleUdp = new Thread(ListenUdpBroadcast);
            handleTcp.Start();
            handleUdp.Start();
        }

        public static void SendMessage(Message message, Socket socketClient)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            MemoryStream messageContainer = new MemoryStream();
            serializer.Serialize(messageContainer, message);
            byte[] buffer = messageContainer.GetBuffer();
            socketClient.Send(buffer);
        }

        public static void RemoveClient(int key)
        {
            clientNames.Remove(key);
            clients.Remove(key);
        }
        public static void SendToAll(Message message)
        {
            foreach (Socket socket in clients.Values)
            {
                SendMessage(message, socket);
            }
        }
    }
}