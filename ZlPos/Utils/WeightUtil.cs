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
                    mSerialPort.DataBits = 8;
                    mSerialPort.StopBits = StopBits.One;
                    mSerialPort.Parity = Parity.None;

                    //}
                    //2018年10月11日 把open还是放到case里面自己设置并判断吧
                    //logger.Info("open serialport");
                    //mSerialPort.Open();
                    //logger.Info("switch brand =>" + brand);
                    //brand = "TOLEDO";
                    switch (brand)
                    {
                        case "TOLEDO":
                            Task.Factory.StartNew(() =>
                            {
                                //重新设置串口
                                mSerialPort.DataBits = 7;
                                mSerialPort.StopBits = StopBits.One;
                                mSerialPort.Parity = Parity.Odd;
                                logger.Info("case TOLEDO");
                                mSerialPort.Open();
                                logger.Info("serialport is open");
                                byte[] buffer = new byte[64];
                                mSerialPort.DiscardInBuffer();
                                while (mSerialPort.IsOpen)
                                {
                                    logger.Info("serialport already to read...");
                                    Thread.Sleep(40);
                                    mSerialPort.Read(buffer, 0, 64);
                                    Thread.Sleep(100);
                                    logger.Info("serialport is readed");

                                    string s = Encoding.Default.GetString(buffer);

                                    logger.Info("serialport read buffer is =>" + s);
                                    try
                                    {
                                        int startIndex = s.IndexOf(Convert.ToChar(02));
                                        logger.Info("startindex = " + startIndex);
                                        int endIndex = s.IndexOf(Convert.ToChar(03), startIndex);
                                        //过滤掉脏数据
                                        if (startIndex < endIndex && startIndex < 41 && endIndex < 65)
                                        {
                                            logger.Info("oneDateStr process...");
                                            string oneDateStr = s.Substring(startIndex, 26);
                                            string oneDate = oneDateStr.Substring(6, 5);
                                            if (!oneDate.Equals(sscache))
                                            {
                                                Thread.Sleep(200);
                                                sscache = oneDate;
                                                try
                                                {
                                                    logger.Info("invoke ss =>>" + oneDate);
                                                    Listener?.Invoke(Convert.ToInt32(Convert.ToDouble(oneDate)) + "");
                                                }
                                                catch (Exception e)
                                                {
                                                    logger.Error("TOLEDO err", e);
                                                }
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error("TOLEDO ERR", ex);
                                    }
                                }

                            });
                            break;
                        case "ACS_A":
                            Task.Factory.StartNew(() =>
                            {
                                logger.Info("case ACS_A");
                                mSerialPort.Open();
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
                                mSerialPort.Open();
                                int size;
                                byte[] buffer = new byte[64];
                                while (mSerialPort.IsOpen)
                                {
                                    try
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
                                    catch (Exception e)
                                    {
                                        logger.Error("EH100", e);
                                    }
                                }
                            });
                            break;
                        case "Aclas"://顶尖电子秤
                            Task.Factory.StartNew(() =>
                            {
                                logger.Info("Aclas");
                                mSerialPort.Open();
                                int size;
                                byte[] buffer = new byte[64];
                                while (mSerialPort.IsOpen)
                                {
                                    Thread.Sleep(50);
                                    //size = mSerialPort.Read(buffer, 0, 1);
                                    mSerialPort.Read(buffer, 0, 64);
                                    logger.Info("buffer is reading...=> " + buffer);

                                    try
                                    {

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
                                    catch (Exception e)
                                    {
                                        logger.Error("Aclas", e);
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
