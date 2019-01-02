using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using ZlPos.Bizlogic;
using ZlPos.Models;

namespace ZlPos.Dao
{
    class DbHelper
    {
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string _DBNAME = "zlCloudPos.db";

        private string _DBPATH = System.AppDomain.CurrentDomain.BaseDirectory + "DataBase\\";

        private string _DBFULLNAME = System.AppDomain.CurrentDomain.BaseDirectory + "DataBase\\zlCloudPos.db";

        private string _DBCONFIGFILE = System.AppDomain.CurrentDomain.BaseDirectory + "DataBase\\DBConfig.xml";

        private static DbHelper _DBUpdateChecking = null;

        private static object obj = new object();

        private DbHelper()
        {

        }

        public static DbHelper Instance
        {
            get
            {
                if (_DBUpdateChecking == null)
                {
                    lock (obj)
                    {
                        _DBUpdateChecking = new DbHelper();
                    }
                }
                return _DBUpdateChecking;
            }
        }

        public void Init()
        {
            //判断下是否含有db文件
            if (!DBExist())
            {
                //生成数据库db文件
                File.Create(_DBFULLNAME).Close();

                //生成数据库配置文件
                CreateConfig();
            }
            //已存在db 
            else
            {
                //根据配置文件判断是否要进行数据结构升级
                UpgradeDatebase();
            }
        }

