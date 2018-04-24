using System;
using System.IO.Ports;

namespace ZlPos.Utils
{
    public class SerialReader
    {
        //static CommPortIdentifier portId;
        int delayRead = 100;
        int numBytes; // buffer中的实际数据字节数
        private static byte[] readBuffer = new byte[1024]; // 4k的buffer空间,缓存串口读入的数据
        //InputStream inputStream;
        //OutputStream outputStream;
        static SerialPort serialPort;
        //HashMap serialParams;
        private string port;
        private string rate;

        // 端口读入数据事件触发后,等待n毫秒后再读取,以便让数据一次性读完
        public static string PARAMS_DELAY = "delay read"; // 延时等待端口数据准备的时间
        public static string PARAMS_TIMEOUT = "timeout"; // 超时时间
        public static string PARAMS_PORT = "port name"; // 端口名称
        public static string PARAMS_DATABITS = "data bits"; // 数据位
        public static string PARAMS_STOPBITS = "stop bits"; // 停止位
        public static string PARAMS_PARITY = "parity"; // 奇偶校验
        public static string PARAMS_RATE = "rate"; // 波特率

        private object obj = new object();

        internal bool Enable(string rate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化端口
        /// </summary>
        /// <param name="port"></param>
        /// <param name="rate"></param>
        public SerialReader(string port, string rate)
        {
            this.port = port;
            this.rate = rate;
        }

        public bool open(object state)
        {
            bool isOpen = false;
            try
            {
                // 参数初始化
                int timeout = Integer.parseInt(params.get(PARAMS_TIMEOUT)
                        .toString());
                int rate = Integer.parseInt(params.get(PARAMS_RATE).toString());
                int dataBits = Integer.parseInt(params.get(PARAMS_DATABITS)
                        .toString());
                int stopBits = Integer.parseInt(params.get(PARAMS_STOPBITS)
                        .toString());
                int parity = Integer.parseInt(params.get(PARAMS_PARITY).toString());
                delayRead = Integer.parseInt(params.get(PARAMS_DELAY).toString());
                String port = params.get(PARAMS_PORT).toString();
                // 打开端口
                portId = CommPortIdentifier.getPortIdentifier(port);
                serialPort = (gnu.io.SerialPort)portId.open("SerialReader",
                        timeout);
                inputStream = serialPort.getInputStream();
                serialPort.addEventListener(this);
                serialPort.notifyOnDataAvailable(true);
                serialPort.setSerialPortParams(rate, dataBits, stopBits, parity);

                //2018年1月31日 add write timeout 6s
                serialPort.enableReceiveTimeout(6000);

                logger.info("打开串口成功：串口编号 " + this.port + " 波特率 " + rate);
                isOpen = true;


            }
            catch(Exception e)
            {

            }
            return isOpen;
        }


    }
}