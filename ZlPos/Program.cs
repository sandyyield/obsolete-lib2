using CefSharp;
using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZlPos.Bizlogic;
using ZlPos.Dao;
using ZlPos.Forms;

namespace ZlPos
{
    static class Program
    {
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Mutex mutex { get; set; }

        public static EventWaitHandle ProgramStarted;
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

            #region "数据库兼容"
            //初始化
            DbHelper.Instance.Init();


            #endregion

            #region " 不允许多个实例运行 "
            //mutex = new System.Threading.Mutex(true, "aabbccdd");
            //if (!mutex.WaitOne(0, false))
            //{
            //    MessageBox.Show("程序已经在运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    Application.Exit();
            //    return;
            //}

            // 尝试创建一个命名事件  
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "MyStartEvent", out createNew);

            // 如果该命名事件已经存在(存在有前一个运行实例)，则发事件通知并退出  
            if (!createNew)
            {
                ProgramStarted.Set();
                return;
            }

            //Process instance = RunningInstance();
            //if(instance != null)
            //{
            //    HandleRunningInstance(instance);
            //    return;
            //}
            #endregion

            #region "升级"
            var updater = FSLib.App.SimpleUpdater.Updater.Instance;
            //当检查发生错误时,这个事件会触发
            updater.Error += (s, e) =>
            {
                var updater1 = s as FSLib.App.SimpleUpdater.Updater;
                logger.Info("update error!");
            };
            //没有找到更新的事件
            updater.NoUpdatesFound += (s, e) => { logger.Info("没有找到更新"); };
            //找到更新的事件.但在此实例中,找到更新会自动进行处理,所以这里并不需要操作
            updater.UpdatesFound += (s, e) =>
            {
                logger.Info("已找到更新");
            };

            /* 
			 * 1.注册程序集。当程序集被注册的时候，任何程序集中实现了 FSLib.App.SimpleUpdater.Defination.IUpdateNotify 接口的都将会被自动实例化并调用
			 *   通过此方法可以实现自己的事件捕捉以及处理类
			 *   此例中， 类 CustomConnect 将会被实例化并调用
			 */
            updater.UsingAssembly(System.Reflection.Assembly.GetExecutingAssembly());
            /*
			 * 2.自定义界面UI。此调用将会替换掉默认的更新界面（此例中将会把更新界面替换为 MainForm）
			 *   和上面的使用方法类似，但可实现完全自定义的效果
			 *   要用来替换的界面必须是基于 FSLib.App.SimpleUpdater.Dialogs.AbstractUpdateBase 派生的子类
			 */
            //updater.UsingFormUI<UpdateDialog>();

            //开始检查更新-这是最简单的模式.请现在 assemblyInfo.cs 中配置更新地址,参见对应的文件.
            //FSLib.App.SimpleUpdater.Updater.CheckUpdateSimple();

            //FSLib.App.SimpleUpdater.Updater.CheckUpdateSimple(AppContext.Instance.UpdateUrl, AppContext.Instance.XmlFile);

            ///*
            // * 如果您希望更加简单的使用而不用去加这样的属性，或者您想程序运行的时候自定义，您可以通过下列方式的任何一种方式取代上面的属性声明：
            // * 使用Updater.CheckUpdateSimple 的重载方法。这个重载方法允许你传入一个升级包的地址；
            // * 在检查前手动设置 FSLib.App.SimpleUpdater.Updater.UpdateUrl 属性。这是一个静态属性，也就是说，您并不需要创建 FSLib.App.SimpleUpdater.Updater.UpdateUrl 的对象实例就可以修改它。
            // */

            //FSLib.App.SimpleUpdater.Updater.CheckUpdateSimple("升级网址");

            #endregion

            logger.Info("Initiallize chromium core..");

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

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        private static void HandleRunningInstance(Process instance)
        {
            // 确保窗口没有被最小化或最大化 
            ShowWindowAsync(instance.MainWindowHandle, 4);
            // 设置真实例程为foreground  window  
            SetForegroundWindow(instance.MainWindowHandle);// 放到最前端 
        }

        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    // 确保例程从EXE文件运行 
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
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
