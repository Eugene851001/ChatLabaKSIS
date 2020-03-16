using System;
using System.Collections.Generic;
using System.Text;

namespace Chat
{
    interface ISerializer
    {
        byte[] Serialize(Message message);
        Message Deserialize(byte[] data, int amount);
    }
}
