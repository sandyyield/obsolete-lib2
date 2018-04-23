using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZlPos.Bean;
using ZlPos.Bizlogic;
using ZlPos.Config;
using ZlPos.Models;

namespace ZlPos.Utils
{
    class USBPrinterSetter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private String TAG = getClass().getSimpleName();
        private ResponseEntity responseEntity;
        //private ArrayList<UsbDevice> usbDevices;
        //private boolean isGet = false;//是否是获取USB设备列表

        public USBPrinterSetter()
        {
        }

        internal void setUSBPrinter(PrinterConfigEntity printerConfigEntity, JSBridge.JsCallbackHandle webCallback)
        {
            if (responseEntity.code == ResponseCode.SUCCESS)
            {
                logger.Info("usb打印机设置成功");
            }
            else
            {
                logger.Info("usb打印机设置失败");
            }
            if (webCallback != null)
            {
                webCallback.Invoke(responseEntity);
            }
        }
    }
}
