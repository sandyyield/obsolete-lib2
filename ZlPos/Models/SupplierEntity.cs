using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class SupplierEntity
    {
        public String id { get; set; }

        public String contactaddress { get; set; }

        public String mnemonic { get; set; }

        public String name { get; set; }

        public String suppliercode { get; set; }

        public String shopcode { get; set; }

        public String contact { get; set; }

        public String contactnum { get; set; }

        //2018年1月30日 add

        public String branchcode { get; set; }


        public String del { get; set; }
    }
}
