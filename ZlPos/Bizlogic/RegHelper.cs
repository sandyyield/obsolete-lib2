using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZlPos.Bizlogic;

namespace ZlPos.Bizlogic
{
    public class RegHelper
    {
        //注册表里头存储格式
        public static string REGSOFTKEYNAME = "SOFTWARE\\ZlPos\\Keys";
        public static string REGSOFTTIME = "SOFTWARE\\ZlPos\\Times";
        public static string REGSOFTK2 = "Software\\ZL";
        public static string TIMEKEY = "CHECKTIMES";
        public static string REGFIPS = @"SYSTEM\CurrentControlSet\Control\Lsa\FipsAlgorithmPolicy";
        public static string IERUNLEAVE = @"ZlPos\Shell\open\command";
        public static string IERUNROOT = @"ZlPos";
        public static string FIPSKEY = @"SYSTEM\CurrentControlSet\Control\Lsa\FipsAlgorithmPolicy";

        public static bool regIslocked()
        {
            RegistryKey key = Registry.LocalMachine;
            try
            {
                RegistryKey software = key.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\system", false);

                if (software != null)
                    return false;

                if (software.GetValue("DisableRegistryTools") == null)
                    return false;

                if (software.GetValue("DisableRegistryTools").ToString() != "0")
                    return false;

                software.Close();

                return false;
            }
            catch (Exception ex)
            {
                throw new DeException("", "注册表检测出现异常", ex);
            }
        }

        ///// <summary>
        ///// 通过ProgID查找CLSID
        ///// </summary>
        ///// <param name="lpszProgID"></param>
        ///// <param name="pclsid"></param>
        ///// <returns></returns>
        //[DllImport("Ole32.dll", EntryPoint = "CLSIDFromProgID")]
        //public static extern long CLSIDFromProgID([In, MarshalAs(UnmanagedType.BStr)] string lpszProgID, [Out]out Guid pclsid);

        ///// <summary>
        ///// OCX或者dll的版本
        ///// </summary>
        ///// <param name="ocx"></param>
        ///// <returns></returns>
        //public static string GetVersion(string clsid)
        //{
        //    RegistryKey currentUser = Registry.ClassesRoot;
        //    string domainsKeyLocation = @"";
        //    int size = SysHelper.GetSysAddressWidth();
        //    if (size == 64)
        //    {
        //        domainsKeyLocation = @"Wow6432Node\CLSID\{" + clsid + @"}\InprocServer32";
        //    }
        //    else
        //    {
        //        domainsKeyLocation = @"CLSID\{" + clsid + @"}\InprocServer32";
        //    }

        //    RegistryKey respect = currentUser.OpenSubKey(domainsKeyLocation, true);
        //    string path = respect.GetValue("").ToString();
        //    if (File.Exists(path))
        //    {
        //        try
        //        {
        //            //TODO:这个不太好
        //            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(path);
        //            return myFileVersionInfo.FileVersion.Replace(",", ".").Replace(" ", "");
        //        }
        //        catch (Exception)
        //        {
        //            return "";
        //        }
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

       

        //public static void SaveUserPin(string deviceNum, string strUserPin)
        //{
        //    try
        //    {
        //        // 
        //        string strEncryUserPin = TripleDES.Encrypt3DES(strUserPin, deviceNum.PadRight(8, '0').Substring(0, 8));

        //        if (strEncryUserPin != "")
        //        {
        //            SetRegistryData(Registry.CurrentUser, REGSOFTKEYNAME, deviceNum, strEncryUserPin);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //可能是regedit里未设置好 跑一边设置看再点会不会出现问题
        //        SetFipsAlgorithmPolicy();
        //        throw new DeException("", "保存pin码时出现异常,已尝试自动修复，请退出软件并重新打开后再次尝试。", ex);
        //    }
        //}

        private static void SetFipsAlgorithmPolicy()
        {
            try
            {
                SetRegistryData(Registry.LocalMachine, REGFIPS, "Enabled", 0);
            }
            catch (Exception ex)
            {
                throw new DeException("-1", "SetFipsAlgorithmPolicy出现异常", ex);
            }
        }

        /// <summary>
        /// 在注册表中保存当前扫描时间
        /// </summary>
        public static void SaveRecTime()
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (nowTime != "")
            {
                SetRegistryData(Registry.CurrentUser, REGSOFTTIME, TIMEKEY, nowTime);
            }
        }

       

        /// <summary>
        /// 注册表相关设置
        /// </summary>
        public static void RemoveHTTPSCheck()
        {
            //将可信任站点https校验去除
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SoftWare", true);
            RegistryKey micr = key.OpenSubKey("Microsoft", true);
            RegistryKey win = micr.OpenSubKey("windows", true);
            RegistryKey cur = win.OpenSubKey("CurrentVersion", true);
            RegistryKey inter = cur.OpenSubKey("Internet Settings", true);
            RegistryKey zones = inter.OpenSubKey("Zones", true);
            RegistryKey respect = zones.OpenSubKey("2", true);
            respect.SetValue("Flags", 67);

            //修复不能当前安全设置不允许发送HTML、表单问题 by zhout 2017/8/24
            respect = zones.OpenSubKey("0", true);
            respect.SetValue("1200", 0);

            RegistryKey net = micr.OpenSubKey("Internet Explorer", true);
            if (net != null)
            {
                RegistryKey safe = net.OpenSubKey("Safety", true);
                if (safe != null)
                {
                    RegistryKey act = safe.OpenSubKey("ActiveXFiltering", true);
                    if (act != null)
                        act.SetValue("IsEnabled", 0);
                }
            }
        }

