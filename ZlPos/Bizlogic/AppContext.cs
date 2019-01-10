using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ZlPos.Bizlogic
{
    public sealed class AppContext
    {
        private static AppContext _instance;

        private AppContext() { }

        public static AppContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppContext();
                    _instance.InitConfigParam();
                }
                return _instance;
            }
        }

        public string AppVersion { get => _AppVersion; set => _AppVersion = value; }
        public string AppName { get => _AppName; set => _AppName = value; }
        public int DatebaseVersion { get => _DatebaseVersion; set => _DatebaseVersion = value; }
        public string UpdateUrl { get => _UpdateUrl; set => _UpdateUrl = value; }
        public string XmlFile { get => _XmlFile; set => _XmlFile = value; }
        public bool Debug { get => _Debug; set => _Debug = value; }

        private string _AppName;

        private string _AppVersion;

        private int _DatebaseVersion;

        private string _UpdateUrl;

        private string _XmlFile;

        /// <summary>
        /// 是否debug模式
        /// </summary>
        private bool _Debug;


        private void InitConfigParam()
        {
            AppName = ConfigurationManager.AppSettings["AppContextName"];

            AppVersion = ConfigurationManager.AppSettings["Version"];

            DatebaseVersion = Convert.ToInt32(ConfigurationManager.AppSettings["DatabaseVersion"]);

            //2019年1月9日  直接不分xp和win7
            UpdateUrl = ConfigurationManager.AppSettings["WIN7UpdateUrl"];
            //if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 1)
            //{
            //    UpdateUrl = ConfigurationManager.AppSettings["XPUpdateUrl"];
            //}

            XmlFile = ConfigurationManager.AppSettings["UpdateXmlFile"];

        }
    }
}
