using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ZlPos.Bizlogic
{
    public class InternetHelper
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);

        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool IsConnectInternet()
        {
            PingTxt();
            int Description = 0;
            bool isOpenInternet = InternetGetConnectedState(Description, 0);
            return isOpenInternet && PingTxt();
        }

        public static bool IsConnectInternetXP()
        {
            logger.Info("xp network");
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
            string url = "http://ls.zhonglunnet.com/ping.txt";
            //string url = "https://www.google.com/ping.txt";
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
                logger.Info("WebClient err", e);
                result = false;
            }
            return result;
        }
    }
}
