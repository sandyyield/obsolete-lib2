﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZlPos.Bean;
using ZlPos.Bizlogic;
using ZlPos.Config;
using ZlPos.Enums;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    class USBPrinterSetter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private String TAG = getClass().getSimpleName();
        private ResponseEntity responseEntity;
        //private ArrayList<UsbDevice> usbDevices;
        //private boolean isGet = false;//是否是获取USB设备列表

        JSBridge.JsCallbackHandle listener;

        public USBPrinterSetter()
        {
        }

        internal void setUSBPrinter(PrinterConfigEntity printerConfigEntity, JSBridge.JsCallbackHandle webCallback)
        {
            IntPtr hUsb;
            if (printerConfigEntity != null)
            {
                listener = webCallback;
                responseEntity = new ResponseEntity();
                PrinterManager.Instance.PrinterConfigEntity = printerConfigEntity;
                if (PrinterManager.Instance.UsbPrinter == null)
                {
                    USBPrinter usbPrinter = new USBPrinter();
                    if (!usbPrinter.Init)
                    {
                        hUsb = usbPrinter.open();
                        usbPrinter.HDevice = hUsb;
                    }
                    PrinterManager.Instance.Init = true;
                    PrinterManager.Instance.PrinterTypeEnum = PrinterTypeEnum.usb;
                    PrinterManager.Instance.UsbPrinter = usbPrinter;
                    PrinterManager.Instance.PrinterConfigEntity = printerConfigEntity;

                    usbPrinter.PrintString("蓝牙打印机测试成功\r\n\r\n\r\n\r\n");
                    responseEntity.code = ResponseCode.SUCCESS;
                    responseEntity.msg = "打印机设置成功";
                    if (listener != null)
                    {
                        listener.Invoke(new object[] { "setPrinterCallBack", responseEntity });
                    }

                }
                else
                {
                    //TODO...
                }


                //getUsbDevices();


                //debug 

                PrintServices.USBPrinterService upt = new PrintServices.USBPrinterService();
                upt.TestPrint();
                responseEntity.code = ResponseCode.SUCCESS;

            }
            else
            {
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "参数不能为空";
            }
            if (listener != null)
            {
                listener.Invoke(new object[] { "setPrinterCallBack", responseEntity });
            }


        }

        //private void getUsbDevices()
        //{
        //    //TODO... 这里的逻辑和android那边的不太一样  因为是引用的第三方封装dll方法 所以好像一些排版会有问题

        //    USBPrinter usbPrinter = new USBPrinter();

        //}

        //public Handle

        //private void getUsbDevices()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
