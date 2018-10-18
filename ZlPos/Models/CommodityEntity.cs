using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    [SugarTable("CommodityEntity")]
    public class CommodityEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsNullable = true)]
        public string id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commodityname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditypic { get; set; }

        [SugarColumn(IsNullable = true)]
        public string spec { get; set; }
        [SugarColumn(IsNullable = true)]
        public string buyprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string saleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string wholesaleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categorycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categoryname { get; set; }

        [SugarColumn(IsNullable = true)]
        public string canscore { get; set; }
        [SugarColumn(IsNullable = true)]
        public string score { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shortname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mnemonic { get; set; }
        [SugarColumn(IsNullable = true)]
        public string brandcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string brandname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string unitcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string unitname { get; set; }

        [SugarColumn(IsNullable = true)]
        public string expiretime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string suppliercode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string suppliername { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string buyspec { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditystatus { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditytype { get; set; }
        [SugarColumn(IsNullable = true)]
        public string pricing { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commission { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commissionratio { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commissionmoney { get; set; }
        [SugarColumn(IsNullable = true)]
        public string del { get; set; }
        [SugarColumn(IsNullable = true)]
        public string creattime { get; set; }

        // TODO: 2017/10/17
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }
        // add: 2018/2/26
        [SugarColumn(IsNullable = true)]
        public string commoditycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string businessmodel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string businessmodelname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commissionrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string intaxrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string outtaxrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string canstock { get; set; }
        [SugarColumn(IsNullable = true)]
        public string dispatchprice { get; set; }



        //add 2018年3月14日
        [SugarColumn(IsNullable = true)]
        public String commodityclassify { get; set; }

        //add 2018年6月7日
        [SugarColumn(IsNullable = true)]
        public string validtime { get; set; }



        // add: 2018/6/12  条码秤需要字段,不入库 
        //2018年7月17日 改为需要入库
        [SugarColumn(IsNullable = true)]
        public string plu { get; set; }

        private string tare1 = "0";
        [SugarColumn(IsIgnore = true)]
        public string tare { get => tare1; set => tare1 = value; }//皮重


        [SugarColumn(IsIgnore = true)]
        public string barcode { get; set; }


        //add 2018年8月27日
        [SugarColumn(IsNullable = true)]
        public string updatetime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public string storagetype { get; set; }
        [SugarColumn(IsNullable = true)]
        public string stylecode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ordernum { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string dishid { get; set; }

        //add 2018年9月3日
        [SugarColumn(IsNullable = true)]
        public string updateuser { get; set; }

        //add 2018年10月12日
        [SugarColumn(IsNullable = true)]
        public string spucode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditylevel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speccode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specvalue { get; set; }
        [SugarColumn(IsNullable = true)]
        public string season { get; set; }

        //不入库，不返回，用来接收服装版打印标签的规格
        [SugarColumn(IsIgnore = true)]
        public string size { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string color { get; set; }


    }
}
