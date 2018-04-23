using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Bizlogic
{
    /// <summary>
    /// 封装异常
    /// </summary>
    public class DeException : Exception
    {
        public string ErrorCode { get; set; }
        public DeException(string strErrorCode, string strExcepitonMsg)
            : base(strExcepitonMsg)
        {
            this.ErrorCode = strErrorCode;
        }

        public DeException(string strErrorCode, string strExcepitonMsg, Exception ex)
           : base(strExcepitonMsg, ex)
        {
            this.ErrorCode = strErrorCode;
        }
    }
}
