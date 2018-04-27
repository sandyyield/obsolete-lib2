using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    public class PrintUtils
    {
        public static void printModel(string content,USBPrinter usbPrinter)
        {
            if(string.IsNullOrEmpty(content) && usbPrinter == null)
            {
                return;
            }
            List<PrintEntity> printEntities = (List<PrintEntity>)JsonConvert.DeserializeObject(content);
            if(printEntities != null && printEntities.Count > 0)
            {
                usbPrinter.initUSB();
                for (int i = 0; i < printEntities.Count; i++)
                {
                    usbPrinter.PrintString(printEntities[i].content);
                }
            }
            usbPrinter.PrintString("\n\n\n\n\n\n");
            //usbPrinter.PrintString(content);
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
