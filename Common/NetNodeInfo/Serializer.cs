using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Chat
{
    public class Serializer: ISerializer
    {
        public byte[] Serialize(Message message)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            MemoryStream messageContainer = new MemoryStream();
            serializer.Serialize(messageContainer, message);
            return messageContainer.GetBuffer();
        }

        public Message Deserialize(byte[] data, int amount)
        {
            MemoryStream messageContainer = new MemoryStream();
            messageContainer.Write(data, 0, amount);
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            messageContainer.Position = 0;
            Message message = (Message)serializer.Deserialize(messageContainer);
            return message;
        }
    }
}
