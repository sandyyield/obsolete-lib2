using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ZlPos.Forms;

namespace ZlPos
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Cef.Initialize(new CefSettings(), true, true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PosForm());
        }
    }
}
