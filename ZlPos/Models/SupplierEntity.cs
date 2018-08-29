using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class SupplierEntity
    {
        [SugarColumn(IsNullable = true, IsPrimaryKey = true)]
        public string id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string contactaddress { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mnemonic { get; set; }
        [SugarColumn(IsNullable = true)]
        public string name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string suppliercode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string contact { get; set; }
        [SugarColumn(IsNullable = true)]
        public string contactnum { get; set; }

        //2018年1月30日 add
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }

        [SugarColumn(IsNullable = true)]
        public string del { get; set; }

        // add: 2018/4/10
        [SugarColumn(IsNullable = true)]
        public string businessmodel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string businessmodelname { get; set; }

        //add 2018年8月27日
        [SugarColumn(IsNullable = true)]
        public string commissionrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string areaname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string areacode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string flag { get; set; }
        [SugarColumn(IsNullable = true)]
        public string required { get; set; }




    }
}
