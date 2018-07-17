using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZlPos.Bizlogic;
using ZlPos.Core;

namespace ZlPos.Forms
{
    public partial class PosForm : Form
    {
        private ChromiumBrowserControl chromiumBrowser; //= new ChromiumBrowser();

        private ChromiumWebBrowser secondScreenWebView;

        //private BoundObject bound;

        private JSBridge hostApp = JSBridge.Instance;

        public PosForm()
        {
            //如果存在进程 则激活
            ThreadPool.RegisterWaitForSingleObject(Program.ProgramStarted, OnProgramStarted, null, -1, false);

            //hostApp = new HostApp();

            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            var fileVersion = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion);

            Text = version.ToString() + "_" + fileVersion.ToString();

            chromiumBrowser = new ChromiumBrowserControl(this)
            {
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(chromiumBrowser);

            //副屏初始化
            string secondScreenFile = System.AppDomain.CurrentDomain.BaseDirectory + "Html\\" + System.Configuration.ConfigurationManager.AppSettings["CustomerScreen"];//testJsCallNetMethod.html";
            secondScreenWebView = new ChromiumWebBrowser(@"file:///" + secondScreenFile.Replace("\\", "/"))
            //secondScreenWebView = new ChromiumWebBrowser("https://zhonglunnet032001.oss-cn-shanghai.aliyuncs.com/attachment/20180110/2587e38d-411c-4d9c-b0dd-7fe3159d129e.mp4")
            {
                Dock = DockStyle.Fill
            };

            hostApp._SecondScreenWebView = secondScreenWebView;
            hostApp.OpenSecondScreen();

            //this.KeyPreview = true;
            //KeyDown += PosForm_KeyDown;

            //bound = new BoundObject(chromiumBrowser);

            //用来ping js的
            //Button button = new Button()
            //{
            //    Dock = DockStyle.Top,
            //};
            //this.Controls.Add(button);

            //button.Text = "PING JS ";
            //button.Click += Button_Click;
        }

        

        //private void PosForm_KeyDown(object sender, KeyEventArgs e)
        //{
        //    MessageBox.Show("hello");
        //}

        private void Button_Click(object sender, EventArgs e)
        {
            //chromiumBrowser.ExecuteJavaScriptAsync(new object[] { });
            hostApp.ExecuteScriptAsync();

            //string path = Application.StartupPath + "\\DataBase\\zlCloudPos.db";
            //FileAttributes attr = File.GetAttributes(path);
            //if (attr == FileAttributes.Directory)
            //{
            //    Directory.Delete(path, true);
            //}
            //else
            //{
            //    File.Delete(path);
            //}
        }

        private void PosForm_Load(object sender, EventArgs e)
        {
            showOnMonitor(0);
        }

        private void showOnMonitor(int showOnMonitor)
        {
            Screen[] sc;
            sc = Screen.AllScreens;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(sc[showOnMonitor].Bounds.Left, sc[showOnMonitor].Bounds.Top);
            if (0 == showOnMonitor)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;// 当检测到一个屏幕则最大化显示但是不全屏
            }
            else
            {
                // If you intend the form to be maximized, change it to normal then maximized.  
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;// 设置边框为 None
            }
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;// 最大化
            this.ControlBox = true;
            //this.TopMost = true;// 置顶
        }

        /// <summary>
        /// 这里的逻辑可以后面在做退出的时候用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PosForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //try
            //{
            //    //chromiumBrowser.browser.CloseDevTools();
            //    chromiumBrowser.browser.GetBrowser().CloseBrowser(true);
            //    secondScreenWebView.GetBrowser().CloseBrowser(true);
            //}
            //catch { }

            //try
            //{
            //    if (chromiumBrowser.browser != null)
            //    {
            //        chromiumBrowser.browser.Dispose();
            //        if (secondScreenWebView != null)
            //        {
            //            secondScreenWebView.Dispose();
            //        }
            //        Cef.Shutdown();
            //    }
            //}
            //catch { }
            //Cef.Shutdown();
            //Application.Exit();
            //this.Dispose();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible == false || WindowState == FormWindowState.Minimized)
            {
                if (!this.Visible)
                {
                    this.Visible = true;
                }
                this.WindowState = FormWindowState.Maximized;
                this.Activate();
            }
            else if (this.Visible == true)
            {
                //this.Visible = false;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void PosForm_Shown(object sender, EventArgs e)
        {
            //this.Hide();

            //return;
        }

        public void HideSelf()
        {
            if (this.InvokeRequired)
            {
                Action action = () =>
                {
                    this.WindowState = FormWindowState.Minimized;
                };
                this.Invoke(action);
            }
        }

        /// <summary>
        /// 当受到第二个进程的通知时，显示窗体
        /// </summary>
        /// <param name="state"></param>
        /// <param name="timedOut"></param>
        private void OnProgramStarted(object state, bool timedOut)
        {
            ShowMine();
        }

        public void ShowMine()
        {
            if (this.InvokeRequired)
            {
                while (!this.IsHandleCreated)
                {
                    if (this.Disposing || this.IsDisposed)
                    {
                        return;
                    }
                }
                Action action = () =>
                {
                    this.notifyIcon1.ShowBalloonTip(2000, "DetectionTool2", "程序已经运行", ToolTipIcon.Info);
                    this.Show();

                    //注意：一定要在窗体显示后，再对属性进行设置  
                    this.WindowState = FormWindowState.Maximized;
                };
                this.Invoke(action);
            }
        }

    }
}
