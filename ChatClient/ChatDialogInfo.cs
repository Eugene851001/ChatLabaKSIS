using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    class ChatDialogInfo
    {
        public List<string> Messages;
        public int UnreadMessageCounter;
        public string Name;

        public ChatDialogInfo(string name)
        {
            Name = name;
            Messages = new List<string>();
            UnreadMessageCounter = 0;
        }
        public void AddMessage(string messageContent)
        {
            Messages.Add(messageContent);
            UnreadMessageCounter++;
        }
    }
}
