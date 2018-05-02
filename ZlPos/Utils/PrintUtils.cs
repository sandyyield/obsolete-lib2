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
        public static void printModel(string content, USBPrinter usbPrinter)
        {
            if (string.IsNullOrEmpty(content) && usbPrinter == null)
            {
                return;
            }
            List<PrintEntity> printEntities = JsonConvert.DeserializeObject<List<PrintEntity>>(content);
            if (printEntities != null && printEntities.Count > 0)
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
            if (string.IsNullOrEmpty(content) && bluetoothPrinter == null)
            {
                return;
            }
            List<PrintEntity> printEntities = JsonConvert.DeserializeObject<List<PrintEntity>>(content);
            if (printEntities != null && printEntities.Count > 0)
            {
                if (!bluetoothPrinter.isConnected())
                {
                    bluetoothPrinter.openConnection();
                }

                for (int i = 0; i < printEntities.Count; i++)
                {
                    bluetoothPrinter.PrintString(printEntities[i].content);
                }
            }
            bluetoothPrinter.PrintString("\n\n\n\n\n\n");
            //bluetoothPrinter.PrintString(content);
        }

        public static void printModel(string content, serialPort portPrinter)
        {
            if (string.IsNullOrEmpty(content) && portPrinter == null)
            {
                return;
            }
            List<PrintEntity> printEntities = JsonConvert.DeserializeObject<List<PrintEntity>>(content);

            if (printEntities != null && printEntities.Count > 0)
            {
                //portPrinter.initUSB();
                for (int i = 0; i < printEntities.Count; i++)
                {
                    portPrinter.Write(printEntities[i].content);
                }
            }
            portPrinter.Write("\n\n\n\n\n\n");
            //portPrinter.Write(content);
        }
    }
}
