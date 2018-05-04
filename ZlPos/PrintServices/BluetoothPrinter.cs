using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using ZlPos.Enums;

namespace ZlPos.PrintServices
{
    public class BluetoothPrinter
    {
        private BluetoothDeviceInfo bluetoothDevice;
        private BluetoothAddress bluetoothAddress;
        private bool init;

        public BluetoothPrinter(BluetoothDeviceInfo bluetoothDevice)
        {
            this.bluetoothDevice = bluetoothDevice;
        }

        public object MacAddress { get; internal set; }
        public string Encoding { get; set; }
        public PrinterType CurrentPrintType { get; set; }

        internal void closeConnection()
        {
            Blueclient.Close();
        }

        internal bool isConnected()
        {
            return init;
        }

        //这个全局变量暂不确定是否有用
        BluetoothClient Blueclient = new BluetoothClient();
        internal void openConnection()
        {
            try
            {
                //连接蓝牙
                bluetoothAddress = bluetoothDevice.DeviceAddress;

                //BluetoothRadio BuleRadio = BluetoothRadio.PrimaryRadio;
                //BuleRadio.Mode = RadioMode.Connectable;
                //BluetoothDeviceInfo[] Devices = Blueclient.DiscoverDevices();

                Blueclient.Connect(bluetoothAddress, BluetoothService.SerialPort);

                //配对成功

                init = true;
            }
            catch (Exception e)
            {

                init = false;
            }
        }

        /// <summary>
        /// 打印文本
        /// </summary>
        /// <param name="mess"></param>
        /// <returns> [>0] :成功返回sendsize; [0]:send fail ; [-1]:蓝牙未连接 ;</returns>
        public int PrintString(string mess)
        {
            int err = 0;
            if (Blueclient.Connected)
            {
                byte[] OutBuffer;//数据
                int BufferSize;
                Encoding targetEncoding;
                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = System.Text.Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(mess); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(mess);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码
                int res = Blueclient.Client.Send(OutBuffer);

                if (res == BufferSize)
                {
                    err = res;
                }
                else
                {
                    err = 0;
                }
            }
            else
            {
                err = -1;

            }
            return err;
        }

        internal void openCash()
        {
            throw new NotImplementedException();
        }
    }
}
