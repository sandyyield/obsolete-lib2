using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ZlPos.Bizlogic
{
    class UpdateBiz
    {
        public static void SoftUpdate(Form frmMain, string currentVersion)
        {
            string strAppFilePath = Application.StartupPath + "\\SoftUpdate\\SilentUpdate.exe";

            string version = currentVersion;
            if (!string.IsNullOrEmpty(version))
            {
                Process proc = null;
                proc = Process.Start(strAppFilePath, "-version " + version);

                if (proc != null)
                {
                    //秩序侦听windows消息
                    while (!proc.HasExited && !frmMain.IsDisposed)
                    {
                        System.Threading.Thread.Sleep(10);
                        Application.DoEvents();
                    }
                }
            }
        }



        /// <summary>
        /// 判断网络是否正常
        /// </summary>
        /// <returns></returns>
        public bool NetWorkStatus()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000;
            PingReply reply = pingSender.Send("61.155.218.132", timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNetConnect()
        {
            try
            {
                int dwFlag = 0;
                if (InternetGetConnectedState(ref dwFlag, 0))
                {//判断联网
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("winInet.dll ")]
        //声明外部的函数： 
        public static extern bool InternetGetConnectedState(
            ref int dwFlag,
            int dwReserved
        );
    }
}
