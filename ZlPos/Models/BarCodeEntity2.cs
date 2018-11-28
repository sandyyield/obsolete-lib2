using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 条码表，用来对应条码和商品的关系，一个商品可能对应多个条码
/// </summary>
namespace ZlPos.Models
{
    public class BarCodeEntity2
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public string id { get; set; }

        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }

        [SugarColumn(IsNullable = true)]
        public string barcode { get; set; }

        [SugarColumn(IsNullable = true)]
        public string commoditycode { get; set; }

        [SugarColumn(IsNullable = true)]
        public string del { get; set; }

        //add 2018年11月15日
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }

        [SugarColumn(IsNullable = true)]
        public string branchname { get; set; }

        [SugarColumn(IsNullable = true)]
        public string datalevel { get; set; }

        //add 2018年11月27日
        [SugarColumn(IsNullable = true)]
        public string spucode { get; set; }

    }
}
