using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using CefSharp.WinForms;
//using CefSharp;
using ZlPos.Bizlogic;
using System.Configuration;
using System.Threading;
using log4net;
using System.Reflection;
using System.Threading.Tasks;
using ZlPos.Dao;
using ZlPos.Utils;
using ZlPos.Models;
using ZlPos.Bean;
using ZlPos.Config;
using ZlPos.Forms;

namespace ZlPos.Core
{
    /// <summary>
    /// create by sVen 2018年3月15日
    /// </summary>
    public partial class ChromiumBrowserControl : UserControl
    {
        //加载依赖程序集 
        Assembly assemblyCefSharp = Assembly.LoadFrom(Application.StartupPath + "\\CefSharp.dll");
        Assembly assemblyCefSharp_core = Assembly.LoadFrom(Application.StartupPath + "\\CefSharp.Core.dll");
        Assembly assemblyCefSharp_WinForms = Assembly.LoadFrom(Application.StartupPath + "\\CefSharp.WinForms.dll");

        Type t_ChromiumWebBrowser;

        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //public ChromiumWebBrowser browser;
        public Control browser;

        public PosForm _parentForm;

        private JSBridge jsBridge = JSBridge.Instance;

        private bool canExecuteJavaScriptAsync = false;

        public bool CanExecuteJavaScriptAsync { get => canExecuteJavaScriptAsync; set => canExecuteJavaScriptAsync = value; }
        //internal JSBridge HostApp { get => hostApp; set => hostApp = value; }
        internal JSBridge JsBridge { get => jsBridge; set => jsBridge = value; }

        public ChromiumBrowserControl(PosForm parentForm, object boundObject = null)
        {
            //Text = "zlpos";

            //string htmlFile = System.AppDomain.CurrentDomain.BaseDirectory + "Html\\" + ConfigurationManager.AppSettings["HTML5FileName"];//testJsCallNetMethod.html";
            //browser = new ChromiumWebBrowser(@"file:///" + htmlFile.Replace("\\", "/"))
            ////browser = new ChromiumWebBrowser(@"file:///E:/ZlPos/newH5andorid/newH5/home.html")
            ////browser = new ChromiumWebBrowser(@"https://zhonglunnet032001.oss-cn-shanghai.aliyuncs.com/attachment/20180110/2587e38d-411c-4d9c-b0dd-7fe3159d129e.mp4")
            //{
            //    KeyboardHandler = new KeyBoardHander(),
            //    Dock = DockStyle.Fill,
            //};
            //browser.MenuHandler = new MenuHandler();

            try
            {
                string htmlFile = System.AppDomain.CurrentDomain.BaseDirectory + "Html\\" + ConfigurationManager.AppSettings["HTML5FileName"];
                t_ChromiumWebBrowser = assemblyCefSharp_WinForms.GetType("CefSharp.WinForms.ChromiumWebBrowser");
                browser = Activator.CreateInstance(t_ChromiumWebBrowser, new object[] { @"file:///" + htmlFile.Replace("\\", "/") }) as Control;
                t_ChromiumWebBrowser.GetProperty("Dock").SetValue(browser, DockStyle.Fill, null);
                t_ChromiumWebBrowser.GetProperty("KeyboardHandler").SetValue(browser, new KeyBoardHander(), null); //快捷键
                t_ChromiumWebBrowser.GetProperty("MenuHandler").SetValue(browser, new MenuHandler(), null);
            }
            catch (Exception e)
            {

            }

            Controls.Add(browser);

            _parentForm = parentForm;

            BrowserLoading();

            InitializeComponent();

        }

        /// <summary>
        /// 加载browser事件
        /// </summary>
        private void BrowserLoading()
        {

            //host注入当前webbrowser
            JsBridge.Browser = browser;

            //传入主窗体句柄
            JsBridge.frmMain = _parentForm;

            //(browser as ChromiumWebBrowser).RegisterJsObject("HostApp", JsBridge);// new BoundObject(browser), true); //No camelcase of names and specify a default binder
            //(browser as ChromiumWebBrowser).LoadingStateChanged += OnLoadingStateChanged;
            //(browser as ChromiumWebBrowser).ConsoleMessage += OnBrowserConsoleMessage;
            //(browser as ChromiumWebBrowser).StatusMessage += OnBrowserStatusMessage;
            //(browser as ChromiumWebBrowser).TitleChanged += OnBrowserTitleChanged;
            //(browser as ChromiumWebBrowser).AddressChanged += OnBrowserAddressChanged;
            //(browser as ChromiumWebBrowser).IsBrowserInitializedChanged += IsBrowserInitializedChanged;

            try
            {

                var method = t_ChromiumWebBrowser.GetMethod("RegisterJsObject", new Type[] { typeof(string), typeof(object), typeof(bool) });
                method?.Invoke(browser, new object[] { "HostApp", JsBridge });
            }
            catch(Exception e)
            {
                logger.Error("register js object err", e);
            }


            //注册DevTools热键
            //this.KeyDown += Browser_KeyDown;

            //启动时候需要加载一下打印机设置
            Task.Factory.StartNew(action: new Action(InitPrinterManangerAsync));


            //var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            //var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion, bitness);
            //DisplayOutput(version);

        }

