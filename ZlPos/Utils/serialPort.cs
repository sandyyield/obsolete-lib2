using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ZlPos.Utils
{
    public class serialPort
    {
        public string brand { get; set; }
        public string port { get; set; }
        public string rate { get; set; }
        public string pageWidth { get; set; }

        private SerialPort m_SerialPort;

        public bool Enable { get => enable; set => enable = value; }

        private bool enable = false;


        public serialPort(String port, String rate)
        {
            this.port = port;
            this.rate = rate;
        }

        public void init()
        {
            if (enable == false)
            {
                m_SerialPort = new SerialPort();
                m_SerialPort.BaudRate = int.Parse(rate);
                m_SerialPort.PortName = port;
                Enable = true;
            }
        }

        public void Close()
        {
            if (m_SerialPort != null)
            {
                m_SerialPort.Close();
            }
        }

        public void Write(string txt)
        {
            if (m_SerialPort.IsOpen)
            {
                byte[] strByte = Encoding.Default.GetBytes(txt);
                m_SerialPort.Write(strByte,0,strByte.Length);
            }
        }

        public bool Open(string port, int rate)
        {
            try
            {
                if (Enable)
                {
                    m_SerialPort.Open();
                    return true;
                }
            }
            catch (Exception e)
            {
            }
            return false;
        }

        //private static Dictionary<string, SerialReader> srs = new Dictionary<String, SerialReader>();

        //public void init()
        //{
        //    //		exeShell("chmod 666 /dev");
        //    //		exeShell("chmod 666 /dev/ttyUSB1");
        //}

        //private SerialReader getSerialReader()
        //{
        //    SerialReader sr = srs[this.port];
        //    if (sr == null)
        //    {
        //        sr = new SerialReader(this.port, this.rate);
        //        srs.Add(this.port, sr);
        //    }
        //    return sr;
        //}



        //public static int CMD = 0;
        //public int STRING = 1;


        //public bool Enable()
        //{
        //    return getSerialReader().Enable(this.rate);
        //}

        //public void Close()
        //{
        //    getSerialReader().close();
        //}

        //public int Write(String str, int flag)
        //{
        //    int res = 0;
        //    getSerialReader().openSerialPort(str, String.valueOf(flag));
        //    return res;
        //}

        //public int Write(String str)
        //{
        //    int res = 0;
        //    getSerialReader().openSerialPort(str, rate);
        //    return res;
        //}

        //public String getCardNum()
        //{
        //    byte[] buffer = new byte[16];
        //    RecvByteUart(fd, buffer, 16);
        //    return MyString.bytesToHexString(buffer).trim();
        //}

        ///**
        // * 认证，在读取卡号完成后，完成认证操作
        // * 
        // * @return 对扇区进行认证，如果密钥匹配，则返回值为 00
        // */
        //public String authentication()
        //{
        //    Write("400800080000ffffffffffff00000D", CMD);
        //    byte[] buffer = new byte[16];
        //    RecvByteUart(fd, buffer, 16);
        //    return MyString.bytesToHexString(buffer).substring(10, 14);
        //}

        ///**
        // * 接收串口数据
        // * 
        // * @param hSerial
        // *            OpenUart成功时，返回的设备句柄
        // * @param buffer
        // *            接收缓冲区
        // * @param size
        // *            接收缓冲区长度(请保证缓冲区大小足够大)
        // * @return 大于0，表示成功，否则失败
        // */
        //public int RecvByteUart(int hSerial, byte[] buffer, int size)
        //{

        //    String res = getSerialReader().receiveSerialPortByte(hSerial, size);
        //    if (res.isEmpty() || res.contains("faield"))
        //    {
        //        return -1;
        //    }
        //    byte[] byteTmp = MyString.hexStringToBytes(res);
        //    for (int i = 0; i < res.length() / 2; i++)
        //        buffer[i] = byteTmp[i];

        //    return res.length() / 2;
        //}
    }
}
