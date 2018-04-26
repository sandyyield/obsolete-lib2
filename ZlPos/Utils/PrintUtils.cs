using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    public class PrintUtils
    {
        public static void printModel(string content,USBPrinter usbPrinter)
        {
            usbPrinter.PrintString(content);
        }

        public static void printModel(string content, BluetoothPrinter bluetoothPrinter)
        {
            bluetoothPrinter.PrintString(content);
        }

        public static void printModel(string content, serialPort portPrinter)
        {
            portPrinter.Write(content);
        }
    }
}
