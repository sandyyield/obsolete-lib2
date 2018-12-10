using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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

        public bool init { get; set; }

        public string TaskID { get; set; }

        //查询执行结果用
        private string CommandID = "";

        public bool ClearData { get; set; }

        public string filePath { get; set; }




        public void BuildTask(string guid)
        {

            TaskID = guid;

            //初始化 sdk
            init = AclasSDK_Initialize();


        }

        public void FinalizeSDK()
        {
            AclasSDK_Finalize();
        }

        uint iAddr;
        TASSDKDeviceInfo DeviceInfo = new TASSDKDeviceInfo();
        public bool GetDeviceInfo()
        {
            if (!string.IsNullOrEmpty(ip))
            {
                iAddr = MakeHostToDWord(ip);
                bool res = AclasSDK_GetDevicesInfo(iAddr, 0, ASSDK_ProtocolType_None, ref DeviceInfo);
                return res;
            }
            return false;
        }

        public void ExecTask()
        {
            TASSDKOnProgressEvent OnProgress = new TASSDKOnProgressEvent(OnProgressEvent);
            AclasSDK_WaitForTask(AclasSDK_ExecTask(DeviceInfo.Addr, DeviceInfo.Port, DeviceInfo.ProtocolType, ASSDK_ProcType_UP, ASSDK_DataType_PLU, Path.Combine(TaskPath, TaskID + ".txt"), OnProgress, null));
        }

        public void ClearPLU()
        {
            TASSDKOnProgressEvent OnProgress = new TASSDKOnProgressEvent(OnProgressEvent);
            AclasSDK_WaitForTask(AclasSDK_ExecTaskA(DeviceInfo.Addr, DeviceInfo.Port, DeviceInfo.ProtocolType,
                ASSDK_ProcType_Del, ASSDK_DataType_PLU, "*", OnProgress, null));
        }


        byte[] tab = new byte[] { 0x09 };

        byte[] newLine = new byte[] { 0x0d, 0x0a };


        public void BuildData()//string PLU, string commodityName, string price, string indate, string tare)
        {
            //XDocument DataXml = new XDocument();
            //DataXml.Add(new XElement("Data"));//, GetItem(PLU: PLU, commodityName: commodityName, price: price, indate: indate, tare: tare)));
            //DataXml.Save(TaskPath + "\\Data.xml");

            filePath = Path.Combine(TaskPath, TaskID + ".txt");

            try
            {
                using (var fs = new FileStream(filePath, FileMode.CreateNew))
                {
                    fs.Write(Encoding.Default.GetBytes("ID"), 0, Encoding.Default.GetBytes("ID").Length);//sw.Write("ID"); //PLU

                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("ItemCode"), 0, Encoding.Default.GetBytes("ItemCode").Length);//sw.Write("ItemCode")      //barcode  str android  据说这两个
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("DepartmentID"), 0, Encoding.Default.GetBytes("DepartmentID").Length);// "DepartmentID" //直接写死20 int 
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("Name1"), 0, Encoding.Default.GetBytes("Name1").Length);//+ "Name1"  //commodityName str
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("Price"), 0, Encoding.Default.GetBytes("Price").Length);//+ "Price"     //price float
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("UnitID"), 0, Encoding.Default.GetBytes("UnitID").Length);//+ "UnitID"    //4-kg 记重  ; 10-pcs 计数 int
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("BarcodeType1"), 0, Encoding.Default.GetBytes("BarcodeType1").Length);//+ "BarcodeType1"  // 定死46 int
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("ValidDate"), 0, Encoding.Default.GetBytes("ValidDate").Length);//+ "ValidDate"     //indate  int
                    fs.Write(tab, 0, 1);
                    //fs.Write(Encoding.Default.GetBytes("Flag1"), 0, Encoding.Default.GetBytes("Flag1").Length);//+ "Flag1"         //写死 0x3c byte
                    //fs.Write(tab, 0, 1);
                    //fs.Write(Encoding.Default.GetBytes("Flag2"), 0, Encoding.Default.GetBytes("Flag2").Length);//+ "Flag2"         //写死 0xf0 byte
                    //fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("TareValue"), 0, Encoding.Default.GetBytes("TareValue").Length);//+ "TareValue"     // 皮重
                                                                                                                       //);
                    fs.Write(newLine, 0, newLine.Length);



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

            string UnitID = "4";
            if (type == "0")
            {
                UnitID = "4";//"KGM";
            }
            else
            {
                UnitID = "10";//"PCS";
            }

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Append))
                {

                    //顶尖ID和itemcode最好一样 待验证
                    fs.Write(Encoding.Default.GetBytes(barcode), 0, Encoding.Default.GetBytes(barcode).Length); //barcode //PLU
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes(barcode), 0, Encoding.Default.GetBytes(barcode).Length);//+ barcode     //itemcode  str android  据说这两个
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("20"), 0, Encoding.Default.GetBytes("20").Length);//+ 20 //直接写死20 int 
                                                                                                         //+ tab + "GroupID"   //此字段对触控称是必须的，对普通按键式标签称可不包含此字段 故这里先注释
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes(commodityName), 0, Encoding.Default.GetBytes(commodityName).Length);//+ commodityName  //commodityName str
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes(price), 0, Encoding.Default.GetBytes(price).Length);//+ Convert.ToDouble(price)    //price float
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes(UnitID), 0, Encoding.Default.GetBytes(UnitID).Length);//+ UnitID    //4-kg 记重  ; 10-pcs 计数 int
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes("46"), 0, Encoding.Default.GetBytes("46").Length);//+ 46  // 定死46 int
                    fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes(indate), 0, Encoding.Default.GetBytes(indate).Length);//+ Convert.ToInt32(indate)    //indate  int
                    fs.Write(tab, 0, 1);
                    //fs.Write(new byte[] { 0x3c }, 0, 1);//+ 0x3c        //写死 0x3c byte
                    //fs.Write(tab, 0, 1);
                    //fs.Write(new byte[] { 0xf0 }, 0, 1);//+ 0xf0         //写死 0xf0 byte
                    //fs.Write(tab, 0, 1);
                    fs.Write(Encoding.Default.GetBytes(tare), 0, Encoding.Default.GetBytes(tare).Length);//+ Convert.ToInt32(tare)     // 皮重
                    fs.Write(newLine, 0, newLine.Length);
                    //using (var sw = new StreamWriter(fs))
                    //{
                    //    //顶尖ID和itemcode最好一样
                    //    sw.Write(barcode //PLU
                    //        + tab 
                    //        + barcode     //itemcode  str android  据说这两个
                    //        + tab
                    //        + 20 //直接写死20 int 
                    //                   //+ tab + "GroupID"   //此字段对触控称是必须的，对普通按键式标签称可不包含此字段 故这里先注释
                    //        + tab 
                    //        + commodityName  //commodityName str
                    //        + tab 
                    //        + Convert.ToDouble(price)    //price float
                    //        + tab
                    //        + UnitID    //4-kg 记重  ; 10-pcs 计数 int
                    //        + tab 
                    //        + 46  // 定死46 int
                    //        + tab
                    //        + Convert.ToInt32(indate)    //indate  int
                    //        + tab 
                    //        + 0x3c        //写死 0x3c byte
                    //        + tab 
                    //        + 0xf0         //写死 0xf0 byte
                    //        + tab 
                    //        + Convert.ToInt32(tare)     // 皮重
                    //        );
                    //    sw.Write(newLine);
                    //}
                }
            }
            catch (Exception e)
            {
                logger.Error("AddData err", e);
            }
        }



        public uint MakeHostToDWord(string sHost)
        {
            int i;
            string[] Segment;
            uint result;
            result = 0;

            Segment = sHost.Split('.');
            if (Segment.Length != 4)
                return result;
            for (i = 0; i < (Segment.Length); i++)
            {
                if ((Convert.ToUInt32(Segment[i]) >= 0) && (Convert.ToUInt32(Segment[i]) <= 255))
                {
                    result = result + Convert.ToUInt32(Convert.ToUInt32(Segment[i]) << ((3 - i) * 8));
                }
                else
                    return result;
            }
            return result;
        }

        public static void OnProgressEvent(UInt32 nErrorCode, UInt32 Index, UInt32 Total, IntPtr lpUserData)
        {
            const string sInfoProgress = "Progress: {0}/{1}";
            const string sInfoComplete = "Complete, Total: {0}";
            const string sInfoStop = "Proc Stop!";
            const string sInfoFailed = "Proc Failed!";

            switch (nErrorCode)
            {
                case ASSDK_Err_Success:
                    {
                        MessageBox.Show(string.Format(sInfoComplete, Total));
                        break;
                    }
                case ASSDK_Err_Progress:
                    {
                        //MessageBox.Show(string.Format(sInfoProgress, Index, Total));                        
                        break;
                    }
                case ASSDK_Err_Terminate:
                    {
                        MessageBox.Show(sInfoStop);
                        break;
                    }
                default:
                    MessageBox.Show(sInfoFailed);
                    break;
            }
        }
    }
}
