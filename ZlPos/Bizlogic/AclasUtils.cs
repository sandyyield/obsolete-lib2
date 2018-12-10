using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ZlPos.Bizlogic
{
    public class AclasUtils
    {
        const string LibraryName = "Library\\Aclas\\AclasSDK.dll";
        // Success
        const int ASSDK_Err_Success = 0x0000;
        // Progress
        const int ASSDK_Err_Progress = 0x0001;
        // Terminate by hand
        const int ASSDK_Err_Terminate = 0x0002;

        // ProtocolType
        const int ASSDK_ProtocolType_None = 0;
        const int ASSDK_ProtocolType_Pecr = 1;
        const int ASSDK_ProtocolType_Hecr = 2;
        const int ASSDK_ProtocolType_TSecr = 3;

        // ProcType
        const int ASSDK_ProcType_Down = 0;
        const int ASSDK_ProcType_UP = 1;
        const int ASSDK_ProcType_Edit = 2;
        const int ASSDK_ProcType_Del = 3;
        const int ASSDK_ProcType_List = 4;
        const int ASSDK_ProcType_Empty = 5;
        const int ASSDK_ProcType_Reserve = 0x0010;

        // DataType
        const int ASSDK_DataType_PLU = 0x0000;
        const int ASSDK_DataType_Unit = 0x0001;
        const int ASSDK_DataType_Department = 0x0002;
        const int ASSDK_DataType_HotKey = 0x0003;
        const int ASSDK_DataType_Group = 0x0004;
        const int ASSDK_DataType_Discount = 0x0005;
        const int ASSDK_DataType_Origin = 0x0006;
        const int ASSDK_DataType_Country = 0x0007;
        const int ASSDK_DataType_SlaughterHouse = 0x0008;
        const int ASSDK_DataType_Cuttinghall = 0x0009;
        const int ASSDK_DataType_Tare = 0x000A;
        const int ASSDK_DataType_Nutrition = 0x000B;
        const int ASSDK_DataType_Note1 = 0x000C;
        const int ASSDK_DataType_Note2 = 0x000D;
        const int ASSDK_DataType_Note3 = 0x000E;
        //const int ASSDK_DataType_TextMessage = 0x000F;
        const int ASSDK_DataType_Options = 0x0010;
        const int ASSDK_DataType_CustomBarcode = 0x0011;
        const int ASSDK_DataType_LabelPrintRecord = 0x0012;
        const int ASSDK_DataType_HeaderInfo = 0x0013;
        const int ASSDK_DataType_FooterInfo = 0x0014;
        const int ASSDK_DataType_AdvertisementInfo = 0x0015;
        const int ASSDK_DataType_HeaderLogo = 0x0016;
        const int ASSDK_DataType_FooterLogo = 0x0017;
        const int ASSDK_DataType_LabelAdvertisement = 0x0018;
        const int ASSDK_DataType_VendorInfo = 0x0019;
        const int ASSDK_DataType_NutritionElement = 0x001A;
        const int ASSDK_DataType_NutritionInfo = 0x001B;
        const int ASSDK_DataType_Note4 = 0x001C;


        // DeviceInfo
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        public struct TASSDKDeviceInfo
        {
            public UInt32 ProtocolType; // ProtocolType
            public UInt32 Addr;
            public UInt32 Port;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Name;
            public UInt32 ID;
            public UInt32 Version;
            public Byte Country;
            public Byte DepartmentID;
            public Byte KeyType;
            public UInt64 PrinterDot;
            public long PrnStartDate;
            public UInt32 LabelPage;
            public UInt32 PrinterNo;
            public UInt16 PLUStorage;
            public UInt16 HotKeyCount;
            public UInt16 NutritionStorage;
            public UInt16 DiscountStorage;
            public UInt16 Note1Storage;
            public UInt16 Note2Storage;
            public UInt16 Note3Storage;
            public UInt16 Note4Storage;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] FirmwareVersion;//固件版本
            public Byte DefaultProtocol;//默认协议            
            public Byte LFCodeLen;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DeviceId; //档口号
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] StockId;//门店号
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 155)]
            public byte[] Adjunct;// 保留参数
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void TASSDKOnProgressEvent(uint nErrorCode, uint Index, uint Total, IntPtr lpUserData);

        [DllImport(LibraryName)]
        static public extern Boolean AclasSDK_Initialize(Pointer Adjuct = null);

        [DllImport(LibraryName)]
        static public extern void AclasSDK_Finalize();
        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        static public extern Boolean AclasSDK_GetDevicesInfo(UInt32 Addr, UInt32 Port, UInt32 ProtocolType, ref TASSDKDeviceInfo DeviceInfo);
        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        static public extern int AclasSDK_GetNetworkSectionDevicesInfo(UInt32 Addr, UInt32 Port, UInt32 ProtocolType,
            IntPtr lpDeviceInfos, UInt32 dwCount);
        [DllImport(LibraryName)]
        static public extern IntPtr AclasSDK_ExecTaskA(UInt32 Addr, UInt32 Port, UInt32 ProtocolType, UInt32 ProcType, UInt32 DataType,
            string FileName, TASSDKOnProgressEvent OnProgress, Pointer lpUserData);
        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall)]
        static public extern IntPtr AclasSDK_ExecTask(UInt32 Addr, UInt32 Port, UInt32 ProtocolType, UInt32 ProcType, UInt32 DataType,
            string FileName, TASSDKOnProgressEvent OnProgress, Pointer lpUserData);
        [DllImport(LibraryName)]
        static public extern IntPtr AclasSDK_ExecTaskW(UInt32 Addr, UInt32 Port, UInt32 ProtocolType, UInt32 ProcType, UInt32 DataType,
            string FileName, TASSDKOnProgressEvent OnProgress, Pointer lpUserData);
        [DllImport(LibraryName)]
        static public extern int AclasSDK_GetLastTaskError();
        [DllImport(LibraryName)]
        static public extern void AclasSDK_StopTask(IntPtr TaskHandle);
        [DllImport(LibraryName)]
        static public extern void AclasSDK_WaitForTask(IntPtr TaskHandle);


        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string ip { get; set; }
        public string port { get; set; }
        public string TaskPath { get; set; }
        public string OutputFile { get; set; }

        public string TaskID { get; set; }

        //查询执行结果用
        private string CommandID = "";

        public bool ClearData { get; set; }

        public string filePath { get; set; }

        public void BuildTask(string guid)
        {

            TaskID = guid;


        }

        byte tab = 0x09;

        byte[] newLine = new byte[] { 0x0d, 0x0a };


        public void BuildData()//string PLU, string commodityName, string price, string indate, string tare)
        {
            //XDocument DataXml = new XDocument();
            //DataXml.Add(new XElement("Data"));//, GetItem(PLU: PLU, commodityName: commodityName, price: price, indate: indate, tare: tare)));
            //DataXml.Save(TaskPath + "\\Data.xml");

            filePath = Path.Combine(TaskPath, TaskID + ".dls");

            try
            {
                using (var fs = new FileStream(filePath, FileMode.CreateNew))
                {
                    using (var sw = new StreamWriter(fs))
                    {

                        sw.Write("ID" //PLU
                            + tab + "ItemCode"      //barcode  str android  据说这两个
                            + tab + "DepartmentID" //直接写死20 int 
                                                   //+ tab + "GroupID"   //此字段对触控称是必须的，对普通按键式标签称可不包含此字段 故这里先注释
                            + tab + "Name1"  //commodityName str
                            + tab + "Price"     //price float
                            + tab + "UnitID"    //4-kg 记重  ; 10-pcs 计数 int
                            + tab + "BarcodeType1"  // 定死46 int
                            + tab + "ValidDate"     //indate  int
                            + tab + "Flag1"         //写死 0x3c byte
                            + tab + "Flag2"         //写死 0xf0 byte
                            + tab + "TareValue"     // 皮重
                            );
                        sw.Write(newLine);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("BuildData err", e);
            }
        }

        public void AddData(string PLU, string commodityName, string price, string indate, string tare, string barcode, string type)
        {
            if (string.IsNullOrEmpty(tare))
            {
                tare = "0";
            }
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Append))
                {
                    using(var sw = new StreamWriter(fs))
                    {
                        //sw.Write(barcode)
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("AddData err", e);
            }
        }
    }
}
