using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZlPos.Bean;
using ZlPos.Config;
using ZlPos.Models;

namespace ZlPos.Utils
{
    class GPrinterSetter
    {
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ResponseEntity responseEntity;

        public void setPrinter(PrinterConfigEntity printerConfigEntity, Action<object> p)
        {
            if(printerConfigEntity != null)
            {
                GPrinterManager.Instance.PrintNumber = int.Parse(printerConfigEntity.printernumber);
                responseEntity = new ResponseEntity();
                switch (printerConfigEntity.printerType)
                {
                    case "usb":
                        USBGPrinterSetter usbGPrinterSetter = new USBGPrinterSetter();
                        usbGPrinterSetter.setUSBPrinter(printerConfigEntity, (res) =>
                        {
                            if(res.code == ResponseCode.SUCCESS)
                            {
                                logger.Info("usb标签打印机设置成功");
                            }
                            else
                            {
                                logger.Info("usb标签打印机设置失败");
                            }
                            p?.Invoke(res);
                        });
                        break;
                    case "port":
                        break;
                    case "bluetooth":
                        break;
                    case "drive":
                        DriveBQPrinterSetter driveBQPrinterSetter = new DriveBQPrinterSetter();
                        driveBQPrinterSetter.SetDrivePrinterSetter(printerConfigEntity, p);
                        break;
                    default:
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "打印机类型不可用";
                        logger.Info("打印机类型不可用");
                        p?.Invoke(responseEntity);
                        break;
                }
            }
        }
    }
}
