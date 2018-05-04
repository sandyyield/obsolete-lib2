using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Enums;
using ZlPos.Utils;

namespace ZlPos.PrintServices
{
    /// <summary>
    /// 封装一下底层的库调用
    /// </summary>
    public class USBPrinter
    {
        protected static ILog logger = null;

        //记录usb设备的句柄
        private IntPtr hDevice = IntPtr.Zero;
        private PrinterType currentPrintType;

        private bool init = false;

        public IntPtr HDevice { get => hDevice; set => hDevice = value; }
        public PrinterType CurrentPrintType { get => currentPrintType; set => currentPrintType = value; }
        public bool Init { get => init; set => init = value; }

        public static readonly int COMM_ALIGN = 13;
        public static readonly int COMM_ALIGN_LEFT = 0;
        public static readonly int COMM_ALIGN_CENTER = 1;
        public static readonly int COMM_LINE_HEIGHT = 20;

        [Obsolete]
        public int PrintText(string content)
        {
            if (content == null)
            {
                return -1;
            }
            else
            {
                try
                {
                    return Write(hDevice, content);
                }
                catch (Exception e)
                {
                    logger.Error(e.StackTrace);
                    return -1;
                }
            }
        }

        public IntPtr open()
        {
            IntPtr hUsb = IntPtr.Zero;
            if (hDevice == IntPtr.Zero)
            {
                hUsb = PrintBridge.OpenUsb();
                init = true;
            }
            return hUsb;
        }

        /// <summary>
        /// ***************注意打开关闭时间间隔过小会导致bug*****************
        /// </summary>
        public void close()
        {
            PrintBridge.CloseUsb(hDevice);
            init = false;
            hDevice = IntPtr.Zero;
        }

        /// <summary>
        /// 转换编码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal int PrintString(string content)
        {
            try
            {
                int sendCount = 0;
                string sendUnicode = Encoding.Unicode.GetString(Encoding.Default.GetBytes(content));
                PrintBridge.WriteUsb(hDevice, sendUnicode, Encoding.Unicode.GetByteCount(sendUnicode), ref sendCount);
                //PrintBridge.WriteUsb(hDevice, content, Encoding.Default.GetByteCount(content), ref sendCount);
                return sendCount;
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
                return -1;
            }
        }

        public static int Write(IntPtr hDevice, string content)
        {
            try
            {
                int sendCount = 0;
                PrintBridge.WriteUsb(hDevice, content, Encoding.Default.GetByteCount(content), ref sendCount);
                return sendCount;
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
                return -1;
            }
        }

        public void initUSB()
        {
            if (!init)
            {
                open();
            }
        }

        internal void openCash()
        {
            int sendCount = 0;
            string str = "1B70008888";
            byte[] strByte = HexUtils.HexStringToByte(str);
            string sendUnicode = Encoding.Unicode.GetString(strByte);
            PrintBridge.WriteUsb(hDevice, sendUnicode, Encoding.Unicode.GetByteCount(sendUnicode), ref sendCount);
        }
    }
}
