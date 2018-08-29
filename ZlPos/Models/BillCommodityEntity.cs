using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class BillCommodityEntity
    {
        [SugarColumn(IsNullable = true)]
        public string commodityclassify { get; set; }
        [SugarColumn(IsNullable = true)]
        public string countcardnums { get; set; }
        [SugarColumn(IsNullable = true)]
        public string score { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditytype { get; set; }
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = false)]
        public string uid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ticketcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string saletime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commodityname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string spec { get; set; }
        [SugarColumn(IsNullable = true)]
        public string unitcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string unitname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string tradeid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string tradename { get; set; }
        [SugarColumn(IsNullable = true)]
        public string salenums { get; set; }
        [SugarColumn(IsNullable = true)]
        public string buyprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string saleprice { get; set; }


        [SugarColumn(IsNullable = true)]
        public string paysaleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string paysubtotal { get; set; }
        [SugarColumn(IsNullable = true)]
        public string subtotal { get; set; }
        [SugarColumn(IsNullable = true)]
        public string assistantid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string assistantname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commissionprice { get; set; }

        [SugarColumn(IsNullable = true)]
        public string cashierid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string cashiername { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categorycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categoryname { get; set; }



        [SugarColumn(IsNullable = true)]
        public string brandcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string brandname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string canreturnnums { get; set; }
        [SugarColumn(IsNullable = true)]
        public string canreturnsubtotal { get; set; }
        [SugarColumn(IsNullable = true)]
        public string iscrossbranch { get; set; }
        [SugarColumn(IsNullable = true)]
        public string coupontotalmoney { get; set; }
        [SugarColumn(IsNullable = true)]
        public string vouchertotalmoney { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberdiscountprice { get; set; }


        [SugarColumn(IsNullable = true)]
        public string holediscountprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string pricing { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string businessmodel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string businessmodelname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commissionrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string suppliercode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string suppliername { get; set; }
        [SugarColumn(IsNullable = true)]
        public string intaxrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string outtaxrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string canstock { get; set; }
        [SugarColumn(IsNullable = true)]
        public string dispatchprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string wholesaleprice { get; set; }


        [SugarColumn(IsNullable = true)]
        public string memberprice { get; set; }

        //add 2018年6月26日
        [SugarColumn(IsNullable = true)]
        public string commission { get; set; }

        //add 2018年8月29日
        [SugarColumn(IsNullable = true)]
        public string barcodes { get; set; }
    }
}
