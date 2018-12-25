using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    [SugarTable("BarCodeEntity")]
    public class BarCodeEntity
    {
        [SugarColumn(IsNullable = true, IsPrimaryKey = true)]
        public string uid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string del { get; set; }//0: ,1
        [SugarColumn(IsNullable = true)]
        public string spucode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string barcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updatetime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updateuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public string datalevel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string skucode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string required { get; set; }

    }
}
