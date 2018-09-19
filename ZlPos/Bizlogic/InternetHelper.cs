using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace ZlPos.Bizlogic
{
    public class InternetHelper
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);


        public static bool IsConnectInternet()
        {
            int Description = 0;
            return InternetGetConnectedState(Description, 0);
        }

        public static bool IsConnectInternetXP()
        {
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send("180.106.148.201");//Send("119.75.217.109");
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else if (reply.Status == IPStatus.TimedOut)
                {
                    //kresponse.write("超时");
                    return false;
                }
                else
                {
                    //response.write("失败");
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
