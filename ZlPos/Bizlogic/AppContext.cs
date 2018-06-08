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

        private string _AppName;

        private string _AppVersion;

        private int _DatebaseVersion;

        private void InitConfigParam()
        {
            AppName = ConfigurationManager.AppSettings["AppContextName"];

            AppVersion = ConfigurationManager.AppSettings["Version"];

            DatebaseVersion = Convert.ToInt32(ConfigurationManager.AppSettings["DatabaseVersion"]);

        }
    }
}
