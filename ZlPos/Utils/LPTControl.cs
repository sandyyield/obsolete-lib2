using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ZlPos.Utils
{
    /// <summary>
    /// LPTControl 的摘要说明。
    /// </summary>
    public class LPTControl
    {
        private string LptStr = "lpt1";
        public LPTControl(string l_LPT_Str)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            LptStr = l_LPT_Str;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct OVERLAPPED
        {
            int Internal;
            int InternalHigh;
            int Offset;
            int OffSetHigh;
            int hEvent;
        }
        [DllImport("kernel32.dll")]
        private static extern int CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);
        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(int hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(int hObject);

        //ADD 2018年9月12日 增加flush方法尝试一下
        [DllImport("kernel32.dll")]
        private static extern int FlushFileBuffers(int hFile);


        private int iHandle;

        public int IHandle { get => iHandle; set => iHandle = value; }

        public bool Open()
        {
            iHandle = CreateFile(LptStr, 0x40000000, 0, 0, 3, 0, 0);
            if (iHandle != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Flush()
        {
            if (iHandle != -1)
            {
                FlushFileBuffers(iHandle);
            }
        }

        public bool Write(string Mystring)
        {
            if (iHandle != -1)
            {
                OVERLAPPED x = new OVERLAPPED();
                int i = 0;
                byte[] mybyte = StringUtils.CopyToBig(Encoding.Default.GetBytes(Mystring), new byte[] { 0x0d, 0x0a });
                bool b = WriteFile(iHandle, mybyte, mybyte.Length, ref i, ref x);
                return b;
            }
            else
            {
                //MessageBox.Show("不能连接到打印机!");
                return false;
            }
        }
        public bool Write(byte[] mybyte)
        {
            if (iHandle != -1)
            {
                OVERLAPPED x = new OVERLAPPED();
                int i = 0;
                WriteFile(iHandle, mybyte, mybyte.Length, ref i, ref x);
                return true;
            }
            else
            {
                //MessageBox.Show("不能连接到打印机!");
                return false;
            }
        }
        public bool Close()
        {
            bool res = CloseHandle(iHandle);
            iHandle = -1;
            return res;
        }
    }
}
