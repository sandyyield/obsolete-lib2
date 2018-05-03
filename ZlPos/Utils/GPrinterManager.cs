using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    class GPrinterManager
    {
        private static GPrinterManager _GPrinterManager;
        //private object obj = new object();

        public bool Init { get; set; }
        public string PrinterTypeEnum { get; set; }
        public int PrintNumber { get; internal set; }


        public PrinterConfigEntity printerConfigEntity { get; set; }

        private USBPrinter usbPrinter;
        private BluetoothPrinter bluetoothPrinter;
        private serialPort portPrinter;



        private GPrinterManager() { }

        public static GPrinterManager Instance
        {
            get
            {
                if (_GPrinterManager == null)
                {
                    _GPrinterManager = new GPrinterManager();
                }
                return _GPrinterManager;
            }
        }

        public USBPrinter UsbPrinter { get => usbPrinter; set => usbPrinter = value; }
        public BluetoothPrinter BluetoothPrinter { get => bluetoothPrinter; set => bluetoothPrinter = value; }
        public serialPort PortPrinter { get => portPrinter; set => portPrinter = value; }

       

    }
}
