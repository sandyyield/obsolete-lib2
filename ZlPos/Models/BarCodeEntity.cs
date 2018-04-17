using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class BarCodeEntity
    {
        [SugarColumn(IsNullable = true,IsPrimaryKey = true)]
        public string uid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string barcodes { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditycode { get; set; }

    }
}
