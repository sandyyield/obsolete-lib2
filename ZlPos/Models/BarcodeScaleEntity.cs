using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class BarcodeScaleEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public string scaleNo { get; set; }

        [SugarColumn(IsNullable = true)]
        public string sacleName { get; set; }

        [SugarColumn(IsNullable = true)]
        public string sacleIp { get; set; }

    }
}
