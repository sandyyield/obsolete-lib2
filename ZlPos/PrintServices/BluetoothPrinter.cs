using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net.Sockets;
using ZlPos.Enums;

namespace ZlPos.PrintServices
{
    public class BluetoothPrinter
    {
        private BluetoothDeviceInfo bluetoothDevice;

        public BluetoothPrinter(BluetoothDeviceInfo bluetoothDevice)
        {
            //this.bluetoothDevice = bluetoothDevice;
            //TODO...
        }

        public object MacAddress { get; internal set; }
        public string Encoding { get; internal set; }
        public PrinterType CurrentPrintType { get; set; }

        internal void closeConnection()
        {
            throw new NotImplementedException();
        }

        internal bool isConnected()
        {
            throw new NotImplementedException();
        }

        internal void openConnection()
        {
            throw new NotImplementedException();
        }
    }
}
