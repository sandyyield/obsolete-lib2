using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Bean
{
    class ResponseEntity
    {
        private int code;

        private string msg;

        private BaseData data;

        private object obj;

        public int Code { get => code; set => code = value; }
        public string Msg { get => msg; set => msg = value; }
        internal BaseData Data { get => data; set => data = value; }
        public object Obj { get => obj; set => obj = value; }
    }
}
