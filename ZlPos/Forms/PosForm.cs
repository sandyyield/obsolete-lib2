using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

            //hostApp = new HostApp();

            InitializeComponent();

            chromiumBrowser = new ChromiumBrowserControl()
            {
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(chromiumBrowser);

            //副屏初始化
            string secondScreenFile = System.AppDomain.CurrentDomain.BaseDirectory + "Html\\" + System.Configuration.ConfigurationManager.AppSettings["CustomerScreen"];//testJsCallNetMethod.html";
            secondScreenWebView = new ChromiumWebBrowser(@"file:///" + secondScreenFile.Replace("\\", "/"))
            //secondScreenWebView = new ChromiumWebBrowser()
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
    }
}
