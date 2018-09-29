using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Bean;
using ZlPos.Config;
using ZlPos.Models;

namespace ZlPos.Utils
{
    class USBBJQPrinterSetter
    {
        private ResponseEntity responseEntity;
        private Action<ResponseEntity> listener;
        //public object USBPrinter { get; internal set; }
        internal void setUSBPrinter(PrinterConfigEntity printerConfigEntity, Action<ResponseEntity> p)
        {
            listener = p;
            responseEntity = new ResponseEntity();
            if (printerConfigEntity != null)
            {
                BJQPrinterManager.Instance.printerConfigEntity = printerConfigEntity;

                //TOFIX...以下直接用Gprinter的 usb代码凑活一下
                //GPrinterManager.Instance.Init = true;
                List<string> usblist = GPrinterUtils.Instance.FindUSBPrinter();
                if (usblist == null)
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "未发现可用USB设备";
                    listener?.Invoke(responseEntity);
                }
                else
                {
                    GPrinterManager.Instance.usbDeviceArrayList = usblist;
                    if (GPrinterUtils.Instance.Connect_Printer())
                    {
                        GPrinterManager.Instance.Init = true;
                        GPrinterManager.Instance.PrinterTypeEnum = "usb";
                        responseEntity.code = ResponseCode.SUCCESS;
                        responseEntity.msg = "打印机设置成功";
                    }
                    else
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "打印机设置失败";
                    }
                }

            }
            else
            {
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "打印机设置失败,参数为空";
            }
            listener?.Invoke(responseEntity);
        }

        public string GetUsbDevices()
        {
            responseEntity = new ResponseEntity();
            List<string> usbList = GPrinterUtils.Instance.FindUSBPrinter();
            DeviceEntity deviceEntity = new DeviceEntity();
            deviceEntity.devices = usbList;
            responseEntity.code = ResponseCode.SUCCESS;
            responseEntity.data = deviceEntity;
            responseEntity.msg = "获取USB设备成功";
            return JsonConvert.SerializeObject(responseEntity);
        }
    }
}
