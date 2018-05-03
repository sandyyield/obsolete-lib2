using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZlPos.Forms
{
    public partial class SecondScreenFrm : Form
    {
        public static ChromiumWebBrowser chromiumWebBrowser;

        public SecondScreenFrm(ChromiumWebBrowser secondScreenChromiumWebBrowser)
        {
            chromiumWebBrowser = secondScreenChromiumWebBrowser;
            chromiumWebBrowser.Dock = DockStyle.Fill;
            Controls.Add(chromiumWebBrowser);
            InitializeComponent();
        }

        private void SecondScreenFrm_Load(object sender, EventArgs e)
        {
            showOnMonitor(1);
        }

        private void showOnMonitor(int showOnMonitor)
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            if (showOnMonitor >= sc.Length)
            {
                showOnMonitor = 0;
                //this.Close();
            }

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
            this.WindowState = FormWindowState.Maximized;// 最大化
            this.ControlBox = true;
            this.TopMost = true;// 置顶
        }



    }
}
