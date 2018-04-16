using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ZlPos.Enums;

namespace ZlPos.PrintServices
{
    //print bridge
    public static class PrintBridge
    {
        //include c++ library
        [DllImport("Library\\JsUsbDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr OpenUsb();

        [DllImport("Library\\JsUsbDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseUsb(IntPtr hUsb);

        [DllImport("Library\\JsUsbDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteUsb(IntPtr hUsb, string sendBuffer, int bufferSize, ref int sendCount);

        [DllImport("Library\\JsUsbDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr OpenComA(string comn, int baud);

        [DllImport("Library\\JsUsbDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseCom(IntPtr hUsb);

        [DllImport("Library\\JsUsbDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteCom(IntPtr hUsb, string sendBuffer, int bufferSize, ref int sendCount);


        [DllImport("Library\\JsUsbDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern EnStateFlag SendInstruction(EnPrinterPort pPort,
            IntPtr handle,
            EnPrinterInstruction pInstr,
            int parameter1,
            int parameter2,
            int parameter3,
            int parameter4);

        [DllImport("Library\\POS_SDK.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr POS_Port_OpenW(string szName, int iPort, bool bFile, string szFilePath);

        [DllImport("Library\\POS_SDK.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int POS_Port_Close(IntPtr iPrinterID);

        [DllImport("Library\\POS_SDK.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int POS_Output_PrintFontStringW(IntPtr iPrinterID,
                                                            int iFont,
                                                            int iThick,
                                                            int iWidth,
                                                            int iHeight,
                                                            int iUnderLine,
                                                            string lpString);


    }
}
