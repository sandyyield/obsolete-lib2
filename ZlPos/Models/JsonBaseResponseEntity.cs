using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class JsonBaseResponseEntity
    {
        public int success { get; set; }
        public int errcode { get; set; }
        public String errmsg { get; set; }
        public String result { get; set; }
    }
}
