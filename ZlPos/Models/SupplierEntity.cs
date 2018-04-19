using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class SupplierEntity
    {
        [SugarColumn(IsNullable = true,IsPrimaryKey = true)]
        public String id { get; set; }
        [SugarColumn(IsNullable = true)]
        public String contactaddress { get; set; }
        [SugarColumn(IsNullable = true)]
        public String mnemonic { get; set; }
        [SugarColumn(IsNullable = true)]
        public String name { get; set; }
        [SugarColumn(IsNullable = true)]
        public String suppliercode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String contact { get; set; }
        [SugarColumn(IsNullable = true)]
        public String contactnum { get; set; }

        //2018年1月30日 add
        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }

        [SugarColumn(IsNullable = true)]
        public String del { get; set; }
    }
}
