using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ZlPos.Models;

namespace ZlPos.Utils
{
    class GPrinterUtils
    {
        private libUsbContorl.UsbOperation NewUsb = new libUsbContorl.UsbOperation();

        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private int mPrinterIndex = 0;
        private int usbPrintIndex = 0;
        private int enterPrinterIndex = 1;
        private static object obj = new object();
        private static GPrinterUtils gPrinterUtils;
        //佳博打印机
        //private GpService mGpService = null;
        //private PrinterServiceConnection conn = null;

        private Dictionary<String, IntPtr> deviceIndexMap = new Dictionary<string, IntPtr>();

        private const int MAIN_QUERY_PRINTER_STATUS = 0xfe;
        private const int REQUEST_PRINT_LABEL = 0xfd;
        private const int REQUEST_PRINT_RECEIPT = 0xfc;

        //private ConnectListener connectListener;

        private GPrinterUtils()
        {
            //TODO...？？？
        }

        public static GPrinterUtils Instance
        {
            get
            {
                lock (obj)
                {
                    if (gPrinterUtils == null)
                    {
                        gPrinterUtils = new GPrinterUtils();
                    }
                    return gPrinterUtils;
                }
            }
        }


        internal void PrintCommodotyLabel(string json)
        {
            if (Connect_Printer())
            {
                PrintCommodityEntity printCommodityEntity = JsonConvert.DeserializeObject<PrintCommodityEntity>(json);
                List<CommodityEntity> commodityEntityList = printCommodityEntity.commodityList;
                if (commodityEntityList != null)
                {
                    foreach (CommodityEntity item in commodityEntityList)
                    {
                        SendBLineLabel(item, printCommodityEntity.number);
                    }
                }
            }
            else
            {
                logger.Info("条码标签打印机连接失败");
            }
        }

        private void SendBLineLabel(CommodityEntity commodityEntity, int number)
        {
            byte[] enddata = { 0x0a };//换行

            List<string> s = new List<string>();
            s.Add("SIZE 80 mm,35 mm");
            s.Add("BLINE 32 mm,0 mm");
            s.Add("DIRECTION 1");
            s.Add("REFERENCE 0,0");
            s.Add("SET TEAR ON");
            s.Add("CLS");
            s.Add("TEXT 80,60,\"TSS24.BF2\",0,1,2,\"" + commodityEntity.commodityname + "\"");
            s.Add("TEXT 80,130,\"TSS24.BF2\",0,1,1,\"" + commodityEntity.commoditycode + "\"");
            s.Add("TEXT 300,130,\"TSS24.BF2\",0,1,1,\"" + commodityEntity.unitname + "\"");
            s.Add("TEXT 80,180,\"TSS24.BF2\",0,1,1,\"" + commodityEntity.spec + "\"");
            s.Add("TEXT 480,180,\"TSS24.BF2\",0,2,2,\"" + commodityEntity.saleprice + "\"");

            if (!string.IsNullOrEmpty(commodityEntity.barcode))
            {
                s.Add("BARCODE 150,220,\"128M\",40,1,0,2,2,\"" + commodityEntity.barcode + "\"");
            }
            //s.Add(" PRINT " + number);
            s.Add(" PRINT " + BJQPrinterManager.Instance.PrintNumber);
            s.Add("SOUND 2,100");

            foreach (var item in s)
            {
                if (!string.IsNullOrEmpty(item))
                {

                    byte[] strb = Encoding.Default.GetBytes(item);
                    SendData_Printer(strb);
                    SendData_Printer(enddata);
                }
            }
        }


        private void SendBarcodeLabel(CommodityEntity commodityEntity, int number)
        {
            byte[] enddata = { 0x0a };//换行

            List<string> s = new List<string>();
            s.Add("SIZE 50 mm,30 mm");
            s.Add("GAP 2 0");//TODO...
            s.Add("DIRECTION 0");
            s.Add("REFERENCE 0,0");
            s.Add("SET TEAR ON");
            s.Add("CLS");
            s.Add("TEXT 20,20,\"TSS24.BF2\",0,1,1,\"" + commodityEntity.commodityname + "\"");
            s.Add("TEXT 10,60,\"TSS24.BF2\",0,1,1,\"" + "规格 " + commodityEntity.spec + "\"");
            s.Add("TEXT 140,60,\"TSS24.BF2\",0,1,1,\"" + "单位 " + commodityEntity.unitname + "\"");
            s.Add("TEXT 280,60,\"TSS24.BF2\",0,1,1,\"" + "零售价" + "\"");
            s.Add("TEXT 280,100,\"TSS24.BF2\",0,2,2,\"" + commodityEntity.saleprice + "\"");
            s.Add("TEXT 10,130,\"TSS24.BF2\",0,1,1,\"" + "条码 " + "\"");
            if (!string.IsNullOrEmpty(commodityEntity.barcode))
            {
                s.Add("BARCODE 30,160,\"128M\",40,1,0,2,2,\"" + commodityEntity.barcode + "\"");
            }

            //s.Add(" PRINT " + number);
            s.Add(" PRINT " + GPrinterManager.Instance.PrintNumber);
            s.Add("SOUND 2,100");

            foreach (var item in s)
            {
                if (!string.IsNullOrEmpty(item))
                {

                    byte[] strb = Encoding.Default.GetBytes(item);
                    SendData_Printer(strb);
                    SendData_Printer(enddata);
                }
            }
        }

