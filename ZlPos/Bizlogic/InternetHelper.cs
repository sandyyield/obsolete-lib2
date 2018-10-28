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
            PingTxt();
            int Description = 0;
            bool isOpenInternet = InternetGetConnectedState(Description, 0);
            return isOpenInternet || PingTxt();
        }

        public static bool IsConnectInternetXP()
        {
            return PingTxt();
            //try
            //{
            //    Ping pingSender = new Ping();
            //    PingReply reply = pingSender.Send("180.106.148.201");//Send("119.75.217.109");
            //    if (reply.Status == IPStatus.Success)
            //    {
            //        return true;
            //    }
            //    else if (reply.Status == IPStatus.TimedOut)
            //    {
            //        //kresponse.write("超时");
            //        return false;
            //    }
            //    else
            //    {
            //        //response.write("失败");
            //        return false;
            //    }

            //}
            //catch (Exception e)
            //{
            //    return false;
            //}
        }

        public static bool PingTxt()
        {
            string url = "https://ls.zhonglunnet.com/ping.txt";
            bool result = false;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Credentials = CredentialCache.DefaultCredentials;
                    byte[] pageData = webClient.DownloadData(url);
                    if(pageData[0] == 0x6f && pageData[1] == 0x6b)
                    {
                        result = true; 
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch(Exception e)
            {
                result = false;
            }
            return result;
        }
    }
}
