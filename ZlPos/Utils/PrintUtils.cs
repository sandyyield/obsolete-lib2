using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Enums;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    public class PrintUtils
    {
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
                    usbPrinter.PrintString(printEntities[i].content);
                }
            }
            usbPrinter.PrintString("\n\n\n\n\n\n");
            //usbPrinter.PrintString(content);
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
                    bluetoothPrinter.PrintString(printEntities[i].content);
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
                    portPrinter.PrintString(printEntities[i].content);
                }
            }
            portPrinter.PrintString("\n\n\n\n\n\n");
            //portPrinter.Write(content);
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
