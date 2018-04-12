using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Models;

namespace ZlPos.PrintServices
{
    class USBPrinterService : IUSBPrinterService
    {
        public void Print(object state)
        {
            if (state == null)
            {
                return;
            }

            //IntPtr h_Printer = PrintBridge.POS_Port_OpenW("SP-USB1", 1002, false, null);

            //int i= PrintBridge.POS_Port_Close(h_Printer);

            //if (i == 0)
            //{
            //    Console.ReadKey();
            //}

            //IntPtr hPrinter = new IntPtr(h_Printer);



            IntPtr hUSB = PrintBridge.OpenUsb();
            int sendSize = 0;
            string teststr = "hello world\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            PrintBridge.WriteUsb(hUSB, teststr, Encoding.Unicode.GetByteCount(teststr), ref sendSize);



            PrintBridge.CloseUsb(hUSB);




            //string title = "";
            //string content = "";


            ////        Type stringObjectMap = new TypeToken<Map<String, Object>>() {
            ////        }.getType();
            //BillEntity billEntity = new BillEntity();
            //StringBuilder sb = new StringBuilder();

            //        usbPrinter.setPrinter(BluetoothPrinter.COMM_ALIGN, BluetoothPrinter.COMM_ALIGN_CENTER);
            //        usbPrinter.setCharacterMultiple(1, 1);
            //        usbPrinter.printText(billEntity.getBranchname() + "\n");

            //        usbPrinter.setPrinter(BluetoothPrinter.COMM_ALIGN, BluetoothPrinter.COMM_ALIGN_LEFT);
            //        usbPrinter.setCharacterMultiple(0, 0);

            //        sb.append(R.string.shop_num + billEntity.getBranchcode() + "   ");
            //        sb.append(R.string.shop_cashier_num + billEntity.getCashiername() + "\n");
            //        sb.append(R.string.shop_receipt_num + billEntity.getTicketcode() + "\n");

            //        sb.append(R.string.time + DateUtil.getTime(new Date()) + "\n");
            //        usbPrinter.printText(sb.toString());

            //        List<BillCommodityEntity> commoditys = billEntity.getCommoditys();
            //        List<PayDetailEntity> paydetails = billEntity.getPaydetails();


            //        if (commoditys != null && commoditys.size() > 0) {
            //            PrintUtils.printTable1(commoditys, usbPrinter); //打印表格
            //            double number = 0;
            //        float buyTotal = 0f;//原价总额
            //        float saleTotal = 0f;//售价总额
            //        float payTotal = 0f;//付款总额
            //            for (BillCommodityEntity billCommodityEntity : commoditys) {
            //                number = number + Double.valueOf(billCommodityEntity.getSalenums());
            ////                buyTotal = buyTotal + Float.valueOf(billCommodityEntity.getSaleprice()) * Integer.valueOf(billCommodityEntity.getSalenums());
            ////                saleTotal = saleTotal + Float.valueOf(billCommodityEntity.getSaleprice()) * Integer.valueOf(billCommodityEntity.getSalenums());
            //            }
            //    buyTotal = Float.valueOf(billEntity.getTotal());
            //            saleTotal = Float.valueOf(billEntity.getPaytotal());
            //            payTotal = Float.valueOf(billEntity.getCollection());
            ////            if (paydetails != null && paydetails.size() > 0) {
            ////                for (PayDetailEntity payDetailEntity : paydetails) {
            ////                    payTotal = payTotal + Float.valueOf(payDetailEntity.getPayamount());
            ////                }
            ////            }
            //            sb = new StringBuffer();
            //    sb.append(R.string.shop_goods_number + "  " + DateFormatUtil.parseDoubleString(number + "") + "\n");
            //            sb.append(R.string.shop_goods_buy_total_price + "  " + DateFormatUtil.parseDoubleString(buyTotal + "") + "\n");
            //            sb.append(R.string.shop_goods_sale_total_price + "  " + DateFormatUtil.parseDoubleString(saleTotal + "") + "\n");
            //            sb.append(R.string.shop_goods_sale_total_collection + "  " + DateFormatUtil.parseDoubleString(payTotal + "") + "\n");
            //            if ("2".equals(billEntity.getTradeid())) {
            //                sb.append(R.string.shop_goods_youhui + "  " + "0.00" + "\n");
            //            } else {
            //                sb.append(R.string.shop_goods_youhui + "  " + DateFormatUtil.parseDoubleString(buyTotal - saleTotal + "") + "\n");
            //            }
            //            sb.append(R.string.shop_payment + "  ");
            //            for (int i = 0; i<billEntity.getPaydetails().size(); i++) {
            //                PayDetailEntity payDetailEntity = billEntity.getPaydetails().get(i);
            //                if (i > 0) {
            //                    sb.append("   " + payDetailEntity.getPayname() + "  " + DateFormatUtil.parseDoubleString(payDetailEntity.getPayamount()) + "\n");

            //                } else {
            //                    sb.append(payDetailEntity.getPayname() + "  " + DateFormatUtil.parseDoubleString(payDetailEntity.getPayamount()) + "\n");
            //                }
            //            }
            //            if ("2".equals(billEntity.getTradeid())) {
            //                sb.append(R.string.shop_change + "  " + "0.00" + "\n");
            //            } else {
            //                sb.append(R.string.shop_change + "  " + DateFormatUtil.parseDoubleString(payTotal - saleTotal + "") + "\n");
            //            }
            ////            if (mPrinter.getCurrentPrintType() == com.printer.sdk.api.PrinterType.TIII || mPrinter.getCurrentPrintType() == com.printer.sdk.api.PrinterType.T5) {
            ////                sb.append(resources.getString(R.string.shop_goods_number) + "  " + DateFormatUtil.parseDoubleString(number + "") + "\n");
            ////                sb.append(resources.getString(R.string.shop_goods_buy_total_price) + "  " + DateFormatUtil.parseDoubleString(buyTotal + "") + "\n");
            ////                sb.append(resources.getString(R.string.shop_goods_sale_total_price) + "  " + DateFormatUtil.parseDoubleString(saleTotal + "") + "\n");
            ////                sb.append(resources.getString(R.string.shop_goods_youhui) + "  " + DateFormatUtil.parseDoubleString(buyTotal - saleTotal + "") + "\n");
            ////                sb.append(resources.getString(R.string.shop_payment) + "  ");
            ////                for (int i = 0; i < billEntity.getPaydetails().size(); i++) {
            ////                    PayDetailEntity payDetailEntity = billEntity.getPaydetails().get(i);
            ////                    if (i > 0) {
            ////                        sb.append("   " + DateFormatUtil.parseDoubleString(payDetailEntity.getPayname() + "  " + payDetailEntity.getPayamount()) + "\n");
            ////
            ////                    } else {
            ////                        sb.append(DateFormatUtil.parseDoubleString(payDetailEntity.getPayname() + "  " + payDetailEntity.getPayamount()) + "\n");
            ////                    }
            ////                }
            ////                sb.append(resources.getString(R.string.shop_change) + "  " + DateFormatUtil.parseDoubleString(payTotal - saleTotal + "") + "\n");
            ////            } else {
            ////                sb.append(resources.getString(R.string.shop_goods_number) + "                                " + DateFormatUtil.parseDoubleString(number + "") + "\n");
            ////                sb.append(resources.getString(R.string.shop_goods_buy_total_price) + "                                " + DateFormatUtil.parseDoubleString(buyTotal + "") + "\n");
            ////                sb.append(resources.getString(R.string.shop_goods_sale_total_price) + "                                " + DateFormatUtil.parseDoubleString(saleTotal + "") + "\n");
            ////                sb.append(resources.getString(R.string.shop_goods_youhui) + "                                " + DateFormatUtil.parseDoubleString(buyTotal - saleTotal + "") + "\n");
            ////
            ////                sb.append(resources.getString(R.string.shop_payment) + "                                " + DateFormatUtil.parseDoubleString(payTotal + "") + "\n");
            ////                sb.append(resources.getString(R.string.shop_change) + "                                " + DateFormatUtil.parseDoubleString(payTotal - saleTotal + "") + "\n");
            ////            }

            ////            sb.append(resources.getString(R.string.shop_company_name) + "\n");
            ////            sb.append(resources.getString(R.string.shop_company_site) + "www.jiangsu1510.com\n");
            ////            sb.append(resources.getString(R.string.shop_company_address) + "\n");
            ////            sb.append(resources.getString(R.string.shop_company_tel) + "0574-88222999\n");
            ////            sb.append(resources.getString(R.string.shop_Service_Line) + "4008-567-567 \n");
            //            if (usbPrinter.getCurrentPrintType() == PrinterType.TIII || usbPrinter.getCurrentPrintType() == PrinterType.T5) {
            //                sb.append("------------------------------\n");
            //            } else {
            //                sb.append("----------------------------------------------\n");
            //            }
            //            usbPrinter.printText(sb.toString());

            //            if (!StringUtil.isEmpty(billEntity.getMemberid())) {
            //                StringBuffer memSb = new StringBuffer();
            //memSb.append(R.string.member_name + billEntity.getMembername());
            //                memSb.append("\n");
            //                memSb.append(R.string.member_code + billEntity.getMembercode());
            //                memSb.append("\n");
            //                memSb.append(R.string.balance + billEntity.getMemberacount());
            //                memSb.append("  ");
            //                memSb.append(R.string.integral + billEntity.getMembercredit());
            //                memSb.append("\n");
            //                if (usbPrinter.getCurrentPrintType() == PrinterType.TIII || usbPrinter.getCurrentPrintType() == PrinterType.T5) {
            //                    memSb.append("------------------------------\n");
            //                } else {
            //                    memSb.append("----------------------------------------------\n");
            //                }
            //                usbPrinter.printText(memSb.toString());
            //            }

            //            usbPrinter.setPrinter(BluetoothPrinter.COMM_ALIGN, BluetoothPrinter.COMM_ALIGN_CENTER);
            //            usbPrinter.setCharacterMultiple(0, 1);
            //            usbPrinter.printText(R.string.shop_thanks + "\n\n\n\n\n\n");
            //            usbPrinter.setPrinter(BluetoothPrinter.COMM_ALIGN, BluetoothPrinter.COMM_ALIGN_LEFT);
            //            usbPrinter.setCharacterMultiple(0, 0);
            ////            mPrinter.printText(resources.getString(R.string.shop_demo) + "\n\n\n");
            //        }

        }
    }
}
