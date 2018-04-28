using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZlPos.Utils
{
    public class WeightUtil
    {
        private static WeightUtil _WeightUtil = null;
        private static object obj = new object();
        internal Action<string> Listener;
        private SerialPort mSerialPort;

        private WeightUtil() { }

        public static WeightUtil Instance
        {
            get
            {
                lock (obj)
                {
                    if (_WeightUtil == null)
                    {
                        _WeightUtil = new WeightUtil();
                    }
                    return _WeightUtil;
                }
            }
        }

        string sBuffer = "asdfsdfa";
        internal void Open(string port)
        {
            if (mSerialPort == null)
            {
                try
                {
                    mSerialPort = new SerialPort(port, 9600);
                    mSerialPort.Open();
                    Task.Factory.StartNew(() =>
                    {
                        int size;
                        byte[] buffer = new byte[64];
                        while (mSerialPort.IsOpen)
                        {
                            Thread.Sleep(40);
                            //size = mSerialPort.Read(buffer, 0, 1);
                            string s = mSerialPort.ReadLine();
                            if (!sBuffer.Equals(s))
                            {
                                
                                sBuffer = s;
                                Listener?.Invoke(s.Replace("\r","").Replace("\n",""));
                            }
                            
                            //if (size > 0)
                            //{
                            //    lock (obj)
                            //    {
                            //        onDataReceived(buffer, size);
                            //    }
                            //}
                        }
                    });
                }
                catch (Exception e)
                {
                    //LOG
                }
            }
        }

        private void onDataReceived(byte[] buffer, int size)
        {
            if(buffer[0] == 32)
            {
                return;
            }
            string s64 = Encoding.Default.GetString(buffer);
            string[] data = s64.Replace("\n\r", "@").Split('@');
            Listener?.Invoke(data[1]);

            



        }

        internal void Close()
        {
            if (mSerialPort != null)
            {
                mSerialPort.Close();
                mSerialPort = null;
                GC.Collect();
            }
        }
    }
}
