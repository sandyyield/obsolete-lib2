using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Bean;
using ZlPos.Models;

namespace ZlPos.Utils
{
    class USBGPrinterSetter
    {
        private ResponseEntity responseEntity;
        private Action<ResponseEntity> listener;
        //public object USBPrinter { get; internal set; }
        internal void setUSBPrinter(PrinterConfigEntity printerConfigEntity, Action<ResponseEntity> p)
        {
            if(printerConfigEntity != null)
            {
                listener = p;
                responseEntity = new ResponseEntity();
                GPrinterManager.Instance.printerConfigEntity = printerConfigEntity;



            }
        }
    }
}