        /// <summary>
        /// 打印服装版商品标签
        /// </summary>
        /// <param name="commodityEntity"></param>
        /// <param name="number"></param>
        private void SendGarmentLabel(CommodityEntity commodityEntity, int number)
        {
            byte[] enddata = { 0x0a };//换行

            List<string> s = new List<string>();
            s.Add("SIZE 40 mm,30 mm");
            s.Add("GAP 2 0");//TODO...
            s.Add("DIRECTION 0");
            s.Add("REFERENCE 0,0");
            s.Add("SET TEAR ON");
            s.Add("CLS");
            s.Add("TEXT 80,20,\"TSS24.BF2\",0,1,1,\"" + commodityEntity.commodityname + "\"");
            s.Add("TEXT 40,70,\"TSS24.BF2\",0,1,1,\"" + "颜色:" + commodityEntity.color + "\"");
            s.Add("TEXT 40,100,\"TSS24.BF2\",0,1,1,\"" + "尺码:" + commodityEntity.size +  "\"");
            s.Add("TEXT 40,130,\"TSS24.BF2\",0,1,1,\"" + "零售价:" + commodityEntity.saleprice + "\"");
            //s.Add("TEXT 280,100,\"TSS24.BF2\",0,2,2,\"" + commodityEntity.saleprice + "\"");
            //s.Add("TEXT 10,130,\"TSS24.BF2\",0,1,1,\"" + "条码 " + "\"");
            if (!string.IsNullOrEmpty(commodityEntity.barcode))
            {
                s.Add("BARCODE 40,160,\"128M\",40,1,0,1,1,\"" + commodityEntity.barcode + "\"");
            }

            //s.Add(" PRINT " + number);
            s.Add(" PRINT " + GPrinterManager.Instance.PrintNumber);
            s.Add("SOUND 2,100");

            foreach (var item in s)
            {
                if (!string.IsNullOrEmpty(item))
                {

                    byte[] strb = Encoding.Default.GetBytes(item);
                    SendData_Printer(strb);
                    SendData_Printer(enddata);
                }
            }
        }

        private void SendData_Printer(byte[] str)
        {
            NewUsb.SendData2USB(str, str.Length);
        }

        public bool Connect_Printer()
        {
            bool bo = false;
            List<string> usbPortList = FindUSBPrinter();

            if (usbPortList == null)
            {
                logger.Info("Connect_Printer : find usblist is null");
                return false;
            }

            foreach (string item in usbPortList)
            {
                bo = NewUsb.LinkUSB(Convert.ToInt32(item));
                if (!bo)
                {
                    //MessageBox.Show("USB Communication Error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    logger.Info("Connect_Printer : reFindUSB >> port index => i=" + item);
                    reFindUSB();
                    //return bo;
                }
            }
            return bo;
        }

        public List<string> FindUSBPrinter()
        {
            List<string> usbPortList = null;

            NewUsb.FindUSBPrinter();
            if (NewUsb.USBPortCount != 0)
            {
                usbPortList = new List<string>();
                for (int i = 0; i < NewUsb.USBPortCount; i++)
                {
                    usbPortList.Add(i.ToString());
                }
            }

            return usbPortList;
        }

        private void reFindUSB()
        {
            NewUsb.FindUSBPrinter();
        }


        internal void printLabel(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }
            BillEntity billEntity = JsonConvert.DeserializeObject<BillEntity>(content);
            if (billEntity == null)
            {
                return;
            }
            List<BillCommodityEntity> commoditys = billEntity.commoditys;
            if (commoditys != null && commoditys.Count > 0)
            {
                foreach (BillCommodityEntity billCommodityEntity in commoditys)
                {
                    if ("1".Equals(billCommodityEntity.pricing))
                    {//称重商品
                        PrintOneLable(billCommodityEntity, "");
                    }
                    else
                    {
                        Double doubleCount = Double.Parse(billCommodityEntity.salenums);
                        int count = (int)doubleCount;//Int32.Parse(billCommodityEntity.salenums);
                        if (count > 1)
                        {
                            for (int m = 1; m <= count; m++)
                            {
                                PrintOneLable(billCommodityEntity, "     (" + m + "/" + count + ")");
                            }
                        }
                        else
                        {
                            PrintOneLable(billCommodityEntity, "");
                        }
                    }

                }
                NewUsb.CloseUSBPort();
            }
        }

        /// <summary>
        /// 打印服装版商品标签
        /// </summary>
        /// <param name="s"></param>
        internal void PrintGarmentLabel(string commodityInfo)
        {
            if (Connect_Printer())
            {
                if (!string.IsNullOrEmpty(commodityInfo))
                {
                    PrintCommodityEntity printCommodityEntity = JsonConvert.DeserializeObject<PrintCommodityEntity>(commodityInfo);
                    List<CommodityEntity> commodityEntityList = printCommodityEntity.commodityList;
                    if (commodityEntityList != null)
                    {
                        foreach (CommodityEntity item in commodityEntityList)
                        {
                            SendGarmentLabel(item, printCommodityEntity.number);
                            Thread.Sleep(3000);
                        }
                        NewUsb.CloseUSBPort();
                    }
                }
            }
            else
            {
                logger.Info("连接标签打印机失败 GprinterUtils");
            }
        }

        