        private void InitPrinterManangerAsync()
        {
            try
            {
                PrinterConfigEntity printerConfigEntity;
                //先从数据库取出上次缓存的配置
                using (var db = SugarDao.Instance)
                {
                    printerConfigEntity = db.Queryable<PrinterConfigEntity>().First();
                }
                if (printerConfigEntity != null)
                {
                    PrinterSetter printerSetter = new PrinterSetter();
                    //委托js回调方法
                    //JsCallbackHandle jsCallbackHandle = new JsCallbackHandle(CallbackMethod4SetPrinter);
                    //_printerConfigEntity = printerConfigEntity;
                    //printerSetter.SetPrinter(printerConfigEntity, ()=> "haha");
                    printerSetter.SetPrinter(printerConfigEntity, CallbackMethod4InitPrinter);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message + e.StackTrace);
            }
        }

        private void CallbackMethod4InitPrinter(object state)
        {
            object[] paramsArr = (object[])state;
            string methodName = paramsArr[0] as string;
            ResponseEntity responseEntity = paramsArr[1] as ResponseEntity;
            if (responseEntity.code == ResponseCode.SUCCESS)
            {
                logger.Info("初始化打印机成功");
            }
            else
            {
                logger.Info("打印机设置失败" + responseEntity.msg);
            }
        }




        //private void Browser_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.F12)
        //    {
        //        browser.ShowDevTools();
        //    }
        //}
        #region 事件驱动

//        private void IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
//        {
//            if (e.IsBrowserInitialized)
//            {
//                //_parentForm.HideSelf();
//                //canExecuteJavaScriptAsync = true;
//#if DEBUG
//                ////调试器
//                //browser.ShowDevTools();
//#endif
//            }

//        }

//        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
//        {
//            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
//        }

//        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
//        {
//            //this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
//        }

//        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
//        {
//            SetCanGoBack(args.CanGoBack);
//            SetCanGoForward(args.CanGoForward);

//            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
//        }

//        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
//        {
//            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
//        }

//        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
//        {
//            //this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
//        }

//        private void SetCanGoBack(bool canGoBack)
//        {
//            //this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
//        }

//        private void SetCanGoForward(bool canGoForward)
//        {
//            //this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
//        }

//        private void SetIsLoading(bool isLoading)
//        {
//            //goButton.Text = isLoading ?
//            //    "Stop" :
//            //    "Go";
//            //goButton.Image = isLoading ?
//            //    Properties.Resources.nav_plain_red :
//            //    Properties.Resources.nav_plain_green;

//            //HandleToolStripLayout();
//        }

//        public void DisplayOutput(string output)
//        {
//            //this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
//        }

//        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
//        {
//            HandleToolStripLayout();
//        }

//        private void HandleToolStripLayout()
//        {
//            //var width = toolStrip1.Width;
//            //foreach (ToolStripItem item in toolStrip1.Items)
//            //{
//            //    if (item != urlTextBox)
//            //    {
//            //        width -= item.Width - item.Margin.Horizontal;
//            //    }
//            //}
//            //urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
//        }

//        private void ExitChromiumBrowser(object sender, EventArgs e)
//        {
//            browser.Dispose();
//            Cef.Shutdown();

//            this.Dispose();
//        }

//        private void GoButtonClick(object sender, EventArgs e)
//        {
//            //LoadUrl(urlTextBox.Text);
//        }

//        private void BackButtonClick(object sender, EventArgs e)
//        {
//            (browser as ChromiumWebBrowser).Back();
//        }

//        private void ForwardButtonClick(object sender, EventArgs e)
//        {
//            (browser as ChromiumWebBrowser).Forward();
//        }

//        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode != Keys.Enter)
//            {
//                return;
//            }

//            //LoadUrl(urlTextBox.Text);
//        }

//        private void LoadUrl(string url)
//        {
//            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
//            {
//                (browser as ChromiumWebBrowser).Load(url);
//            }
//        }
        #endregion

        public void ExecuteJavaScriptAsync(object state)
        {

            //if (state == null)
            //{
            //    throw new Exception("执行参数为NULL");
            //}

            //ResposeEntity resposeEntity = state as ResposeEntity;

            //string resposeJson = JsonConvert.SerializeObject(resposeEntity);

#if DEBUG
            CanExecuteJavaScriptAsync = true;
#endif

            if (CanExecuteJavaScriptAsync)
            {
                try
                {
                    //TODO：JS那边方法名待定
                    //browser.ExecuteScriptAsync("printInvokeJSMethod('hello world')");
                    Type t_WebBrowserExtensions = assemblyCefSharp.GetType("CefSharp.WebBrowserExtensions");
                    MethodInfo ExecuteScriptAsyncMethod = t_WebBrowserExtensions.GetMethods().FirstOrDefault(m => m.Name == "ExecuteScriptAsync" && m.GetParameters().Length == 2);
                    ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "printInvokeJSMethod('hello world')" });
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                //throw new Exception("不允许执行");
            }
        }

    }
}
