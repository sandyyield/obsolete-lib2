using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Bean;
using ZlPos.Bizlogic;
using ZlPos.Models;

namespace ZlPos.Utils
{
    public class BluethoothPrinterSetter
    {
        private ResponseEntity responseEntity;
        internal void setBluethoothPrinter(PrinterConfigEntity printerConfigEntity, JSBridge.JsCallbackHandle webCallback)
        {
            if (webCallback != null)
            {
                webCallback.Invoke(new object[] { "setPrinterCallBack", responseEntity });
            }
        }
    }
}
