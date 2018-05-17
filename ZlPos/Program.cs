using CefSharp;
using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZlPos.Bizlogic;
using ZlPos.Forms;

namespace ZlPos
{
    static class Program
    {
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Mutex mutex { get; set; }

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
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            logger = LogManager.GetLogger("Logger");

            logger.Info("Initiallize chromium core..");

            #region " 不允许多个实例运行 "
            mutex = new System.Threading.Mutex(true, "aabbccdd");
            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show("程序已经在运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
                return;
            }
            #endregion

            CefSettings cefSettings = new CefSettings();
            //禁用调试日志
            cefSettings.LogSeverity = LogSeverity.Disable;

            Cef.Initialize(cefSettings, true, true);

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                //设置所有未觉察异常被觉察
                e.SetObserved();
            };

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

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (e.Exception is DeException)
            {
                DeException dx = e.Exception as DeException;

                if (logger != null)
                    logger.Error("Application_ThreadException,errorcode is " + dx.ErrorCode, dx.InnerException);

                MessageBox.Show(String.Format("错误编码{0},{1}---{2}", dx.ErrorCode, dx.Message, dx.StackTrace));
            }
            else
            {
                if (logger != null)
                    logger.Error("Application_ThreadException", e.Exception);

                MessageBox.Show(e.Exception.Message);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is DeException)
            {
                DeException dx = e.ExceptionObject as DeException;

                if (logger != null)
                    logger.Error("Application_ThreadException,errorcode is " + dx.ErrorCode, dx.InnerException);

                if (!dx.ErrorCode.Equals(""))
                {
                    MessageBox.Show(String.Format("错误编码{0},{1}---{2}", dx.ErrorCode, dx.Message, dx.StackTrace));
                }
            }
            else
            {
                Exception ex = e.ExceptionObject as Exception;

                if (logger != null)
                    logger.Error("CurrentDomain_UnhandledException", ex);

                MessageBox.Show(ex.Message);
            }
        }


    }
}
