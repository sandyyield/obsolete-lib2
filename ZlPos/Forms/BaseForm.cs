using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZlPos.Bean;
using ZlPos.Core;
using Newtonsoft.Json;

namespace ZlPos.Forms
{
    public partial class BaseForm : Form
    {

        protected ChromiumWebBrowser browser;

        private bool canExecuteJavaScriptAsync = false;

        private Point mouseOff;//鼠标移动位置变量  

        private bool blnIsMoving;// 是否选选中状态

        public bool CanExecuteJavaScriptAsync { get => canExecuteJavaScriptAsync; set => canExecuteJavaScriptAsync = value; }

        public BaseForm()
        {
            Text = "zlpos";

            //TOFIX: 文件可以选择嵌入资源
            browser = new ChromiumWebBrowser(@"file:///F:/SelfTest/testJsCallNetMethod.html")
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(browser);

            BrowserLoading();

#if DEBUG
            FormBorderStyle = FormBorderStyle.Sizable;
#else
            FormBorderStyle = FormBorderStyle.None;
#endif
        }


        /// <summary>
        /// 加载browser事件
        /// </summary>
        private void BrowserLoading()
        {
            //浏览器未加载情况下
            browser.RegisterJsObject("boundAsync", new BoundObject()); //Standard object rego
            browser.RegisterJsObject("bound", new BoundObject(), false); //Use the default binder to serialize values into complex objects
            browser.RegisterJsObject("bound", new BoundObject(), true); //No camelcase of names and specify a default binder

            browser.LoadingStateChanged += OnLoadingStateChanged;
            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;
            browser.IsBrowserInitializedChanged += IsBrowserInitializedChanged;


            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion, bitness);
            DisplayOutput(version);

            // 移动事件
            MouseDown += new MouseEventHandler(Form_MouseDown);
            MouseMove += new MouseEventHandler(Form_MouseMove);
            MouseUp += new MouseEventHandler(Form_MouseUp);
        }

        #region 事件驱动

        private void IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            if (e.IsBrowserInitialized)
            {
                canExecuteJavaScriptAsync = true;
#if DEBUG
                //调试器
                browser.ShowDevTools();
#endif
            }

        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            //this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            //this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }

        private void SetCanGoBack(bool canGoBack)
        {
            //this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            //this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            //goButton.Text = isLoading ?
            //    "Stop" :
            //    "Go";
            //goButton.Image = isLoading ?
            //    Properties.Resources.nav_plain_red :
            //    Properties.Resources.nav_plain_green;

            //HandleToolStripLayout();
        }

        public void DisplayOutput(string output)
        {
            //this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            //var width = toolStrip1.Width;
            //foreach (ToolStripItem item in toolStrip1.Items)
            //{
            //    if (item != urlTextBox)
            //    {
            //        width -= item.Width - item.Margin.Horizontal;
            //    }
            //}
            //urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            //LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            //LoadUrl(urlTextBox.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }
        #endregion

        #region 无边框窗体拖动


        protected void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值  
                blnIsMoving = true;
            }
        }

        protected void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnIsMoving)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);
                Location = mouseSet;
            }
        }

        protected void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (blnIsMoving)
            {
                blnIsMoving = false;
            }
        }
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
            //canExecuteJavaScriptAsync = true;
#endif

            if (canExecuteJavaScriptAsync)
            {
                try
                {
                    //TODO：JS那边方法名待定
                    //browser.GetMainFrame().ExecuteJavaScriptAsync("JSmethod(" + resposeJson + ")");
                    browser.ExecuteScriptAsync("printInvokeJSMethod('hello world')");
                }
                catch(Exception e)
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
