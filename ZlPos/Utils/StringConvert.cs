using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Utils
{
    class StringConvert
    {
        internal static string getWordCode(string word)
        {
            string s = "";
            try
            {
                byte[] bs = Encoding.GetEncoding("GBK").GetBytes(word);
                for (int i = 0; i < bs.Length; i++)
                {
                    //int a = Convert.ToInt32(bytes2HexString(bs[i]), 16);
                    int a = Convert.ToInt32(bs[i]);
                    int code = a - 160;
                    if (code < 0)
                    {//特殊字符处理
                     //查ASCII码和区位码对照图，得出第三分区区位码的位码==ASCII-32
                        code = a - 32;
                        if (code < 0)
                        {

                        }
                        else if (code < 10)
                        {
                            s += "030" + code;
                        }
                        else if(code < 100)
                        {
                            s += "03" + code;
                        }
                    }
                    else if (code > 0 && code < 10)
                    {
                        s += "0" + code;
                    }
                    else
                    {
                        s += code + "";
                    }

                }
            }
            catch (Exception e)
            {

            }
            return s;
        }

        public static string bytes2HexString(byte b)
        {
            return bytes2HexString(new byte[] { b });
        }

        public static string bytes2HexString(byte[] b)
        {
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                string hex = (b[i] & 0xFF).ToString() ;
                if (hex.Length == 1)
                {
                    hex = '0' + hex;
                }
                ret += hex.ToUpper();
            }
            return ret;
        }





        internal static byte[] convertStringToBytesForTMC(string str)
        {
            char[] chars = str.ToCharArray();
            byte[] r1 = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                //byte[] arr = HexUtils.HexStringToByte(chars[i].ToString());   //hexStringToBytes2(Convert.ToInt32(chars[i]).ToString());
                byte[] arr = HexUtils.HexStringToByte(String.Format("{0:X}", Convert.ToInt32(chars[i])));
                r1[i] = arr[0];
            }

            int len = r1.Length;
            byte[] r2 = new byte[len + 3];
            for (int i = 0; i < len; i++)
            {
                r2[i] = r1[i];
            }
            r2[len] = 0x0D;
            r2[len + 1] = 0x0A;
            r2[len + 2] = 0x03;
            return r2;
        }

        internal static byte[] convertStringToBytesForTMCBLS(string str)
        {
            char[] chars = str.ToCharArray();
            byte[] r1 = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                //byte[] arr = HexUtils.HexStringToByte(chars[i].ToString());   //hexStringToBytes2(Convert.ToInt32(chars[i]).ToString());
                byte[] arr = HexUtils.HexStringToByte(String.Format("{0:X}", Convert.ToInt32(chars[i])));
                r1[i] = arr[0];
            }

            int len = r1.Length;
            byte[] r2 = new byte[len + 3];
            for (int i = 0; i < len; i++)
            {
                r2[i] = r1[i];
            }
            r2[len] = 0x0D;
            r2[len + 1] = 0x0A;
            return r2;
        }


        //public static byte[] hexStringToBytes2(string hexString)
        //{
        //    if (string.IsNullOrEmpty(hexString))
        //    {
        //        return null;
        //    }
        //    hexString = hexString.ToUpper();
        //    int length = hexString.Length / 2;
        //    char[] hexChars = hexString.ToCharArray();
        //    byte[] d = new byte[length];
        //    for (int i = 0; i < length; i++)
        //    {
        //        int pos = i * 2;
        //        d[i] = (byte)(charToByte(hexChars[pos]) << 4 | charToByte(hexChars[pos + 1]));
        //    }
        //    return d;
        //}

        public static byte charToByte(char c)
        {
            return (byte)"0123456789ABCDEF".IndexOf(c);
        }
    }
}
