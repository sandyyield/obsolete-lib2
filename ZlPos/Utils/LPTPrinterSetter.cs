using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Bean;
using ZlPos.Config;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    class LPTPrinterSetter
    {
        internal static void setLPT(PrinterConfigEntity printerConfigEntity, Action<object> webCallback)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            if(printerConfigEntity != null)
            {
                string lpt = printerConfigEntity.port;
                if (!string.IsNullOrEmpty(lpt))
                {
                    LPTPrinter lptPrinter;
                    if(PrinterManager.Instance.LptPrinter == null)
                    {
                        lptPrinter = new LPTPrinter();
                        lptPrinter.lptPort = lpt;
                    }
                    else
                    {
                        lptPrinter = PrinterManager.Instance.LptPrinter;
                        lptPrinter.Close();
                    }

                    if (lptPrinter.Init())
                    {
                        PrinterManager.Instance.Init = true;
                        PrinterManager.Instance.LptPrinter = lptPrinter;

                        lptPrinter.PrintString("并口打印机连接成功!\n\n\n\n\n");
                        responseEntity.code = ResponseCode.SUCCESS;
                        responseEntity.msg = "打印机设置成功";
                    }
                    else
                    {
                        lptPrinter.Close();
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "该端口不可用";
                    }

                    webCallback?.Invoke(new object[] { "setPrinterCallBack", responseEntity });
                }

            }
        }
    }
}
