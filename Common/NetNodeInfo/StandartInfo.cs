using System;
using System.Net;
using System.Net.NetworkInformation;

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

        public static IPAddress GetIPv4Mask()
        {
            IPAddress result = null;
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach(NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterPropeties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection unicast = adapterPropeties.UnicastAddresses;
                result = unicast[0].IPv4Mask;
            }
            return result;
        }
    }
}
