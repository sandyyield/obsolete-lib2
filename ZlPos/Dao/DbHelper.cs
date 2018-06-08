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
                        UpgradingSchema.UpgradingVersion2();
                        //生成数据库配置文件
                        CreateConfig();
                        break;

                }
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
