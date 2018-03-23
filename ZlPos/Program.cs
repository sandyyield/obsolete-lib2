using CefSharp;
using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZlPos.Bizlogic;
using ZlPos.Forms;

namespace ZlPos
{
    static class Program
    {
        private static ILog logger = null;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            InitLog4netCfg(Level.Error);
#else
            InitLog4netCfg(Level.Info);
#endif
            logger = LogManager.GetLogger("Logger");

            logger.Info("Initiallize chromium core..");

            Cef.Initialize(new CefSettings(), true, true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            logger.Info("Start run " + AppContext.Instance.AppName + " services");
            Application.Run(new PosForm());
        }

        private static void InitLog4netCfg(Level level)
        {
            log4net.Repository.Hierarchy.Hierarchy h = (log4net.Repository.Hierarchy.Hierarchy)LogManager.CreateRepository("Rep_" + AppContext.Instance.AppName);

            log4net.Appender.RollingFileAppender appender1 = new log4net.Appender.RollingFileAppender
            {
                Name = "App_File_" + AppContext.Instance.AppName,
                AppendToFile = true,
                File = string.Format("LogFiles/"),
                ImmediateFlush = true,
                MaxSizeRollBackups = 90,
                StaticLogFileName = false,
                DatePattern = "yyyyMMdd'.log'",
                RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date
            };

            log4net.Layout.PatternLayout lay = new log4net.Layout.PatternLayout
            {
                //%d [%t] %-5p %c  - %m%n
                ConversionPattern = "%d %-5p - %m%n"
            };
            lay.ActivateOptions();
            appender1.Layout = lay;

            appender1.Encoding = Encoding.UTF8;

            appender1.ActivateOptions();

            h.Root.Level = level;

            h.Root.AddAppender(appender1);

            h.Configured = true;

            log4net.Config.BasicConfigurator.Configure(appender1);
        }

        
    }
}
