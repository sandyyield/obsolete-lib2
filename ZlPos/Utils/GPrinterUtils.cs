using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Utils
{
    class GPrinterUtils
    {
        private int mPrinterIndex = 0;
        private int usbPrintIndex = 0;
        private int enterPrinterIndex = 1;
        private static object obj = new object();
        private static GPrinterUtils gPrinterUtils;
        //佳博打印机
        //private GpService mGpService = null;
        //private PrinterServiceConnection conn = null;

        private Dictionary<String, IntPtr> deviceIndexMap = new Dictionary<string, IntPtr>();

        private const int MAIN_QUERY_PRINTER_STATUS = 0xfe;
        private const int REQUEST_PRINT_LABEL = 0xfd;
        private const int REQUEST_PRINT_RECEIPT = 0xfc;

        //private ConnectListener connectListener;

        private GPrinterUtils()
        {
            //TODO...？？？
        }

        public static GPrinterUtils Instance
        {
            get
            {
                lock (obj)
                {
                    if(gPrinterUtils == null)
                    {
                        gPrinterUtils = new GPrinterUtils();
                    }
                    return gPrinterUtils;
                }
            }
        }

        internal void printLabel(string content)
        {
            if(string.IsNullOrEmpty(content))
            {
                return;
            }
        }

        internal void printLabelTest()
        {
            throw new NotImplementedException();
        }

        internal void printUSBTest()
        {
            throw new NotImplementedException();
        }
    }
}
