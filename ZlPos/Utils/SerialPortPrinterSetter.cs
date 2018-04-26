using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Bean;
using ZlPos.Bizlogic;
using ZlPos.Config;
using ZlPos.Enums;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    public class SerialPortPrinterSetter
    {

        internal static void setSerialPort(PrinterConfigEntity printerConfigEntity, Action<object> webCallback)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            if (printerConfigEntity != null)
            {
                string port = printerConfigEntity.port;
                string intBaud = printerConfigEntity.intBaud;
                if (!String.IsNullOrEmpty(port) && !String.IsNullOrEmpty(intBaud))
                {
                    serialPort m_serialPort;
                    if (PrinterManager.Instance.PortPrinter == null)
                    {
                        m_serialPort = new serialPort(port,intBaud);
                        m_serialPort.init();
                    }
                    else
                    {
                        m_serialPort = PrinterManager.Instance.PortPrinter;
                        m_serialPort.Close();
                    }
                    if (m_serialPort.Open(port, int.Parse(intBaud)))
                    {
                        PrinterManager.Instance.Init = true;
                        PrinterManager.Instance.PrinterTypeEnum = PrinterTypeEnum.port;
                        PrinterManager.Instance.PortPrinter = m_serialPort;

                        m_serialPort.Write("打印机测试成功!\r\n\r\n\r\n\r\n\r\n\r\n\r\n");
                        responseEntity.code = ResponseCode.SUCCESS;
                        responseEntity.msg = "打印机设置成功";

                    }
                    else
                    {
                        m_serialPort.Close();
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "该端口不可用";
                    }
                }
                else
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "端口或波特率参数不能为空";
                }


                if (webCallback != null)
                {
                    webCallback.Invoke(new object[] { "setPrinterCallBack", responseEntity });
                }
            }
        }

    }
}
