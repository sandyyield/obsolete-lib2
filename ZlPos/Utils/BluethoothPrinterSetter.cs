using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Bean;
using ZlPos.Bizlogic;
using ZlPos.Config;
using ZlPos.Enums;
using ZlPos.Models;
using ZlPos.PrintServices;

namespace ZlPos.Utils
{
    public class BluethoothPrinterSetter
    {
        private Action<object> listener;
        private PrinterConfigEntity printerConfigEntity;
        private ResponseEntity responseEntity;
        internal void setBluethoothPrinter(PrinterConfigEntity printerConfigEntity, Action<object> webCallback)
        {
            if (printerConfigEntity != null)
            {
                this.listener = webCallback;
                this.printerConfigEntity = printerConfigEntity;
                responseEntity = new ResponseEntity();
                if (PrinterManager.Instance.BluetoothDeviceArrayList == null)
                {
                    getBluetoothDevices();
                }
                if (PrinterManager.Instance.BluetoothDeviceArrayList != null)
                {
                    if (!setBluetooth())
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "该设备不可用";
                        if (listener != null)
                        {
                            listener.Invoke(new object[] { "setPrinterCallBack", responseEntity });
                        }
                    }
                }
            }
            else
            {
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "参数不能为空";
                if (listener != null)
                {
                    listener.Invoke(new object[] { "setPrinterCallBack", responseEntity });
                }
            }

            //if (webCallback != null)
            //{
            //    webCallback.Invoke(new object[] { "setPrinterCallBack", responseEntity });
            //}
        }

        private bool setBluetooth()
        {
            List<BluetoothDeviceInfo> bluetoothDeviceArrayList = PrinterManager.Instance.BluetoothDeviceArrayList;
            foreach (BluetoothDeviceInfo bluetoothDevice in bluetoothDeviceArrayList)
            {
                if (bluetoothDevice.DeviceName.Equals(printerConfigEntity.deviceId))
                {
                    BluetoothPrinter bluetoothPrinter;
                    if (PrinterManager.Instance.Init && PrinterManager.Instance.BluetoothPrinter != null)
                    {
                        if (bluetoothDevice.DeviceAddress.Equals(PrinterManager.Instance.BluetoothPrinter.MacAddress))
                        {
                            bluetoothPrinter = PrinterManager.Instance.BluetoothPrinter;
                        }
                        else
                        {
                            PrinterManager.Instance.BluetoothPrinter.closeConnection();//关闭之前的蓝牙打印机
                            bluetoothPrinter = new BluetoothPrinter(bluetoothDevice);//创建新的蓝牙打印机实例
                        }

                    }
                    else
                    {
                        bluetoothPrinter = new BluetoothPrinter(bluetoothDevice);
                    }

                    bluetoothPrinter.Encoding = "GBK";
                    if (printerConfigEntity.pageWidth == "small")
                    {
                        bluetoothPrinter.CurrentPrintType = PrinterType.T8;
                    }
                    else
                    {
                        bluetoothPrinter.CurrentPrintType = PrinterType.T5;
                    }

                    //bluetoothPrinter.Handler(bluetoothHandler);
                    if (!bluetoothPrinter.isConnected())
                    {
                        bluetoothPrinter.openConnection();
                    }
                    else
                    {
                        //                    savePrinterConfig(printerConfigEntity);
                        //                    PrintUtils.printText(bluetoothPrinter);
                    }
                    PrinterManager.Instance.Init = true;
                    PrinterManager.Instance.PrinterTypeEnum = PrinterTypeEnum.bluetooth;
                    PrinterManager.Instance.BluetoothPrinter = bluetoothPrinter;
                    PrinterManager.Instance.PrinterConfigEntity = printerConfigEntity;
                    bluetoothPrinter.PrintString("蓝牙打印机连接成功\n\n\n\n\n");
                    responseEntity.code = ResponseCode.SUCCESS;
                    responseEntity.msg = "打印机设置成功";
                    if (listener != null)
                    {
                        listener.Invoke(new object[] { "setPrinterCallBack", responseEntity });
                    }
                    return true;
                }
            }
            return false;
        }

        private void getBluetoothDevices()
        {
            try
            {
                BluetoothRadio BuleRadio = BluetoothRadio.PrimaryRadio;
                BuleRadio.Mode = RadioMode.Connectable;

                BluetoothClient Blueclient = new BluetoothClient();
                Dictionary<string, BluetoothAddress> deviceAddresses = new Dictionary<string, BluetoothAddress>();

                BluetoothDeviceInfo[] Devices = Blueclient.DiscoverDevices();
                List<BluetoothDeviceInfo> bluetoothDeviceInfos = new List<BluetoothDeviceInfo>(Devices);

                PrinterManager.Instance.BluetoothDeviceArrayList = bluetoothDeviceInfos;
            }
            catch (Exception e)
            {
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "该设备不可用";
                //ExceptionUtils.capture(TAG, "getBluetoothDevices", "获取蓝牙设备", ErrorCode.ExceptionCode, e.getMessage(), "获取蓝牙设备,蓝牙异常", new JSBridge().getDeviceId(), LoginUserManager.getInstance().getUserEntity().getShopcode());

                if (listener != null)
                {
                    listener.Invoke(new object[] { "setPrinterCallBack", responseEntity });
                }
            }
        }


    }

}
