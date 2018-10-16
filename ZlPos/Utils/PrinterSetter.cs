using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using ZlPos.Bean;
using ZlPos.Bizlogic;
using ZlPos.Config;
using ZlPos.Enums;
using ZlPos.Models;

namespace ZlPos.Utils
{
    public class PrinterSetter
    {

        private ResponseEntity responseEntity;

        public PrinterSetter()
        {
        }
        internal void SetPrinter(PrinterConfigEntity printerConfigEntity, Action<object> webCallback)
        {
            if (printerConfigEntity != null)
            {
                //add 2018/01/15 保存设置的小票打印份数
                PrinterManager.Instance.PrintNumber = int.Parse(printerConfigEntity.printernumber);
                responseEntity = new ResponseEntity();

                switch (printerConfigEntity.printerType)
                {
                    case "usb":
                        USBPrinterSetter usbPrinterSetter = new USBPrinterSetter();
                        usbPrinterSetter.setUSBPrinter(printerConfigEntity, webCallback);
                        break;
                    case "bluetooth":
                        BluethoothPrinterSetter bluethoothPrinterSetter = new BluethoothPrinterSetter();
                        bluethoothPrinterSetter.setBluethoothPrinter(printerConfigEntity, webCallback);
                        break;
                    case "port":
                        SerialPortPrinterSetter.setSerialPort(printerConfigEntity, webCallback);
                        break;
                    //add 增加并口 2018年5月29日
                    case "LPT":
                        LPTPrinterSetter.setLPT(printerConfigEntity, webCallback);
                        break;
                    //add 增加驱动打印 2018年10月15日
                    case "drive":
                        DrivePrinterSetter drivePrinterSetter = new DrivePrinterSetter();
                        drivePrinterSetter.SetDrivePrinterSetter(printerConfigEntity, webCallback);
                        break;
                    default:
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "打印机类型不可用";
                        //L.i(TAG, "打印机类型不可用");
                        if (webCallback != null)
                        {
                            webCallback.Invoke(new object[] { "setPrinterCallBack", responseEntity });
                        }
                        break;
                }
            }

        }

    }
}