        private void UpgradeDatebase()
        {
            //本机版本
            int oldVersion = GetCurrentDatabaseVersion();
            //当前要升级的版本
            int currentVersion = AppContext.Instance.DatebaseVersion;


            while (oldVersion < currentVersion)
            {
                logger.Info("Upgrading schema from version " + oldVersion + " to " + (oldVersion + 1) + " by dropping all tables");
                oldVersion++;
                switch (oldVersion)
                {
                    case 2:
                        //UpgradingSchema.UpgradingVersion2();
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "validtime" });
                        break;
                    case 3:
                        //UpgradingSchema.UpgradingVersion3();
                        UpgradingSchema.UpgradingVersion<ContextEntity>(new string[] { "barcodeStyle" });
                        break;
                    case 4:
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "commission" });
                        break;
                    case 5:
                        UpgradingSchema.UpgradingBarCodeEntity2();
                        break;
                    case 6:
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "plu" });
                        break;
                    case 7:
                        UpgradingSchema.UpgradingVersion<CategoryEntity>(new string[] { "createtime", "createuser", "updatetime", "updateuser" });
                        UpgradingSchema.UpgradingVersion<CashierEntity>(new string[] { "createtime", "updatetime" });
                        UpgradingSchema.UpgradingVersion<SupplierEntity>(new string[] { "commissionrate", "areaname", "areacode", "createtime", "flag", "required" });
                        UpgradingSchema.UpgradingVersion<PayTypeEntity>(new string[] { "createtime" });
                        UpgradingSchema.UpgradingVersion<ShopConfigEntity>(new string[] { "industryname", "industryid", "softwaretype", "expirestime", "contactaddress", "isbranchpay", "isrecharge", "payway", "expirestimestr" });
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "updatetime", "createuser", "storagetype", "stylecode", "ordernum", "createtime", "dishid" });
                        break;
                    case 8:
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "barcodes" });
                        break;
                    case 9:
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "updateuser" });
                        UpgradingSchema.UpgradingVersion<ContextEntity>(new string[] { "BJQprint" });
                        break;
                    case 10:
                        UpgradingSchema.UpgradingVersion<PayDetailEntity>(new string[] { "createtime" });
                        break;
                    case 11:
                        UpgradingSchema.UpgradingVersion<PrinterConfigEntity>(new string[] { "printerName" });
                        break;
                    case 12:
                        //add 2018/10/12
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "spucode" });
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "commoditylevel" });
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "speccode" });
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "specvalue" });
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "season" });

                        //add 2018/10/15
                        UpgradingSchema.UpgradingVersion<PayDetailEntity>(new string[] { "apiordercode" });

                        //add 2018/10/16
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "spucode" });
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "commoditylevel" });
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "speccode" });
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "specvalue" });
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "season" });
                        break;
                    case 13:
                        //add 2018年10月18日 这个正式版 并到12上去
                        UpgradingSchema.UpgradingVersion<BillEntity>(new string[] { "whichversion" });
                        //直接让测试卸载重装
                        UpgradingSchema.UpgradingVersion<CommodityPriceEntity>(new string[] { "spucode" });
                        UpgradingSchema.UpgradingVersion<CommodityPriceEntity>(new string[] { "commoditylevel" });
                        break;
                    case 14:
                        UpgradingSchema.UpgradingVersion<BarcodeScaleEntity>(new string[] { "brand" });
                        break;
                    case 15:
                        UpgradingSchema.UpgradingVersion<ContextEntity>(new string[] { "SPBQTemplet", "DDBQTemplet", "BJQTemplet" });
                        UpgradingSchema.UpgradingVersion<BillEntity>(new string[] { "cashtocard", "molinmoney" });
                        UpgradingSchema.UpgradingVersion<DisCountDetailEntity>(new string[] { "saleactivitytypecode", "activitycode" });
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "molinmoney" });
                        break;
                    case 16:
                        //add 2018年11月15日
                        UpgradingSchema.UpgradingVersion<CommodityEntity>(new string[] { "branchname", "required", "remark", "speclevel", "updownstatus", "datalevel" });
                        UpgradingSchema.UpgradingVersion<BarCodeEntity2>(new string[] { "branchcode", "branchname", "datalevel", "spucode" });
                        UpgradingSchema.UpgradingVersion<PayTypeEntity>(new string[] { "ordernum" });
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "speclevel", "mnemonic" });//1120 增加了一个mnemonic
                        //add 2018年11月28日
                        UpgradingSchema.UpgradingVersion<ContextEntity>(new string[] { "barcodeScale" });
                        break;
                    case 17:
                        //1119版本为了兼容 需要删库
                        UpgradingSchema.DeleteTable(new string[] { "CategoryEntity", "CommodityEntity", "MemberEntity", "PayTypeEntity", "AssistantsEntity", "CashierEntity", "SupplierEntity", "BarCodeEntity2", "CommodityPriceEntity", "CommodityInfoVM" });

                        break;
                    case 18:
                        //紧急修复字段
                        UpgradingSchema.FixColumns<BillEntity>(new string[] { "cashtocard" });
                        break;
                    case 19:
                        UpgradingSchema.UpgradingVersion<ContextEntity>(new string[] { "PrintModelSetting" });
                        break;
                    case 20:
                        UpgradingSchema.UpgradingVersion<BillCommodityEntity>(new string[] { "skucode", "category1code", "category2code", "category3code", "category4code", "category1name", "category2name", "category3name", "category4name", "speccode01", "specname01", "specvalue01", "speccode02", "specname02", "specvalue02", "speccode03", "specname03", "specvalue03", "memberpricelv1", "memberpricelv2", "memberpricelv3", "memberpricelv4", "memberpricelv5", "memberpricelv6" });
                        UpgradingSchema.UpgradingVersion<BillEntity>(new string[] { "consumecreditmoney" });
                        UpgradingSchema.UpgradingVersion<DisCountDetailEntity>(new string[] { "usercouponcode" });
                        UpgradingSchema.UpgradingVersion<MemberEntity>(new string[] { "levelcode" });
                        //变化大的直接删表吧
                        UpgradingSchema.DeleteTable(new string[] { "BarCodeEntity", "CategoryEntity", "CommodityInfoVM" });

                        break;

                }
                //生成数据库配置文件
                CreateConfig();
            }
        }

        private int GetCurrentDatabaseVersion()
        {
            string oldVersion = "";
            if (File.Exists(_DBCONFIGFILE))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_DBCONFIGFILE);

                XmlNode configuration = xmlDoc.SelectSingleNode("configuration");

                XmlNode setting = configuration.SelectSingleNode("setting");

                XmlElement xe = (XmlElement)setting;

                oldVersion = xe.GetAttribute("version");
            }

            //默认version = 1
            return "".Equals(oldVersion) ? 1 : Convert.ToInt32(oldVersion);
        }

        private void CreateConfig()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlSM = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);

            XmlElement configuration = xmlDoc.CreateElement("configuration");
            xmlDoc.AppendChild(configuration);

            XmlElement setting = xmlDoc.CreateElement("setting");
            setting.SetAttribute("version", AppContext.Instance.DatebaseVersion.ToString());

            configuration.AppendChild(setting);

            xmlDoc.Save(_DBCONFIGFILE);
        }

        private void CheckingDBVersion()
        {
            throw new NotImplementedException();
        }

        private bool DBExist()
        {
            if (File.Exists(_DBFULLNAME))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
