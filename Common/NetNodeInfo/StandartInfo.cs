using System;
using System.Net;

namespace NetNodeInfo
{
    public static class StandartInfo
    {
        public static IPAddress GetCurrentIP()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress currentIPAdress = null;
            bool IsFound = false;
            foreach (var adress in adresses)
            {
                if (adress.GetAddressBytes().Length == 4 && !IsFound)
                {
                    currentIPAdress = adress;
                    IsFound = true;
                }
            }
            return currentIPAdress;
        }
    }
}
