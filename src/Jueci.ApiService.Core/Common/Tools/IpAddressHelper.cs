using System.Net;

namespace Jueci.ApiService.Common.Tools
{
    public class IpAddressHelper
    {
        public static string GetLocalIpAddress()
        {
            ///获取本地的IP地址
            string addressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    addressIP = _IPAddress.ToString();
                }
            }
            return addressIP;
        }
    }
}