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
                        while (true)
                        {
                            //TODO...
                        }
                    });
                }
                catch (Exception e)
                {
                    //LOG
                }
            }
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }
    }
}
