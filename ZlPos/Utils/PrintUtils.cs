using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZlPos.Enums;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    public class PrintUtils
    {
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region print model usb
        public static void printModel(string content, USBPrinter usbPrinter)
        {
            if (string.IsNullOrEmpty(content) && usbPrinter == null)
            {
                return;
            }
            List<PrintEntity> printEntities = JsonConvert.DeserializeObject<List<PrintEntity>>(content);
            if (printEntities != null && printEntities.Count > 0)
            {
                usbPrinter.initUSB();
                for (int i = 0; i < printEntities.Count; i++)
                {
                    if (string.IsNullOrEmpty(printEntities[i].isQRCode) || printEntities[i].isQRCode == "0")
                    {
                        logger.Info("usb print:" + printEntities[i].content);
                        //add 2018年9月10日 增加一个换行
                        usbPrinter.PrintString(printEntities[i].content + "\n");
                    }
                    else
                    {
                        //usb打印二维码可能会出现问题 所以直接不打印
                        //usbPrinter.printQRCode(printEntities[i].content);
                    }
                }
            }
            usbPrinter.PrintString("\n\n\n\n\n\n");
            //usbPrinter.PrintString(content);
        }
        #endregion

        #region print QRCode usb
        internal static void PrintQRCode(string code, USBPrinter usbPrinter)
        {
            if (string.IsNullOrEmpty(code) && usbPrinter == null)
            {
                return;
            }
            usbPrinter.initUSB();
            usbPrinter.printQRCode(code);
        }
        #endregion
        #region printmodel bluetooth
        public static void printModel(string content, BluetoothPrinter bluetoothPrinter)
        {
            if (string.IsNullOrEmpty(content) && bluetoothPrinter == null)
            {
                return;
            }
            List<PrintEntity> printEntities = JsonConvert.DeserializeObject<List<PrintEntity>>(content);
            if (printEntities != null && printEntities.Count > 0)
            {
                if (!bluetoothPrinter.isConnected())
                {
                    bluetoothPrinter.openConnection();
                }

                for (int i = 0; i < printEntities.Count; i++)
                {
                    if (string.IsNullOrEmpty(printEntities[i].isQRCode) || printEntities[i].isQRCode == "0")
                    {
                        logger.Info("bluetooth print:" + printEntities[i].content);
                        //add 2018年9月10日 增加换行
                        bluetoothPrinter.PrintString(printEntities[i].content + "\n");
                    }
                    else
                    {
                        //usb打印二维码可能会出现问题 所以直接不打印
                        //usbPrinter.printQRCode(printEntities[i].content);
                    }
                }
            }
            bluetoothPrinter.PrintString("\n\n\n\n\n\n");
            //bluetoothPrinter.PrintString(content);
        }
        #endregion

        #region printmodel port
        public static void printModel(string content, serialPort portPrinter)
        {
            if (string.IsNullOrEmpty(content) && portPrinter == null)
            {
                return;
            }
            List<PrintEntity> printEntities = JsonConvert.DeserializeObject<List<PrintEntity>>(content);

            if (printEntities != null && printEntities.Count > 0)
            {
                //portPrinter.initUSB();
                for (int i = 0; i < printEntities.Count; i++)
                {
                    //portPrinter.PrintString(printEntities[i].content);
                    if (string.IsNullOrEmpty(printEntities[i].isQRCode) || printEntities[i].isQRCode == "0")
                    {
                        logger.Info("port print:" + printEntities[i].content);
                        //add 2018年9月10日 增加换行
                        portPrinter.PrintString(printEntities[i].content + "\n");
                    }
                    else
                    {
                        portPrinter.PrintQRCode(printEntities[i].content);
                    }
                }
            }
            portPrinter.PrintString("\n\n\n\n\n\n");
            //portPrinter.Write(content);
        }
        #endregion
        internal static void PrintQRCode(string code, serialPort portPrinter)
        {
            if (string.IsNullOrEmpty(code) && portPrinter == null)
            {
                return;
            }
            portPrinter.PrintString(code);
        }
        #region print QRCode port

        #endregion

        #region printmodel lpt
        internal static void printModel(string content, LPTPrinter lptPrinter)
        {
            if (string.IsNullOrEmpty(content) && lptPrinter == null)
            {
                return;
            }
            content = content.Replace("\\r"," ");
            content = content.Replace("\\n", "\n");
            List<PrintEntity> printEntities = JsonConvert.DeserializeObject<List<PrintEntity>>(content);
            if (printEntities != null && printEntities.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                logger.Info("xx传的JSON" + content);
                //portPrinter.initUSB();
                for (int i = 0; i < printEntities.Count; i++)
                {
                    logger.Info("lpt print:" + printEntities[i].content);

                    if (string.IsNullOrEmpty(printEntities[i].isQRCode) || printEntities[i].isQRCode == "0")
                    {
                        //add 2018年9月10日 增加换行
                        sb.Append(printEntities[i].content + "\n");
                    }
                    else
                    {
                        //并口没有二维码
                        //portPrinter.PrintQRCode(printEntities[i].content);
                    }
                }
                lptPrinter.PrintString(sb.ToString() + "\n\n\n\n\n\n\n");
            }
        }
        #endregion

        #region printnote usb
        internal static void printNote(StatisticsVM statisticsVM, USBPrinter mPrinter)
        {
            if (statisticsVM == null)
            {
                return;
            }
            if (mPrinter == null)
            {
                return;
            }
            if (mPrinter.CurrentPrintType == PrinterType.TIII || mPrinter.CurrentPrintType == PrinterType.T5)
            {
                mPrinter.PrintString("             收银对账             \n");
            }
            else
            {
                mPrinter.PrintString("                      收银对账                    \n");
            }

            StringBuilder sb = new StringBuilder();
            if (mPrinter.CurrentPrintType == PrinterType.TIII || mPrinter.CurrentPrintType == PrinterType.T5)
            {
                sb.Append("------------------------------\n");
            }
            else
            {
                sb.Append("----------------------------------------------\n");
            }
            sb.Append(Resources.R.branch_name + statisticsVM.branchname + "\n");
            sb.Append(Resources.R.time + DateTime.Now.ToString("D") + "\n");
            sb.Append(Resources.R.date_time + statisticsVM.starttime + "至" + statisticsVM.endtime + "\n");
            sb.Append(Resources.R.shop_cashier_num + statisticsVM.cashiername + "\n");
            sb.Append(Resources.R.ticketnums + statisticsVM.ticketnums + "\n");
            sb.Append(Resources.R.ticketamount + statisticsVM.ticketamount + "\n");
            sb.Append(Resources.R.returnnums + statisticsVM.returnnums + "\n");
            sb.Append(Resources.R.returnamount + statisticsVM.returnamount + "\n");
            sb.Append(Resources.R.rechargeamount + statisticsVM.rechargeamount + "\n");
            sb.Append(Resources.R.subtotal + statisticsVM.subtotal + "\n");
            if (mPrinter.CurrentPrintType == PrinterType.TIII || mPrinter.CurrentPrintType == PrinterType.T5)
            {
                sb.Append("------------------------------\n");
            }
            else
            {
                sb.Append("----------------------------------------------\n");
            }
            mPrinter.PrintString(sb.ToString());

            StringBuilder sbtb = new StringBuilder();

            sbtb.Append(tableFormat(Resources.R.payType, true, 15, false));
            sbtb.Append(tableFormat(Resources.R.mumber, false, 8, false));
            sbtb.Append(tableFormat(Resources.R.money, false, 8, false));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("现金", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.cashnums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.cashamount, false, 8, true));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("支付宝", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.alinums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.aliamount, false, 8, true));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("微信", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.wxnums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.wxamount, false, 8, true));
            sbtb.Append("\n");

            foreach (ZidingyizhifuBean zidingyizhifuBean in statisticsVM.zidingyizhifu)
            {
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyiname, true, 15, false));
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyinums, false, 8, true));
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyiamount, false, 8, true));
                sbtb.Append("\n");
            }
            sbtb.Append(tableFormat("合计", true, 15, false));
            sbtb.Append(tableFormat("", false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.subtotal, false, 8, true));
            sbtb.Append("\n\n\n");
            mPrinter.PrintString(sbtb.ToString());
            mPrinter.PrintString("\n\n\n\n\n");
        }
        #endregion

        #region printnote bluetooth
        internal static void printNote(StatisticsVM statisticsVM, BluetoothPrinter mPrinter)
        {
            if (statisticsVM == null)
            {
                return;
            }
            if (mPrinter == null)
            {
                return;
            }
            if (mPrinter.CurrentPrintType == PrinterType.TIII || mPrinter.CurrentPrintType == PrinterType.T5)
            {
                mPrinter.PrintString("             收银对账             \n");
            }
            else
            {
                mPrinter.PrintString("                      收银对账                    \n");
            }
            StringBuilder sb = new StringBuilder();
            if (mPrinter.CurrentPrintType == PrinterType.TIII || mPrinter.CurrentPrintType == PrinterType.T5)
            {
                sb.Append("------------------------------\n");
            }
            else
            {
                sb.Append("----------------------------------------------\n");
            }
            sb.Append(Resources.R.branch_name + statisticsVM.branchname + "\n");
            sb.Append(Resources.R.time + DateTime.Now.ToString("D") + "\n");
            sb.Append(Resources.R.date_time + statisticsVM.starttime + "至" + statisticsVM.endtime + "\n");
            sb.Append(Resources.R.shop_cashier_num + statisticsVM.cashiername + "\n");
            sb.Append(Resources.R.ticketnums + statisticsVM.ticketnums + "\n");
            sb.Append(Resources.R.ticketamount + statisticsVM.ticketamount + "\n");
            sb.Append(Resources.R.returnnums + statisticsVM.returnnums + "\n");
            sb.Append(Resources.R.returnamount + statisticsVM.returnamount + "\n");
            sb.Append(Resources.R.rechargeamount + statisticsVM.rechargeamount + "\n");
            sb.Append(Resources.R.subtotal + statisticsVM.subtotal + "\n");
            if (mPrinter.CurrentPrintType == PrinterType.TIII || mPrinter.CurrentPrintType == PrinterType.T5)
            {
                sb.Append("------------------------------\n");
            }
            else
            {
                sb.Append("----------------------------------------------\n");
            }
            mPrinter.PrintString(sb.ToString());

            StringBuilder sbtb = new StringBuilder();

            sbtb.Append(tableFormat(Resources.R.payType, true, 15, false));
            sbtb.Append(tableFormat(Resources.R.mumber, false, 8, false));
            sbtb.Append(tableFormat(Resources.R.money, false, 8, false));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("现金", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.cashnums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.cashamount, false, 8, true));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("支付宝", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.alinums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.aliamount, false, 8, true));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("微信", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.wxnums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.wxamount, false, 8, true));
            sbtb.Append("\n");

            foreach (ZidingyizhifuBean zidingyizhifuBean in statisticsVM.zidingyizhifu)
            {
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyiname, true, 15, false));
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyinums, false, 8, true));
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyiamount, false, 8, true));
                sbtb.Append("\n");
            }
            sbtb.Append(tableFormat("合计", true, 15, false));
            sbtb.Append(tableFormat("", false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.subtotal, false, 8, true));
            sbtb.Append("\n\n\n");
            mPrinter.PrintString(sbtb.ToString());
            mPrinter.PrintString("\n\n\n\n\n");
        }
        #endregion

        #region printnote port
        internal static void printNote(StatisticsVM statisticsVM, serialPort mPrinter)
        {
            if (statisticsVM == null)
            {
                return;
            }
            if (mPrinter == null)
            {
                return;
            }
            if (mPrinter.pageWidth == "small")
            {
                mPrinter.PrintString("             收银对账             \n");
            }
            else
            {
                mPrinter.PrintString("                      收银对账                    \n");
            }
            StringBuilder sb = new StringBuilder();
            if (mPrinter.pageWidth == "small")
            {
                sb.Append("------------------------------\n");
            }
            else
            {
                sb.Append("----------------------------------------------\n");
            }
            sb.Append(Resources.R.branch_name + statisticsVM.branchname + "\n");
            sb.Append(Resources.R.time + DateTime.Now.ToString("D") + "\n");
            sb.Append(Resources.R.date_time + statisticsVM.starttime + "至" + statisticsVM.endtime + "\n");
            sb.Append(Resources.R.shop_cashier_num + statisticsVM.cashiername + "\n");
            sb.Append(Resources.R.ticketnums + statisticsVM.ticketnums + "\n");
            sb.Append(Resources.R.ticketamount + statisticsVM.ticketamount + "\n");
            sb.Append(Resources.R.returnnums + statisticsVM.returnnums + "\n");
            sb.Append(Resources.R.returnamount + statisticsVM.returnamount + "\n");
            sb.Append(Resources.R.rechargeamount + statisticsVM.rechargeamount + "\n");
            sb.Append(Resources.R.subtotal + statisticsVM.subtotal + "\n");
            if (mPrinter.pageWidth == "small")
            {
                sb.Append("------------------------------\n");
            }
            else
            {
                sb.Append("----------------------------------------------\n");
            }
            mPrinter.PrintString(sb.ToString());

            StringBuilder sbtb = new StringBuilder();

            sbtb.Append(tableFormat(Resources.R.payType, true, 15, false));
            sbtb.Append(tableFormat(Resources.R.mumber, false, 8, false));
            sbtb.Append(tableFormat(Resources.R.money, false, 8, false));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("现金", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.cashnums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.cashamount, false, 8, true));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("支付宝", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.alinums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.aliamount, false, 8, true));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("微信", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.wxnums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.wxamount, false, 8, true));
            sbtb.Append("\n");

            foreach (ZidingyizhifuBean zidingyizhifuBean in statisticsVM.zidingyizhifu)
            {
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyiname, true, 15, false));
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyinums, false, 8, true));
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyiamount, false, 8, true));
                sbtb.Append("\n");
            }
            sbtb.Append(tableFormat("合计", true, 15, false));
            sbtb.Append(tableFormat("", false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.subtotal, false, 8, true));
            sbtb.Append("\n\n\n");
            mPrinter.PrintString(sbtb.ToString());
            mPrinter.PrintString("\n\n\n\n\n");

        }
        #endregion

        #region printnote lpt
        internal static void printNote(StatisticsVM statisticsVM, LPTPrinter mPrinter)
        {
            if (statisticsVM == null)
            {
                return;
            }
            if (mPrinter == null)
            {
                return;
            }
            if (mPrinter.pageWidth == "small")
            {
                mPrinter.PrintString("             收银对账             \n");
            }
            else
            {
                mPrinter.PrintString("                      收银对账                    \n");
            }
            StringBuilder sb = new StringBuilder();
            if (mPrinter.pageWidth == "small")
            {
                sb.Append("------------------------------\n");
            }
            else
            {
                sb.Append("----------------------------------------------\n");
            }
            sb.Append(Resources.R.branch_name + statisticsVM.branchname + "\n");
            sb.Append(Resources.R.time + DateTime.Now.ToString("D") + "\n");
            sb.Append(Resources.R.date_time + statisticsVM.starttime + "至" + statisticsVM.endtime + "\n");
            sb.Append(Resources.R.shop_cashier_num + statisticsVM.cashiername + "\n");
            sb.Append(Resources.R.ticketnums + statisticsVM.ticketnums + "\n");
            sb.Append(Resources.R.ticketamount + statisticsVM.ticketamount + "\n");
            sb.Append(Resources.R.returnnums + statisticsVM.returnnums + "\n");
            sb.Append(Resources.R.returnamount + statisticsVM.returnamount + "\n");
            sb.Append(Resources.R.rechargeamount + statisticsVM.rechargeamount + "\n");
            sb.Append(Resources.R.subtotal + statisticsVM.subtotal + "\n");
            if (mPrinter.pageWidth == "small")
            {
                sb.Append("------------------------------\n");
            }
            else
            {
                sb.Append("----------------------------------------------\n");
            }
            mPrinter.PrintString(sb.ToString());

            StringBuilder sbtb = new StringBuilder();

            sbtb.Append(tableFormat(Resources.R.payType, true, 15, false));
            sbtb.Append(tableFormat(Resources.R.mumber, false, 8, false));
            sbtb.Append(tableFormat(Resources.R.money, false, 8, false));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("现金", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.cashnums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.cashamount, false, 8, true));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("支付宝", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.alinums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.aliamount, false, 8, true));
            sbtb.Append("\n");
            sbtb.Append(tableFormat("微信", true, 15, false));
            sbtb.Append(tableFormat(statisticsVM.wxnums, false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.wxamount, false, 8, true));
            sbtb.Append("\n");

            foreach (ZidingyizhifuBean zidingyizhifuBean in statisticsVM.zidingyizhifu)
            {
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyiname, true, 15, false));
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyinums, false, 8, true));
                sbtb.Append(tableFormat(zidingyizhifuBean.zidingyiamount, false, 8, true));
                sbtb.Append("\n");
            }
            sbtb.Append(tableFormat("合计", true, 15, false));
            sbtb.Append(tableFormat("", false, 8, true));
            sbtb.Append(tableFormat(statisticsVM.subtotal, false, 8, true));
            sbtb.Append("\n\n\n");
            mPrinter.PrintString(sbtb.ToString());
            mPrinter.PrintString("\n\n\n\n\n");

        }
        #endregion

        #region tableFormat
        public static string tableFormat(string text, bool left, int number, bool isNumber)
        {
            StringBuilder stringBuffer = new StringBuilder();
            int length = text.Length;
            if (!isNumber)
            {//一个汉字占两个字符
                length = length * 2;
            }
            if (left)
            {
                if (length > number)
                {
                    if (isNumber)
                    {
                        stringBuffer.Append(text.Substring(0, number));
                    }
                    else
                    {
                        stringBuffer.Append(text.Substring(0, number / 2));
                    }
                }
                else
                {
                    stringBuffer.Append(text);
                    for (int i = 0; i < number - length; i++)
                    {
                        stringBuffer.Append(" ");
                    }
                }
            }
            else
            {
                if (length > number)
                {
                    if (isNumber)
                    {
                        stringBuffer.Append(text.Substring(0, number));
                    }
                    else
                    {
                        stringBuffer.Append(text.Substring(0, number / 2));
                    }
                }
                else
                {
                    stringBuffer.Append(text);
                    for (int i = 0; i < number - length; i++)
                    {
                        stringBuffer.Insert(0, " ");
                    }
                }
            }
            return stringBuffer.ToString();
        }



        #endregion
    }
}
