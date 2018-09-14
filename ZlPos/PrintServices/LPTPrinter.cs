using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Utils;

namespace ZlPos.PrintServices
{
    public class LPTPrinter
    {
        public string lptPort { get; set; }
        public string pageWidth { get; set; }

        public bool Enable { get => enable; set => enable = value; }

        private bool enable = false;

        private LPTControl lptControl;

        public bool Init()
        {
            if (Enable == false)
            {
                if (!string.IsNullOrEmpty(lptPort))
                {

                    lptControl = new LPTControl(lptPort);
                    lptControl.Open();
                    Enable = true;
                    return true;
                }
                return false;
            }
            return false;
        }

        public void Close()
        {
            if (lptControl != null)
            {
                lptControl.Close();
                Enable = false;
            }
        }

        public void PrintString(string txt)
        {
            if (Enable)
            {
                if (lptControl != null && lptControl.IHandle != -1)
                {
                    lptControl.Close();
                }
                lptControl.Open();
                lptControl.Write(txt);
                lptControl.Close();
                //lptControl.Flush();
            }
        }

        public void OpenCash(string str)
        {
            if (Enable)
            {
                if (lptControl != null && lptControl.IHandle != -1)
                {
                    lptControl.Close();
                }
                lptControl.Open();
                lptControl.Write(str);
                lptControl.Close();
            }
        }

        internal void OpenCash()
        {
            if (Enable)
            {
                if (lptControl != null && lptControl.IHandle != -1)
                {
                    lptControl.Close();
                }
                lptControl.Open();
                lptControl.Write(new byte[] { 0x1b, 0x70, 0x00, 0x10, 0x90 });
                lptControl.Close();
            }
        }
    }
}
