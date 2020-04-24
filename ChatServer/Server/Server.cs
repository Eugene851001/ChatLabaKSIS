using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Chat;
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
        public static List<Message> MessageHistory = new List<Message>();

        static Serializer messageSerializer = new Serializer();
       public static void HandleSearchMessage(Message message)
        {
            Message messageResponse = new Message()
            {
                IPAdress = StandartInfo.GetCurrentIP().
                ToString(),
                Port = port,
                messageType = MessageType.SearchResponse
            };
           //Message messageResponse = new Message(StandartInfo.GetCurrentIP().ToString(), port, MessageType.SearchResponse);
            Socket socketSetAdress = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(message.IPAdress), message.Port);
            socketSetAdress.SendTo(messageSerializer.Serialize(messageResponse), endPoint); 
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
                Message message = messageSerializer.Deserialize(data, amount);
                if (message.messageType == MessageType.SearchRequest)
                    HandleSearchMessage(message);
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
            socketClient.Send(messageSerializer.Serialize(message));
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