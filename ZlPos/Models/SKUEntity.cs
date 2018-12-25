using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class SKUEntity
    {
        [SugarColumn(IsIgnore = true)]
        public string id { get; set; }
        [SugarColumn(IsNullable = true,IsPrimaryKey = true)]
        public string uid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string spucode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string buyprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string saleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string dispatchprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string wholesaleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speccode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specvalue { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commodityclassify { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updatetime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updateuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ordernum { get; set; }
        [SugarColumn(IsNullable = true)]
        public string del { get; set; }
        [SugarColumn(IsNullable = true)]
        public string datalevel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv1 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv2 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv3 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv4 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv5 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv6 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speccode01 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specvalue01 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speccode02 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specvalue02 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speccode03 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specvalue03 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string skucode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string required { get; set; }

        //spu表中复制过来属性begin
        [SugarColumn(IsNullable = true)]
        public string commoditystatus { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updownstatus { get; set; }
        [SugarColumn(IsNullable = true)]
        public string pricing { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categorycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categoryname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string plu { get; set; }
        [SugarColumn(IsNullable = true)]
        public string validtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commodityname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mnemonic { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category1code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category2code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category3code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category4code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category1name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category2name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category3name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category4name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speclevel { get; set; }


        [SugarColumn(IsIgnore = true)]
        public string barcode { get; set; }
        
        //add 2018年12月24日
        [SugarColumn(IsNullable = true)]
        public string specname01 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specname02 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specname03 { get; set; }
    }
}
