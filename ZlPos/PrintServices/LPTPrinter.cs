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
            if(lptControl != null)
            {
                lptControl.Close();
                Enable = false;
            }
        }

        public void PrintString(string txt)
        {
            if (Enable)
            {
                lptControl.Write(txt);
            }
        }

        public void OpenCash(string str)
        {
            if (Enable)
            {
                lptControl.Write(str);
            }
        }

        internal void OpenCash()
        {
            if (Enable)
            {

                lptControl.Write(new byte[] { 0x1b, 0x70, 0x00, 0x10, 0x90 });
            }
        }
    }
}