        /// <summary>
        /// 标签打印 打印商品订单
        /// </summary>
        /// <param name="billCommodityEntity"></param>
        /// <param name="v"></param>
        private void PrintOneLable(BillCommodityEntity billCommodityEntity, string v)
        {
            if (Connect_Printer())
            {

                byte[] enddata = { 0x0a };//换行

                List<string> s = new List<string>();

                s.Add("SIZE 60 mm,40 mm");
                s.Add("GAP 2 0");//TODO...
                s.Add("DIRECTION 0");
                s.Add("REFERENCE 0,0");
                s.Add("SET TEAR ON");
                s.Add("CLS");

                s.Add("TEXT 20,20,\"TSS24.BF2\",0,2,2,\"" + center(24, billCommodityEntity.branchname, 2) + "\"");

                s.Add("TEXT 360,20,\"TSS24.BF2\",0,2,2,\"" + center(8, billCommodityEntity.ticketcode.Substring(billCommodityEntity.ticketcode.LastIndexOf("-") + 1), 2) + "\"");

                s.Add("TEXT 0,80,\"TSS24.BF2\",0,1,1,\"" + "`````````````````````````````````````````````````````````````````" + "\"");
                s.Add("TEXT 20,110,\"TSS24.BF2\",0,1,1,\"" + billCommodityEntity.commodityname + "\"");

                if ("1".Equals(billCommodityEntity.pricing))
                {//称重商品
                    s.Add("TEXT 20,280,\"TSS24.BF2\",0,1,1,\"" + "￥" + billCommodityEntity.paysubtotal + "          " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                    //tsc.addText(20, 280, FONTTYPE.SIMPLIFIED_CHINESE, ROTATION.ROTATION_0,
                    //        FONTMUL.MUL_1, FONTMUL.MUL_1, "￥" + billCommodityEntity.getPaysubtotal() + "          " + DateUtil.getTime(new Date()));
                }
                else
                {
                    s.Add("TEXT 20,280,\"TSS24.BF2\",0,1,1,\"" + "￥" + billCommodityEntity.saleprice + "          " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                    //tsc.addText(20, 280, FONTTYPE.SIMPLIFIED_CHINESE, ROTATION.ROTATION_0,
                    //        FONTMUL.MUL_1, FONTMUL.MUL_1, "￥" + billCommodityEntity.getSaleprice() + "          " + DateUtil.getTime(new Date()));
                }

                //s.Add("PRINT 1");
                s.Add("PRINT " + GPrinterManager.Instance.PrintNumber);//TOFIX...打印份数应当通过设置
                s.Add("SOUND 2,100");

                foreach (var item in s)
                {
                    if (!string.IsNullOrEmpty(item))
                    {

                        byte[] strb = Encoding.Default.GetBytes(item);
                        SendData_Printer(strb);
                        SendData_Printer(enddata);
                    }
                }
            }
        }

        private string center(int count, string s, int level)
        {
            StringBuilder stringBuffer = new StringBuilder(s);
            for (int i = 0; i < (count - s.Count() * level) / 2 / level; i++)
            {
                stringBuffer.Insert(0, " ");
            }
            return stringBuffer.ToString();
        }

        internal void printLabelTest()
        {
            throw new NotImplementedException();
        }

        internal void printUSBTest()
        {
            byte[] enddata = { 0x0a };//换行

            List<string> s = new List<string>();
            s.Add("SIZE 60 mm,40 mm");
            s.Add("GAP 2 0");//TODO...
            s.Add("DIRECTION 0");
            s.Add("REFERENCE 0,0");
            s.Add("SET TEAR ON");
            s.Add("CLS");

            s.Add("TEXT 140,100,\"TSS24.BF2\",0,2,2,\"" + "标签打印测试" + "\"");


            s.Add(" PRINT 1");
            s.Add("SOUND 2,100");

            foreach (var item in s)
            {
                if (!string.IsNullOrEmpty(item))
                {

                    byte[] strb = Encoding.Default.GetBytes(item);
                    SendData_Printer(strb);
                    SendData_Printer(enddata);
                }
            }
            NewUsb.CloseUSBPort();
        }

        internal void PrintBarcodeLable(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                PrintCommodityEntity printCommodityEntity = JsonConvert.DeserializeObject<PrintCommodityEntity>(json);
                List<CommodityEntity> commodityEntityList = printCommodityEntity.commodityList;
                if (commodityEntityList != null)
                {
                    foreach (CommodityEntity item in commodityEntityList)
                    {
                        SendBarcodeLabel(item, printCommodityEntity.number);
                        Thread.Sleep(3000);
                    }
                    NewUsb.CloseUSBPort();
                }
            }
        }

        
    }
}