        /// <summary>
        /// 生成一个授权码
        /// </summary>
        public static void CreateAndSaveNewKey2()
        {
            try
            {
                Guid guid = Guid.NewGuid();

                SetRegistryData(Registry.CurrentUser, REGSOFTK2, "Key2", guid.ToString());
                //生成授权码认为是新用户  向服务器发送一个消息
                //CommitMsg.InitCommit();

            }
            catch (Exception ex)
            {
                //不能让用户看到
                throw new DeException("", "CreatAndCommitKey2Exception", ex);
            }
        }

        /// <summary>
        /// 获取授权码 如果为空则未生成授权码
        /// </summary>
        /// <returns></returns>
        public static string GetKey2()
        {
            try
            {
                string key2 = GetRegistryData(Registry.CurrentUser, REGSOFTK2, "Key2");
                if (key2 != "")
                {
                    return key2;
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }

        }

       

        /// <summary>
        /// 获取最近扫描时间
        /// </summary>
        /// <returns></returns>
        public static string GetLoadRecTime()
        {
            try
            {
                // 获取注册码
                string strRecCheckTime = GetRegistryData(Registry.CurrentUser, REGSOFTTIME, TIMEKEY);

                if (strRecCheckTime != "")
                {
                    return strRecCheckTime;
                }

                return "未开始扫描";
            }
            catch (Exception ex)
            {
                throw new DeException("", "GetLoadRecTime出现异常", ex);
            }
        }


        /// <summary>
        /// 读取指定名称的注册表的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetRegistryData(RegistryKey root, string subkey, string name)
        {
            string registData = "";
            RegistryKey myKey = root.OpenSubKey(subkey, true);
            if (myKey != null)
            {
                registData = myKey.GetValue(name).ToString();
            }

            return registData;
        }

        /// <summary>
        /// 创建注册表中根据介质号生成的key
        /// 
        /// 这个代码纯属扯淡
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="subkey"></param>
        /// <param name="name"></param>
        public static void CreateSubkey(RegistryKey root, string subkey, string name)
        {
            //TODO: 先判断是否存在当前项，没有则创建 有则返回
            RegistryKey myKey = root;
            myKey = myKey.OpenSubKey(subkey, true);
            if (myKey != null)
            {
                if (myKey.GetValue(name) == null)
                {
                    myKey.SetValue(name, "0");
                }
            }
            else
            {
                RegistryKey TempKey = root;
                myKey = root;
                string[] subkeyname = subkey.Split('\\');
                foreach (string strsubkeyname in subkeyname)
                {
                    TempKey = myKey.OpenSubKey(strsubkeyname, true);
                    if (TempKey != null)
                    {
                        myKey = TempKey;
                        continue;
                    }
                    else
                    {
                        myKey.CreateSubKey(strsubkeyname);
                        myKey = myKey.OpenSubKey(strsubkeyname, true);
                    }
                }
                myKey.SetValue(name, "0");
            }
        }

        /// <summary>
        /// 向注册表中写数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tovalue"></param> 
        public static void SetRegistryData(RegistryKey root, string subkey, string name, string value)
        {
            RegistryKey aimdir = root.CreateSubKey(subkey);
            aimdir.SetValue(name, value);
        }

        public static void SetRegistryData(RegistryKey root, string subkey, string name, int value)
        {
            RegistryKey aimdir = root.CreateSubKey(subkey);
            aimdir.SetValue(name, value, RegistryValueKind.DWord);
        }

        /// <summary>
        /// 删除注册表中指定的注册表项
        /// </summary>
        /// <param name="name"></param>
        public static void DeleteRegist(RegistryKey root, string subkey, string name)
        {
            string[] subkeyNames;
            RegistryKey myKey = root.OpenSubKey(subkey, true);
            subkeyNames = myKey.GetSubKeyNames();
            foreach (string aimKey in subkeyNames)
            {
                if (aimKey == name)
                    myKey.DeleteSubKeyTree(name);
            }
        }

        /// <summary>
        /// 判断指定注册表项是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsRegistryExist(RegistryKey root, string subkey, string name)
        {
            bool _exit = false;
            string[] subkeyNames;
            RegistryKey myKey = root.OpenSubKey(subkey, true);
            subkeyNames = myKey.GetSubKeyNames();
            foreach (string keyName in subkeyNames)
            {
                if (keyName == name)
                {
                    _exit = true;
                    return _exit;
                }
            }

            return _exit;
        }

        public static void InitOwnUrl()
        {
            SetRegistryData(Registry.ClassesRoot, IERUNROOT, "URL Protocol", "");
            SetRegistryData(Registry.ClassesRoot, IERUNLEAVE, "", "\"" + Application.StartupPath + "\\DetectionTool2.exe\" \"%1\"");
        }

        public static void RepaireFIP()
        {
            //TODO:做Fips修复
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa\FipsAlgorithmPolicy", true);
                if (regKey.GetValue("Enabled").ToString() == "1")
                {
                    regKey.SetValue("Enabled", 0);
                }

            }
            catch (Exception ex)
            {
                try
                {
                    //尝试直接添加Fip
                    SetRegistryData(Registry.LocalMachine, FIPSKEY, "Enabled", 0);
                }
                catch{}
                return;
            }
        }
    }
}
