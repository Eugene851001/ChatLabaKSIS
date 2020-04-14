using System.Collections.Generic;

namespace Chat
{
    class ChatDialogInfo
    {
        public List<Message> Messages;
        public int UnreadMessageCounter;
        public string Name;

        public ChatDialogInfo(string name)
        {
            Name = name;
            Messages = new List<Message>();
            UnreadMessageCounter = 0;
        }
        public void AddMessage(Message message)
        {
            Messages.Add(message);
            UnreadMessageCounter++;
        }
    }
}
