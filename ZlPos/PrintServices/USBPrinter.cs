using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Enums;

namespace ZlPos.PrintServices
{
    /// <summary>
    /// 封装一下底层的库调用
    /// </summary>
    public class USBPrinter
    {
        protected static ILog logger = null;

        private IntPtr hDevice;
        private PrinterType currentPrintType;

        public IntPtr HDevice { get => hDevice; set => hDevice = value; }
        public PrinterType CurrentPrintType { get => currentPrintType; set => currentPrintType = value; }

        public static readonly int COMM_ALIGN = 13;
        public static readonly int COMM_ALIGN_LEFT = 0;
        public static readonly int COMM_ALIGN_CENTER = 1;
        public static readonly int COMM_LINE_HEIGHT = 20;

        public int PrintText(string content)
        {
            if(content == null)
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

        public static int Write(IntPtr hDevice, string content)
        {
            try
            {
                int sendCount = 0;
                PrintBridge.WriteUsb(hDevice, content, Encoding.Unicode.GetByteCount(content), ref sendCount);
                return sendCount;
            }
            catch(Exception e)
            {
                logger.Error(e.StackTrace);
                return -1;
            }
        }
    }
}
