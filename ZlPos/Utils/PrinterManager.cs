using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Enums;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    public class PrinterManager
    {
        private static PrinterManager printerManager;

        private PrinterTypeEnum printerTypeEnum;
        //add 2018/01/15
        private int printNumber;//打印小票份数

        private USBPrinter usbPrinter;
        private BluetoothPrinter bluetoothPrinter;
        private serialPort portPrinter;

        private PrinterConfigEntity printerConfigEntity;

        //private ArrayList<UsbDevice> usbDeviceArrayList;
        //private ArrayList<BluetoothDevice> bluetoothDeviceArrayList;


        private bool init;

        private PrinterManager()
        {
        }

        public static PrinterManager Instance
        {
            get
            {
                if(printerManager == null)
                {
                    lock(typeof(PrinterManager))
                    {
                        if (printerManager == null)
                        {
                            printerManager = new PrinterManager();
                        }
                    }
                }
                return printerManager;
            }
        }

        public PrinterTypeEnum PrinterTypeEnum { get => printerTypeEnum; set => printerTypeEnum = value; }
        public int PrintNumber { get => printNumber; set => printNumber = value; }
        public USBPrinter UsbPrinter { get => usbPrinter; set => usbPrinter = value; }
        public PrinterConfigEntity PrinterConfigEntity { get => printerConfigEntity; set => printerConfigEntity = value; }
        public bool Init { get; internal set; }
        internal BluetoothPrinter BluetoothPrinter { get => bluetoothPrinter; set => bluetoothPrinter = value; }
        internal serialPort PortPrinter { get => portPrinter; set => portPrinter = value; }
    }
}
