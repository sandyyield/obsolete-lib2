using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
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

        private log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        public string sBuffer = "";
        public string sscache = "";
        internal void Open(string port, string brand)
        {
            logger.Info("open weight port =>>" + port + "---" + brand);
            //缓存置空
            sBuffer = "";
            sscache = "";
            if (mSerialPort == null)
            {
                try
                {
                    //mSerialPort = new SerialPort(port, 9600);
                    //2018年5月25日 临时写法{
                    mSerialPort = new SerialPort();
                    mSerialPort.PortName = port;
                    mSerialPort.BaudRate = 9600;
                    //}
                    logger.Info("open serialport");
                    mSerialPort.Open();
                    logger.Info("switch brand =>" + brand);
                    switch (brand)
                    {
                        case "ACS_A":
                            Task.Factory.StartNew(() =>
                            {
                                logger.Info("case ACS_A");
                                int size;
                                byte[] buffer = new byte[64];
                                while (mSerialPort.IsOpen)
                                {
                                    Thread.Sleep(40);
                                    //size = mSerialPort.Read(buffer, 0, 1);
                                    string s = mSerialPort.ReadLine();
                                    logger.Info("s(SerialPort.ReadLine ) =>" + s);
                                    if (!sBuffer.Equals(s))
                                    {
                                        sBuffer = s;
                                        Listener?.Invoke(s.Replace("\r", "").Replace("\n", ""));
                                    }
                                }
                            });
                            break;
                        case "EH100":
                            Task.Factory.StartNew(() =>
                            {
                                logger.Info("EH100");
                                int size;
                                byte[] buffer = new byte[64];
                                while (mSerialPort.IsOpen)
                                {
                                    Thread.Sleep(50);
                                    //size = mSerialPort.Read(buffer, 0, 1);
                                    mSerialPort.Read(buffer, 0, 64);

                                    string s = Encoding.Default.GetString(buffer);
                                    if (!sBuffer.Equals(s))
                                    {
                                        Thread.Sleep(200);
                                        sBuffer = s;
                                        if (s.IndexOf(Convert.ToChar(01)) == 0 && s.IndexOf(Convert.ToChar(02)) == 1)
                                        {
                                            //2018年6月8日 多显示一位负数
                                            string ss = s.Substring(3, 6);
                                            if (!ss.Equals(sscache))
                                            {
                                                sscache = ss;
                                                try
                                                {
                                                    logger.Info("invoke ss =>>" + ss);
                                                    Listener?.Invoke(Convert.ToInt32(Convert.ToDouble(ss) * 1000) + "");
                                                }
                                                catch (Exception e)
                                                {
                                                    logger.Info(e.Message + e.StackTrace);
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            break;
                        case "Aclas"://顶尖电子秤
                            Task.Factory.StartNew(() =>
                            {
                                logger.Info("Aclas");
                                int size;
                                byte[] buffer = new byte[64];
                                while (mSerialPort.IsOpen)
                                {
                                    Thread.Sleep(50);
                                    //size = mSerialPort.Read(buffer, 0, 1);
                                    mSerialPort.Read(buffer, 0, 64);
                                    logger.Info("buffer is reading...=> " + buffer);
                                    
                                    string s = Encoding.Default.GetString(buffer);
                                    if (!sBuffer.Equals(s))
                                    {
                                        Thread.Sleep(200);
                                        sBuffer = s;
                                        if (s.IndexOf(Convert.ToChar(01)) == 0 && s.IndexOf(Convert.ToChar(02)) == 1)
                                        {
                                            //2018年6月8日 多显示一位负数
                                            string ss = s.Substring(3, 7);
                                            if (!ss.Equals(sscache))
                                            {
                                                sscache = ss;
                                                try
                                                {
                                                    logger.Info("invoke ss =>>" + ss);
                                                    Listener?.Invoke(Convert.ToInt32(Convert.ToDouble(ss) * 1000) + "");
                                                }
                                                catch (Exception e)
                                                {
                                                    logger.Info(e.Message + e.StackTrace);
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            break;
                    }

                }
                catch (Exception e)
                {
                    logger.Info(e.Message + e.StackTrace);
                }
            }
            else
            {
                logger.Info("mSerialPort = null");
            }
        }

        private void onDataReceived(byte[] buffer, int size)
        {
            if (buffer[0] == 32)
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
