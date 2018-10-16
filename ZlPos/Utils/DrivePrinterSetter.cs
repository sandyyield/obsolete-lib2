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
    class DrivePrinterSetter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ResponseEntity responseEntity;
        internal void SetDrivePrinterSetter(PrinterConfigEntity printerConfigEntity, Action<object> webCallback)
        {
            responseEntity = new ResponseEntity();
            if (printerConfigEntity != null)
            {
                PrinterManager.Instance.PrinterConfigEntity = printerConfigEntity;
                try
                {
                    //DrivePrinter最好还是采用单例比较好
                    DrivePrinter drivePrinter = DrivePrinter.Instance;
                    drivePrinter.printerName = printerConfigEntity.printerName; //"GP-5890X Series";

                    drivePrinter.SetPrinterName();

                    drivePrinter.Print("驱动连接打印机成功\r\n\r\n\r\n");
                    drivePrinter.pageWidth = printerConfigEntity.pageWidth;
                    PrinterManager.Instance.Init = true;
                    PrinterManager.Instance.PrinterTypeEnum = Enums.PrinterTypeEnum.drive;
                    PrinterManager.Instance.DrivePrinter = drivePrinter;
                    PrinterManager.Instance.PrinterConfigEntity = printerConfigEntity;


                    responseEntity.code = ResponseCode.SUCCESS;
                    responseEntity.msg = "打印机设置成功";
                }
                catch (Exception e)
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "驱动打印出错";
                    logger.Error("drive print err", e);
                }
            }
            else
            {
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "参数不能为空";
            }
            if (webCallback != null)
            {
                webCallback.Invoke(new object[] { "setPrinterCallBack", responseEntity });
            }
        }
    }
}
