using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class PayTypeEntity
    {
        public String id { get; set; }

        public String paytypecode { get; set; }

        public String name { get; set; }

        public String shopcode { get; set; }

        //2018年1月30日 add


        public String branchcode { get; set; }

        public String del { get; set; }
    }
}
