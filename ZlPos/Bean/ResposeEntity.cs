using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Bean
{
    public class ResponseEntity
    {
        public int code { get; set; }

        public string msg { get; set; }

        public object data { get; set; }

        public object obj { get; set; }

        //public int Code { get => code; set => code = value; }
        //public string Msg { get => msg; set => msg = value; }
        //public BaseData Data { get => data; set => data = value; }
        //public object Obj { get => obj; set => obj = value; }
    }
}
