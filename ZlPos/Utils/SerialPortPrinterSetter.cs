using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Bean;
using ZlPos.Bizlogic;
using ZlPos.Models;

namespace ZlPos.Utils
{
    public class SerialPortPrinterSetter
    {
        
        internal static void setSerialPort(PrinterConfigEntity printerConfigEntity, JSBridge.JsCallbackHandle webCallback)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            if (webCallback != null)
            {
                webCallback.Invoke(new object[] { "setPrinterCallBack", responseEntity });
            }
        }
    }
}
