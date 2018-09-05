using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Utils
{
    public class StringUtils
    {
        /// <summary>
        /// 将普通字符串转换成16进制的字符串。
        /// </summary>
        /// <param name="_str">要转换的字符串。</param>
        /// <returns></returns>
        public static string StringToHex16String(string _str)
        {
            //将字符串转换成字节数组。
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(_str);
            //定义一个string类型的变量，用于存储转换后的值。
            string result = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                //将每一个字节数组转换成16进制的字符串，以空格相隔开。
                result += Convert.ToString(buffer[i], 16);
            }
            return result.ToUpper();
        }

        /// <summary>
        /// hex string 转 byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        //public static string ConvertStringToHex(string strASCII, string separator = null)
        //{
        //    StringBuilder sbHex = new StringBuilder();
        //    foreach (char chr in strASCII)
        //    {
        //        sbHex.Append(String.Format("{0:X2}", Convert.ToInt32(chr)));
        //        sbHex.Append(separator ?? string.Empty);
        //    }
        //    return sbHex.ToString();
        //}

        //public static string ConvertHexToString(string HexValue, string separator = null)
        //{
        //    HexValue = string.IsNullOrEmpty(separator) ? HexValue : HexValue.Replace(string.Empty, separator);
        //    StringBuilder sbStrValue = new StringBuilder();
        //    while (HexValue.Length > 0)
        //    {
        //        sbStrValue.Append(Convert.ToChar(Convert.ToUInt32(HexValue.Substring(0, 2), 16)).ToString());
        //        HexValue = HexValue.Substring(2);
        //    }
        //    return sbStrValue.ToString();
        //}



    }

}
