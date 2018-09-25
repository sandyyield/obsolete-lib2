using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZlPos.Models;

namespace ZlPos.Utils
{
    class BJQPrinterManager
    {
        private static BJQPrinterManager _BJQPrinterManager;

        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //private object obj = new object();

        public bool Init { get; set; }
        public string PrinterTypeEnum { get; set; }
        public int PrintNumber { get; internal set; }


        public PrinterConfigEntity printerConfigEntity { get; set; }

        //internal List<string> usbDeviceArrayList;

        private BJQPrinterManager() { }

        public static BJQPrinterManager Instance
        {
            get
            {
                if (_BJQPrinterManager == null)
                {
                    _BJQPrinterManager = new BJQPrinterManager();
                }
                return _BJQPrinterManager;
            }
        }



    }
}
